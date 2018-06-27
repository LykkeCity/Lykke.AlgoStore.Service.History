using System;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Sdk;
using Lykke.AlgoStore.Service.History.MapperProfiles;
using Lykke.AlgoStore.Service.History.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.AlgoStore.Service.History.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Service.History
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            InitMapper();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static void InitMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
                cfg.AddProfile<MapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.LoadSettings<AppSettings>();

            services.AddInstanceAuthentication(appSettings.Nested(x => x.HistoryService.InstanceCacheSettings)
                                                          .CurrentValue);

            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "History API";
                options.Logs = logs =>
                {
                    logs.AzureTableName = "HistoryLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.HistoryService.Db.LogsConnString;

                };
                options.Swagger = swagger => swagger.OperationFilter<ApiKeyHeaderOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeForwardedHeaders();

            app.UseAuthentication();

            app.UseLykkeConfiguration();
        }
    }
}
