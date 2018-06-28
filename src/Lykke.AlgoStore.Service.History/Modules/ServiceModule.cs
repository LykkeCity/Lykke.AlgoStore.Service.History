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
using Lykke.Logs;
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
            var reloadingDbManager = _appSettings.ConnectionString(x => x.HistoryService.Db.TableStorageConnectionString);

            var tempLog = LogFactory.Create().AddUnbufferedConsole();

            builder.RegisterInstance(AzureTableStorage<AlgoClientInstanceEntity>
                        .Create(reloadingDbManager, AlgoClientInstanceRepository.TableName, tempLog));
            builder.RegisterInstance(AzureTableStorage<AlgoInstanceStoppingEntity>
                        .Create(reloadingDbManager, AlgoClientInstanceRepository.TableName, tempLog));
            builder.RegisterType<AlgoClientInstanceRepository>().As<IAlgoClientInstanceRepository>();

            builder.RegisterType<CandleProviderService>().As<ICandleProviderService>();

            builder.RegisterInstance(_appSettings.CurrentValue.HistoryService.RateLimitSettings)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<Candleshistoryservice>()
                .As<ICandleshistoryservice>()
                .WithParameter(TypedParameter.From(new Uri(_appSettings.CurrentValue.CandlesHistoryServiceClient.ServiceUrl)));
        }
    }
}
