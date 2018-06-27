using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Service.History.Settings
{
    public class CandlesHistoryServiceClient
    {
        [HttpCheck("api/IsAlive")]
        public string ServiceUrl { get; set; }
    }
}
