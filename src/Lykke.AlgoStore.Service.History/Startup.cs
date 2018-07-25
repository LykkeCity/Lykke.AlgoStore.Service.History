using System;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.Sdk;
using Lykke.AlgoStore.Service.History.MapperProfiles;
using Lykke.AlgoStore.Service.History.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lykke.AlgoStore.Service.History.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Lykke.SettingsReader;
using Lykke.AlgoStore.Service.History.Modules;

namespace Lykke.AlgoStore.Service.History
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            InitMapper();
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
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = "History API",
                    ApiVersion = "v1"
                };
                options.Extend = (collection, appSettings) =>
                {
                    collection.AddInstanceAuthentication(appSettings.Nested(x => x.AlgoStoreHistoryService.InstanceAuthSettings)
                                                          .CurrentValue);
                };
                options.Logs = logs =>
                {
                    logs.AzureTableName = "HistoryLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.AlgoStoreHistoryService.Db.LogsConnString;
                };
                options.Swagger = swagger => swagger.OperationFilter<ApiKeyHeaderOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseLykkeConfiguration();
        }
    }
}
