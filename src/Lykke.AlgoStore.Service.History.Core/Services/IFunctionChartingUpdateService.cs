using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.History.Core.Domain;

namespace Lykke.AlgoStore.Service.History.Core.Services
{
    public interface IFunctionChartingUpdateService
    {
        Task<IEnumerable<FunctionChartingUpdate>> GetFunctionChartingUpdateForPeriodAsync(string instanceId, DateTime from, DateTime to, IErrorDictionary errorDictionary, CancellationToken ct);
    }
}
