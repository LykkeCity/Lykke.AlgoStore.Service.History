using Autofac;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Service.CandlesHistory.Client;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AlgoStore.Service.History.Services;
using Lykke.AlgoStore.Service.History.Settings;
using Lykke.SettingsReader;
using System;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeAzureTable;
using Lykke.Logs.Loggers.LykkeConsole;

namespace Lykke.AlgoStore.Service.History.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var reloadingDbManager = _appSettings
                .ConnectionString(x => x.AlgoStoreHistoryService.Db.TableStorageConnectionString);

            var tempLog = LogFactory.Create().AddUnbufferedConsole();

            builder.RegisterInstance(AzureTableStorage<AlgoClientInstanceEntity>
                        .Create(reloadingDbManager, AlgoClientInstanceRepository.TableName, tempLog));
            builder.RegisterInstance(AzureTableStorage<AlgoInstanceStoppingEntity>
                        .Create(reloadingDbManager, AlgoClientInstanceRepository.TableName, tempLog));
            builder.RegisterInstance(AzureTableStorage<AlgoInstanceTcBuildEntity>
                        .Create(reloadingDbManager, AlgoClientInstanceRepository.TableName, tempLog));

            builder.RegisterInstance(AzureTableStorage<FunctionChartingUpdateEntity>
                .Create(reloadingDbManager, FunctionChartingUpdateRepository.TableName, tempLog));

            builder.RegisterType<AlgoClientInstanceRepository>().As<IAlgoClientInstanceRepository>();

            builder.RegisterType<FunctionChartingUpdateRepository>().As<IFunctionChartingUpdateRepository>();

            builder.RegisterType<CandleProviderService>().As<ICandleProviderService>();

            builder.RegisterType<FunctionChartingUpdateService>().As<IFunctionChartingUpdateService>();

            builder.RegisterInstance(_appSettings.CurrentValue.AlgoStoreHistoryService.RateLimitSettings)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<Candleshistoryservice>()
                .As<ICandleshistoryservice>()
                .WithParameter(
                    TypedParameter.From(
                        new Uri(_appSettings.CurrentValue.AlgoStoreCandlesHistoryServiceClient.ServiceUrl)));
        }
    }
}
