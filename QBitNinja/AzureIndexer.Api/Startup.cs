using System.IO;
using System.Threading;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.JsonConverters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBitcoin.Networks;
using Newtonsoft.Json;
using Stratis.Bitcoin.Networks;
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
            NetworkRegistration.Register(new StratisTest());

            services
                .AddMvc(options =>
                    {
                        options.Filters.Add<WebApiExceptionActionFilter>();
                        options.Filters.Add<GlobalExceptionFilter>();
                    })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new TransactionJsonConverter();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IHostedService, UpdateChainListener>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Azure Indexer API", Version = "v1" });
                c.DocInclusionPredicate((value, description) => description.ActionDescriptor.DisplayName.Contains("AzureIndexer.Api"));
            });

            // Create the container builder.
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.Register(
                ctx =>
                {
                    var loggerFactory = ctx.Resolve<ILoggerFactory>();
                    var config = new QBitNinjaConfiguration(this.Configuration, loggerFactory);
                    config.Indexer.EnsureSetup();
                    //config.EnsureSetup();
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
                var client = ctx.Resolve<IndexerClient>();
                var chain = new ConcurrentChain(config.Indexer.Network);
                LoadCache(chain, config.LocalChain);
                var changes = client.GetChainChangesUntilFork(chain.Tip, false);
                try
                {
                    changes.UpdateChain(chain);
                }
                catch (ArgumentException) // Happen when chain in table is corrupted
                {
                    client.Configuration.GetChainTable().DeleteIfExistsAsync().GetAwaiter().GetResult();
                    for (var i = 0; i < 20; i++)
                    {
                        try
                        {
                            if (client.Configuration.GetChainTable().CreateIfNotExistsAsync().GetAwaiter().GetResult())
                            {
                                break;
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        Thread.Sleep(10000);
                    }
                    client.Configuration.CreateIndexer().IndexChain(chain);
                }

                SaveChainCache(chain, config.LocalChain);
                return chain;
            }).As<ConcurrentChain>().SingleInstance();

            this.ApplicationContainer = builder.Build();

            var csl = new AutofacServiceLocator(this.ApplicationContainer);
            ServiceLocator.SetLocatorProvider(() => csl);

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSwagger();

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

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private static void LoadCache(ConcurrentChain chain, string cacheLocation)
        {
            if (string.IsNullOrEmpty(cacheLocation))
            {
                return;
            }

            try
            {
                var bytes = File.ReadAllBytes(cacheLocation);
                chain.Load(bytes);
            }
            catch
            {
                // We don't care if it don't succeed
            }
        }

        private static void SaveChainCache(ConcurrentChain chain, string cacheLocation)
        {
            if (string.IsNullOrEmpty(cacheLocation))
            {
                return;
            }

            try
            {
                var file = new FileInfo(cacheLocation);
                if (!file.Exists || (DateTime.UtcNow - file.LastWriteTimeUtc) > TimeSpan.FromDays(1))
                {
                    using (var fs = File.Open(cacheLocation, FileMode.Create))
                    {
                        chain.WriteTo(fs);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
