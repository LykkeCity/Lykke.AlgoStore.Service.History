using System;
using Lykke.Sdk;
using Lykke.Service.History.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.History
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "History API";
                options.Logs = ("HistoryLog", ctx => ctx.HistoryService.Db.LogsConnString);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();

        }
    }
}
