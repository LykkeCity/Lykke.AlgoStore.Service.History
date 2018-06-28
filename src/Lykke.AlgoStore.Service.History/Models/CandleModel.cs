using Newtonsoft.Json;
using System;

namespace Lykke.AlgoStore.Service.History.Models
{
    public class CandleModel
    {
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }
        [JsonProperty("open")]
        public double Open { get; set; }
        [JsonProperty("close")]
        public double Close { get; set; }
        [JsonProperty("high")]
        public double High { get; set; }
        [JsonProperty("low")]
        public double Low { get; set; }
    }
}
