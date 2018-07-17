using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.CandlesHistory.Client.Models;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AlgoStore.Service.History.Services.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Service.History.Services
{
    public class CandleProviderService : ICandleProviderService
    {
        private static readonly Dictionary<CandleTimeInterval, TimeSpan> _maxAllowedPeriods =
            new Dictionary<CandleTimeInterval, TimeSpan>
            {
                [CandleTimeInterval.Sec]    = TimeSpan.FromHours(2),
                [CandleTimeInterval.Minute] = TimeSpan.FromDays(6),
                [CandleTimeInterval.Min5]   = TimeSpan.FromDays(34),
                [CandleTimeInterval.Min15]  = TimeSpan.FromDays(104),
                [CandleTimeInterval.Min30]  = TimeSpan.FromDays(208),
                [CandleTimeInterval.Hour]   = TimeSpan.FromDays(416),
                [CandleTimeInterval.Hour4]  = TimeSpan.FromDays(1666),
                [CandleTimeInterval.Hour6]  = TimeSpan.FromDays(2499),
                [CandleTimeInterval.Hour12] = TimeSpan.FromDays(4999)
            };

        private static readonly Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval, CandleTimeInterval>
            _candleIntervalMap = new Dictionary<CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval, CandleTimeInterval>
            {
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Sec]     = CandleTimeInterval.Sec,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Minute]  = CandleTimeInterval.Minute,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Min5]    = CandleTimeInterval.Min5,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Min15]   = CandleTimeInterval.Min15,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Min30]   = CandleTimeInterval.Min30,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour]    = CandleTimeInterval.Hour,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour4]   = CandleTimeInterval.Hour4,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour6]   = CandleTimeInterval.Hour6,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Hour12]  = CandleTimeInterval.Hour12,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Day]     = CandleTimeInterval.Day,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Week]    = CandleTimeInterval.Week,
                [CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval.Month]   = CandleTimeInterval.Month,
            };

        private readonly ICandleshistoryservice _candlesHistoryService;

        public CandleProviderService(ICandleshistoryservice candlesHistoryService)
        {
            _candlesHistoryService = candlesHistoryService ?? 
                throw new ArgumentNullException(nameof(candlesHistoryService));
        }

        private bool Validate(
            CandleRequest request, 
            FunctionParamsBase functionParams, 
            IErrorDictionary errorDictionary)
        {
            if (request.StartFrom >= request.EndOn)
                errorDictionary.Add(nameof(request.StartFrom), 
                    $"{nameof(request.StartFrom)} must be before {nameof(request.EndOn)}");

            if (functionParams == null)
            {
                errorDictionary.Add(nameof(request.IndicatorName), "Unknown indicator name");
                return false;
            }

            if (functionParams.StartingDate > request.StartFrom)
                errorDictionary.Add(nameof(request.StartFrom),
                    $"{nameof(request.StartFrom)} must be after indicator start date");

            if (functionParams.EndingDate < request.StartFrom)
                errorDictionary.Add(nameof(request.StartFrom),
                    $"{nameof(request.StartFrom)} must be before indicator end date");

            if (functionParams.StartingDate > request.EndOn)
                errorDictionary.Add(nameof(request.StartFrom),
                    $"{nameof(request.EndOn)} must be after indicator start date");

            if (functionParams.EndingDate < request.EndOn)
                errorDictionary.Add(nameof(request.StartFrom),
                    $"{nameof(request.EndOn)} must be before indicator end date");

            var cti = _candleIntervalMap[functionParams.CandleTimeInterval];

            if (_maxAllowedPeriods.ContainsKey(cti) &&
                _maxAllowedPeriods[cti] < request.EndOn - request.StartFrom)
                errorDictionary.Add("Period", "The request period is too large");

            return errorDictionary.IsValid;
        }

        public async Task<IEnumerable<Candle>> GetCandlesForPeriod(
            CandleRequest request,
            AlgoClientInstanceData instanceData,
            IErrorDictionary errorDictionary)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (instanceData == null)
                throw new ArgumentNullException(nameof(instanceData));
            if (errorDictionary == null)
                throw new ArgumentNullException(nameof(errorDictionary));

            var functionParams = instanceData.GetParamsForIndicator(request.IndicatorName);

            if (!Validate(request, functionParams, errorDictionary))
                return null;

            var responseTask = _candlesHistoryService.GetCandlesHistoryOrErrorWithHttpMessagesAsync(
                functionParams.AssetPair, CandlePriceType.Mid, _candleIntervalMap[functionParams.CandleTimeInterval],
                request.StartFrom, request.EndOn);
            var timeOutTask = Task.Delay(10_000);

            var resultTask = await Task.WhenAny(responseTask, timeOutTask);

            if (resultTask == timeOutTask || !responseTask.IsCompletedSuccessfully)
                throw new TaskCanceledException(responseTask);

            using (var response = responseTask.Result)
            {
                return ((CandlesHistoryResponseModel)response.Body).History;
            }
        }
    }
}
