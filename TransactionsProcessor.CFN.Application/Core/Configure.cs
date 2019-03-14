using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Services.Downloader;
using TransactionsProcessor.Data;
using TransactionsProcessor.FileManager;
using TransactionsProcessor.FileManager.Core;

namespace TransactionsProcessor.CFN.Application.Core
{
    public static class Configure
    {
        private static IConfigurationRoot Configuration { get; set; }

        public static void AddCFN(this IServiceCollection services)
        {
            ConfigureConnectionStrings(services);
            ConfigureNuggetPackages(services);
            ConfigureOutboundHttp(services);
        }

        private static void ConfigureConnectionStrings(IServiceCollection services)
        {
            services.Configure<CfnSettings>(Configuration.GetSection(nameof(CfnSettings)));
            CfnSettings cfnSettings = services.BuildServiceProvider().GetService<IOptions<CfnSettings>>().Value;

            services.Configure<AfDatabase>(Configuration.GetSection(nameof(AfDatabase)));
            AfSettings afSettings = services.BuildServiceProvider().GetService<IOptions<AfSettings>>().Value;
        }

        private static void ConfigureNuggetPackages(IServiceCollection services)
        {
            services.AddAutoMapper();
        }

        private static void ConfigureOutboundHttp(IServiceCollection services)
        {
            services.AddHttpClient("Billing", client =>
            {
                client.BaseAddress = new Uri("http://localhost/billing/api/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60) }))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 3,
                        durationOfBreak: TimeSpan.FromSeconds(70)
                    ));

            services.AddHttpClient("QCClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost/qc-client-api/api/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })

                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(100) }))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 3,
                        durationOfBreak: TimeSpan.FromSeconds(70)
                    ));
        }

        private static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IDatabase, Database>();
            services.AddTransient<ICfnDatabase, CfnDatabase>();
            services.AddTransient<IAfDatabase, AfDatabase>();
            services.AddTransient<IFileManagerDatabase, FileManagerDatabase>();

            services.AddTransient<IFileManagerFileConfigurations, FileManagerFileConfigurations>();
            services.AddTransient<IFileManagerFolderPaths, FileManagerFolderPaths>();
        }
    }
}
