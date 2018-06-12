using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.History.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class HistorySettings
    {
        public DbSettings Db { get; set; }
    }
}
