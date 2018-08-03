using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Service.History.Services
{
    public class FunctionChartingUpdateService : IFunctionChartingUpdateService
    {
        private IFunctionChartingUpdateRepository _repo;

        public FunctionChartingUpdateService(IFunctionChartingUpdateRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<FunctionChartingUpdate>> GetFunctionChartingUpdateForPeriodAsync(string instanceId, DateTime fromMoment, DateTime toMoment, IErrorDictionary errorDictionary)
        {
            if (!Validate(instanceId, fromMoment, toMoment, errorDictionary))
                return null;

            var functions = await _repo.GetFunctionChartingUpdateForPeriodAsync(instanceId, fromMoment, toMoment);

            var result = functions.Select(AutoMapper.Mapper.Map<FunctionChartingUpdate>);

            return result;
        }

        private bool Validate(string instanceId,DateTime fromMoment, DateTime toMoment, IErrorDictionary errorDictionary)
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
