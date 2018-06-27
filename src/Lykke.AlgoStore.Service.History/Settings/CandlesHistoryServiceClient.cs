using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Service.History.ServiceSettings
{
    public class CandlesHistoryServiceClient
    {
        [HttpCheck("api/IsAlive")]
        public string ServiceUrl { get; set; }
    }
}
