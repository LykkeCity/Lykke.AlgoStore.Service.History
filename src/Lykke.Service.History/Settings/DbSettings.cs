using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.History.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
