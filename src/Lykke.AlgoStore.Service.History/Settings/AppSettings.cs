using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.AlgoStore.Service.History.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public HistorySettings HistoryService { get; set; }
        public CandlesHistoryServiceClient CandlesHistoryServiceClient { get; set; }
    }
}
