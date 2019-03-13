using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;

namespace TransactionsProcessor.CFN.Application.App_Start
{
    public static class Configure
    {
        public static void Services(IServiceCollection services)
        {
            ConfigureNuggetPackages(services);
            ConfigureOutboundHttp(services);
        }

        private static void ConfigureNuggetPackages(IServiceCollection services)
        {
            services.AddAutoMapper();
        }

        private static void ConfigureOutboundHttp(IServiceCollection services)
        {
            services.AddHttpClient("Billing", client =>
            {
                client.BaseAddress = new Uri("http://localhost/billing/api");
            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60) }))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 3,
                        durationOfBreak: TimeSpan.FromSeconds(70)
                    ));

            services.AddHttpClient("QCClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost/qc-client-api/api");
            })

                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(100) }))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 3,
                        durationOfBreak: TimeSpan.FromSeconds(70)
                    ));
        }
    }
}
