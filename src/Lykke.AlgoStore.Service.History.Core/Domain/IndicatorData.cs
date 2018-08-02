using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.Service.History.Core.Domain
{
    public class IndicatorData
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public string AssetPair { get; set; }
        public CandleTimeInterval CandleTimeInterval { get; set; } = CandleTimeInterval.Unspecified;
    }
}
