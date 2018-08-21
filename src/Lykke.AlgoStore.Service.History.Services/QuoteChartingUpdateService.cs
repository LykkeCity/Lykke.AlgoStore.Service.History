using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Core.Services;

namespace Lykke.AlgoStore.Service.History.Services
{
    public class QuoteChartingUpdateService : IQuoteChartingUpdateService
    {
        private IQuoteChartingUpdateRepository _quotesRepo;

        public QuoteChartingUpdateService(IQuoteChartingUpdateRepository quotesRepo)
        {
            _quotesRepo = quotesRepo;  
        }

        public async Task<IEnumerable<QuoteChartingUpdate>> GetQuoteChartingUpdateForPeriodAsync(DateTime from, DateTime to, string instanceId, string assetPair, IErrorDictionary errorDictionary, CancellationToken ct, bool? isBuy = null)
        {
            if (!Validate(instanceId, from, to, errorDictionary))
                return null;

            var quotes = await _quotesRepo.GetQuotesForPeriodAsync(instanceId, assetPair, from, to, ct, isBuy);

            var result = AutoMapper.Mapper.Map<IEnumerable<QuoteChartingUpdate>>(quotes);

            return result;
        }

        private bool Validate(string instanceId, DateTime fromMoment, DateTime toMoment, IErrorDictionary errorDictionary)
        {
            if (String.IsNullOrWhiteSpace(instanceId))
            {
                errorDictionary.Add(nameof(instanceId), $"{nameof(instanceId)} Can not be empty.");
            }
            if (fromMoment < new DateTime(1990, 1, 1))
            {
                errorDictionary.Add("fromMoment", $"fromMoment is too far in the past.");
                return false;
            }
            if (toMoment < fromMoment)
            {
                errorDictionary.Add("toMoment", $"toMoment cant be earlier than fromMoment.");
                return false;
            }

            return errorDictionary.IsValid;
        }
    }
}
