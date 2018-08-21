using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.History.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.History.Core.Services
{
    public interface IQuoteChartingUpdateService
    {
        Task<IEnumerable<QuoteChartingUpdate>> GetQuoteChartingUpdateForPeriodAsync(DateTime from, DateTime to, string instanceId, string assetPair,
            IErrorDictionary errorDictionary, CancellationToken ct, bool? isBuy = null);
    }
}
