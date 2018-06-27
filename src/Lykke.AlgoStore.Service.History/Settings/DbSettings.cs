using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Service.History.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
        [AzureTableCheck]
        public string TableStorageConnectionString { get; set; }
    }
}
