using System.IO;
using System.Threading;
using AzureIndexer.Api.Controllers;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.IoC;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBitcoin.Networks;
using Newtonsoft.Json;
using Serilog;
using Stratis.Bitcoin.Networks;
using Stratis.Sidechains.Networks;
using Swashbuckle.AspNetCore.Swagger;

namespace AzureIndexer.Api
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Extras.CommonServiceLocator;
    using CommonServiceLocator;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var network = this.GetNetwork();
            NetworkRegistration.Register(network);

            services
                .AddMvc(options =>
                    {
                        options.Filters.Add<WebApiExceptionActionFilter>();
                        options.Filters.Add<GlobalExceptionFilter>();
                    })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IHostedService, BuildChainCache>();
            services.AddSingleton<IHostedService, UpdateChainListener>();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Azure Indexer API", Version = "v1" });
                c.DocInclusionPredicate((value, description) =>
                            description.ActionDescriptor.DisplayName.Contains("AzureIndexer.Api") && !description.ActionDescriptor.DisplayName.Contains("Main"));
            });

            // Create the container builder.
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule<AutomapperModule>();
            builder.RegisterInstance(Log.Logger).As<Serilog.ILogger>();
            builder.Register(
                ctx =>
                {
                    var loggerFactory = ctx.Resolve<ILoggerFactory>();
                    var config = new QBitNinjaConfiguration(this.Configuration, loggerFactory);
                    config.Indexer.EnsureSetup();
                    return config;
                }).As<QBitNinjaConfiguration>().SingleInstance();
            builder.Register(
                ctx =>
                {
                    var config = ctx.Resolve<QBitNinjaConfiguration>();
                    return config.Indexer.CreateIndexerClient();
                }).As<IndexerClient>();
            builder.Register(ctx =>
            {
                var config = ctx.Resolve<QBitNinjaConfiguration>();
                var chain = new ConcurrentChain(config.Indexer.Network);

                return chain;
            }).As<ConcurrentChain>().SingleInstance();

            builder.RegisterType<TransactionSearchService>().As<ITransactionSearchService>();
            builder.RegisterType<BalanceSearchService>().As<IBalanceSearchService>();
            builder.RegisterType<BlockSearchService>().As<IBlockSearchService>();
            builder.RegisterType<SmartContractSearchService>().As<ISmartContractSearchService>();
            builder.RegisterType<MainController>().AsSelf();
            builder.RegisterType<ChainCacheProvider>().AsSelf();
            builder.RegisterType<WhatIsIt>().AsSelf();
            this.ApplicationContainer = builder.Build();

            var csl = new AutofacServiceLocator(this.ApplicationContainer);
            ServiceLocator.SetLocatorProvider(() => csl);

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        private Network GetNetwork()
        {
            var networkName = this.Configuration["Network"];

            switch (networkName)
            {
                case "CirrusMain":
                    return FederatedPegNetwork.NetworksSelector.Mainnet();
                case "FederatedPegTest":
                    return FederatedPegNetwork.NetworksSelector.Testnet();
                case "StratisMain":
                    return new StratisMain();
                case "StratisTest":
                    return new StratisTest();
                case "Main":
                    return new BitcoinMain();
                case "TestNet":
                    return new BitcoinTest();
                default:
                    return new StratisMain();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSwagger();
            app.UseMiddleware<ChainCacheCheckMiddleware>();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Azure Indexer API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
