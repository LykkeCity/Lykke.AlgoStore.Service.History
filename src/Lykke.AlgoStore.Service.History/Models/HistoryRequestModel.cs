using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Service.History.Models
{
    public class HistoryRequestModel
    {
        /// <summary>
        /// The start of the period to get candles for (inclusive)
        /// </summary>
        [Required]
        public DateTime StartFrom { get; set; }

        /// <summary>
        /// The end of the period to get candles for (exclusive)
        /// </summary>
        [Required]
        public DateTime EndOn { get; set; }

        /// <summary>
        /// The asset pair to get candles for
        /// </summary>
        [Required]
        public string AssetPair { get; set; }

        /// <summary>
        /// The candle time interval to get candles for
        /// </summary>
        [Required]
        public CandleTimeInterval CandleTimeInterval { get; set; }

        /// <summary>
        /// The name of the indicator to fetch candles for
        /// </summary>
        [Required]
        public string IndicatorName { get; set; }
    }
}
