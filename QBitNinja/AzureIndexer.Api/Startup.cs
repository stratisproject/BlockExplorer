using System.IO;
using System.Threading;
using AzureIndexer.Api.Controllers;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.IoC;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.OpenApi.Models;
using NBitcoin.Networks;
using Newtonsoft.Json;
using Serilog;
using Stratis.Bitcoin;
using Stratis.Bitcoin.AsyncWork;
using Stratis.Bitcoin.Base;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Configuration.Settings;
using Stratis.Bitcoin.EventBus;
using Stratis.Bitcoin.Networks;
using Stratis.Bitcoin.P2P;
using Stratis.Bitcoin.Signals;
using Stratis.Bitcoin.Utilities;
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
        public Startup(IWebHostEnvironment env)
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

            services.AddRazorPages();

            services
                .AddMvc(options =>
                    {
                        options.Filters.Add<WebApiExceptionActionFilter>();
                        options.Filters.Add<GlobalExceptionFilter>();
                    })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSingleton(new DBreezeSerializer(network.Consensus.ConsensusFactory));
            services.AddSingleton<IHostedService, BuildChainCache>();
            services.AddSingleton<IHostedService, UpdateChainListener>();
            services.AddSingleton<IHostedService, UpdateStatsListener>();
            services.AddCors();
            services.AddLogging(c => c.AddDebug().AddConsole());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Azure Indexer API", Version = "v1" });
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
                    var asyncProvider = ctx.Resolve<IAsyncProvider>();
                    var peerAddressManager = ctx.Resolve<IPeerAddressManager>();
                    var config = new QBitNinjaConfiguration(this.Configuration, loggerFactory, asyncProvider, peerAddressManager);
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
                var chain = new ChainIndexer(config.Indexer.Network);

                return chain;
            }).As<ChainIndexer>().SingleInstance();

            builder.Register(ctx =>
            {
                var loggerFactory = ctx.Resolve<ILoggerFactory>();
                var dateTimeProvider = ctx.Resolve<IDateTimeProvider>();
                var connectionManagerSettings = new ConnectionManagerSettings(NodeSettings.Default(network));
                var selfEndpointTracker = new SelfEndpointTracker(loggerFactory, connectionManagerSettings);

                var peerAddressManager = new PeerAddressManager(dateTimeProvider, new DataFolder("D:\\Home\\site\\wwwroot\\data\\temp"), loggerFactory, selfEndpointTracker);

                return peerAddressManager;
            }).As<IPeerAddressManager>().SingleInstance();

            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>();

            builder.RegisterType<AsyncProvider>().As<IAsyncProvider>();
            builder.Register(ctx =>
            {
                var loggerFactory = ctx.Resolve<ILoggerFactory>();
                var signals = new Signals(loggerFactory, new DefaultSubscriptionErrorHandler(loggerFactory));

                return signals;
            }).As<ISignals>().SingleInstance();
            builder.RegisterType<ChainRepository>().As<IChainRepository>().WithParameter("folder", this.Configuration["LocalChain"]).SingleInstance();
            builder.RegisterType<TransactionSearchService>().As<ITransactionSearchService>();
            builder.RegisterType<NodeLifetime>().As<INodeLifetime>();
            builder.RegisterType<BalanceSearchService>().As<IBalanceSearchService>();
            builder.RegisterType<BlockSearchService>().As<IBlockSearchService>();
            builder.RegisterType<SmartContractSearchService>().As<ISmartContractSearchService>();
            builder.RegisterType<MainController>().AsSelf();
            builder.RegisterType<ChainCacheProvider>().AsSelf();
            builder.RegisterType<WhatIsIt>().AsSelf();
            builder.RegisterInstance(new Stats()).AsSelf();
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
                    return CirrusNetwork.NetworksSelector.Mainnet();
                case "CirrusTest":
                    return CirrusNetwork.NetworksSelector.Testnet();
                case "FederatedPegTest":
                    return CirrusNetwork.NetworksSelector.Regtest();
                case "StraxMain":
                    return new StraxMain();
                case "StraxTest":
                    return new StraxTest();
                case "Main":
                    return new BitcoinMain();
                case "TestNet":
                    return new BitcoinTest();
                default:
                    return new StraxMain();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
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
            //app.UseMvc();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}
