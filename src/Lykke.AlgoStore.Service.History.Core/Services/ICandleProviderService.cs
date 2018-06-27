using Lykke.Service.CandlesHistory.Client.Models;
using Lykke.AlgoStore.Service.History.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Service.History.Core.Services
{
    public interface ICandleProviderService
    {
        Task<IEnumerable<Candle>> GetCandlesForPeriod(
            CandleRequest request,
            AlgoClientInstanceData instanceData,
            IErrorDictionary errorDictionary);
    }
}
