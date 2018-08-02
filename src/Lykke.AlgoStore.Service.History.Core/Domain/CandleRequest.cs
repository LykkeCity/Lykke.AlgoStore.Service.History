using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.Service.History.Core.Domain
{
    public class CandleRequest
    {
        public DateTime StartFrom { get; set; }
        public DateTime EndOn { get; set; }
        public string AssetPair { get; set; }
        public CandleTimeInterval CandleTimeInterval { get; set; }
        public string IndicatorName { get; set; }
    }
}
