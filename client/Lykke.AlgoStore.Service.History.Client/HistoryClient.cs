﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Service.History.Client.AutorestClient;
using Lykke.AlgoStore.Service.History.Client.AutorestClient.Models;
using Lykke.AlgoStore.Service.History.Client.Models;
using Microsoft.Rest;

namespace Lykke.AlgoStore.Service.History.Client
{
    internal sealed class HistoryClient : IHistoryClient, IDisposable
    {
        private readonly ILog _log;
        private readonly IHistoryAPI _historyApi;

        public HistoryClient(string serviceUrl, ILog log)
        {
            _log = log;
            _historyApi = new HistoryAPI(new Uri(serviceUrl));
        }

        public async Task<IEnumerable<Candle>> GetCandles(
            DateTime from, 
            DateTime to,
            string assetPair,
            Models.CandleTimeInterval timeInterval,
            string indicatorName, 
            string authToken)
        {
            if (string.IsNullOrEmpty(indicatorName))
                throw new ArgumentNullException(nameof(indicatorName));
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            using (var operationResponse = await _historyApi
                .GetCandlesWithHttpMessagesAsync(from, to, assetPair,
                    timeInterval.ToString().ParseCandleTimeInterval().Value, indicatorName, GetHeaders(authToken)))
            {
                if (operationResponse.Response.StatusCode == System.Net.HttpStatusCode.OK)
                    return ((IList<CandleModel>)operationResponse.Body).Select(c => c.ToCandle());

                if (operationResponse.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errors = ((ErrorResponseModel)operationResponse.Body);
                    var exceptions = errors.Errors.Select(e => new ArgumentException(e));
                    throw new AggregateException("There were validation errors. See inner exceptions for details.",
                                                 exceptions);
                }

                var message = (int)operationResponse.Response.StatusCode == 429 ?
                                    "Rate limited" :
                                    "Service unavailable";

                throw new HttpOperationException(message)
                {
                    Response = new HttpResponseMessageWrapper(operationResponse.Response, "")
                };
            }
        }

        public async Task<IEnumerable<FunctionChartingUpdate>> GetFunctionValues(string instanceId, DateTime @from, DateTime to, string authToken)
        {
            if (string.IsNullOrEmpty(instanceId))
                throw new ArgumentNullException(nameof(instanceId));
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            using (var operationResponse = await _historyApi.GetFunctionForPeriodWithHttpMessagesAsync(from, to, instanceId, GetHeaders(authToken)))
            {
                if (operationResponse.Response.StatusCode == System.Net.HttpStatusCode.OK)
                    return ((IList<FunctionChartingUpdate>)operationResponse.Body).Select(c => c);

                if (operationResponse.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errors = ((ErrorResponseModel)operationResponse.Body);
                    var errorList = String.Join(",", errors.Errors);
                    throw new HttpOperationException()
                    {
                        Response = new HttpResponseMessageWrapper(operationResponse.Response, errorList)
                    };
                }

                var message = (int)operationResponse.Response.StatusCode == 429 ?
                    "Rate limited" :
                    "Service unavailable";

                throw new HttpOperationException(message)
                {
                    Response = new HttpResponseMessageWrapper(operationResponse.Response, "")
                };
            }
        }

        public async Task<IEnumerable<QuoteChartingUpdate>> GetQuotes(DateTime from, DateTime to, string assetPair, string instanceId, string authToken, bool? isBuy = null)
        {
            if (string.IsNullOrEmpty(instanceId))
                throw new ArgumentNullException(nameof(instanceId));
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            using (var operationResponse = await _historyApi.GetQuotesForPeriodWithHttpMessagesAsync(from, to, instanceId, assetPair, isBuy, GetHeaders(authToken)))
            {
                if (operationResponse.Response.StatusCode == System.Net.HttpStatusCode.OK)
                    return ((IList<QuoteChartingUpdate>)operationResponse.Body).Select(c => c);

                if (operationResponse.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errors = (ErrorResponseModel)operationResponse.Body;
                    var errorList = String.Join(",", errors.Errors);
                    throw new HttpOperationException()
                    {
                        Response = new HttpResponseMessageWrapper(operationResponse.Response, errorList)
                    };
                }

                var message = (int)operationResponse.Response.StatusCode == 429 ?
                    "Rate limited" :
                    "Service unavailable";

                throw new HttpOperationException(message)
                {
                    Response = new HttpResponseMessageWrapper(operationResponse.Response, "")
                };
            }
        }

        public void Dispose()
        {
            _historyApi?.Dispose();
        }

        private Dictionary<string, List<string>> GetHeaders(string authToken)
        {
            return new Dictionary<string, List<string>>
            {
                ["Authorization"] = new List<string> { $"Bearer {authToken}" }
            };
        }

        
    }
}
