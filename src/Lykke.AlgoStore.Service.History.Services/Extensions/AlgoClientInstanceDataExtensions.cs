using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.History.Core.Domain;
using System;
using System.Linq;

namespace Lykke.AlgoStore.Service.History.Services.Extensions
{
    internal static class AlgoClientInstanceDataExtensions
    {
        public static IndicatorData GetParamsForIndicator(this AlgoClientInstanceData data, string indicator)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrEmpty(indicator))
                throw new ArgumentNullException(nameof(indicator));

            var indicatorData = new IndicatorData();

            var function = data.AlgoMetaDataInformation
                               .Functions
                               .FirstOrDefault(f => f.Id == indicator);

            if(function != null)
            {
                indicatorData.Name = function.Id;
                indicatorData.AssetPair = function.Parameters.FirstOrDefault(f => f.Key == "assetPair")?.Value;

                var startDateVal = function.Parameters.FirstOrDefault(f => f.Key == "startingDate")?.Value;
                var endDateVal = function.Parameters.FirstOrDefault(f => f.Key == "endingDate")?.Value;
                var intervalVal = function.Parameters.FirstOrDefault(f => f.Key == "candleTimeInterval")?.Value;

                if (!string.IsNullOrEmpty(intervalVal))
                    indicatorData.CandleTimeInterval = Enum.Parse<CandleTimeInterval>(intervalVal);
                if (!string.IsNullOrEmpty(startDateVal))
                    indicatorData.StartDate = DateTime.Parse(startDateVal);
                if(!string.IsNullOrEmpty(endDateVal))
                    indicatorData.EndDate = DateTime.Parse(endDateVal);

                return indicatorData;
            }

            if (data.AuthToken != indicator) return null;

            indicatorData.CandleTimeInterval = Enum.Parse<CandleTimeInterval>(
                data.AlgoMetaDataInformation.Parameters.First(f => f.Key == "CandleInterval").Value);
            indicatorData.AssetPair = 
                data.AlgoMetaDataInformation.Parameters.First(f => f.Key == nameof(indicatorData.AssetPair)).Value;
            indicatorData.StartDate =
                DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(f => f.Key == "StartFrom").Value);
            indicatorData.EndDate =
                DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(f => f.Key == "EndOn").Value);

            return indicatorData;
        }
    }
}
