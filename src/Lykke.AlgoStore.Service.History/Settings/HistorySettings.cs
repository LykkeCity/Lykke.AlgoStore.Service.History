using JetBrains.Annotations;
using Lykke.AlgoStore.Security.InstanceAuth;

namespace Lykke.AlgoStore.Service.History.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class HistorySettings
    {
        public DbSettings Db { get; set; }
        public InstanceAuthSettings InstanceAuthSettings { get; set; }
        public RateLimitSettings RateLimitSettings { get; set; }
    }
}
