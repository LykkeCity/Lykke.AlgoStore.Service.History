namespace Lykke.AlgoStore.Service.History.Client.Models
{
    /// <summary>
    /// The period length each candle should cover
    /// </summary>
    public enum CandleTimeInterval
    {
        /// <summary>
        /// One second
        /// </summary>
        Sec = 1,
        /// <summary>
        /// One minute
        /// </summary>
        Minute = 60,
        /// <summary>
        /// Five minutes
        /// </summary>
        Min5 = 300,
        /// <summary>
        /// Fifteen minutes
        /// </summary>
        Min15 = 900,
        /// <summary>
        /// Thirty minutes
        /// </summary>
        Min30 = 1800,
        /// <summary>
        /// One hour
        /// </summary>
        Hour = 3600,
        /// <summary>
        /// Four hours
        /// </summary>
        Hour4 = 14400,
        /// <summary>
        /// Six hours
        /// </summary>
        Hour6 = 21600,
        /// <summary>
        /// Twelve hours
        /// </summary>
        Hour12 = 43200,
        /// <summary>
        /// One day
        /// </summary>
        Day = 86400,
        /// <summary>
        /// One week
        /// </summary>
        Week = 604800,
        /// <summary>
        /// One month
        /// </summary>
        Month = 3000000,
    }
}
