using Lykke.AlgoStore.Service.History.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.History.Client
{
    /// <summary>
    /// Interface for fetching historical candle data
    /// </summary>
    public interface IHistoryClient
    {
        /// <summary>
        /// Get historical candles for a given period
        /// </summary>
        /// <param name="from">The start date for the candle period (inclusive)</param>
        /// <param name="to">The end date for the candle period (exclusive)</param>
        /// <param name="assetPair">The asset pair of the indicator</param>
        /// <param name="timeInterval">The candle time interval of the indicator</param>
        /// <param name="indicatorName">The name of the indicator to fetch candles for</param>
        /// <param name="authToken">The instance authentication token</param>
        /// <returns>A list of candles for the given period</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="indicatorName"/>, <paramref name="assetPair"/> 
        /// or <paramref name="authToken"/> are null
        /// </exception>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when an unexpected error has occured. See exception message for details
        /// </exception>
        /// <exception cref="AggregateException">
        /// Thrown when there are validation errors. Each validation message is contained within
        /// a <see cref="ArgumentException"/>
        /// </exception>
        Task<IEnumerable<Candle>> GetCandles(
            DateTime from, 
            DateTime to,
            string assetPair,
            CandleTimeInterval timeInterval,
            string indicatorName, 
            string authToken);
    }
}
