using System;

namespace Lykke.AlgoStore.Service.History.Client.Models
{
    /// <summary>
    /// Represents a candle
    /// </summary>
    public class Candle
    {
        /// <summary>
        /// The date of the candle
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The open price of the candle
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// The close price of the candle
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// The highest price of the candle
        /// </summary>
        public double High { get; set; }
        
        /// <summary>
        /// The lowest price of the candle
        /// </summary>
        public double Low { get; set; }
    }
}
