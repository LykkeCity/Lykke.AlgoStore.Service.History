using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.Linq;

namespace Lykke.AlgoStore.Service.History.Services.Extensions
{
    internal static class AlgoClientInstanceDataExtensions
    {
        public static FunctionParamsBase GetParamsForIndicator(this AlgoClientInstanceData data, string indicator)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrEmpty(indicator))
                throw new ArgumentNullException(nameof(indicator));

            var fParams = new FunctionParamsBase();

            var function = data.AlgoMetaDataInformation
                               .Functions
                               .FirstOrDefault(f => f.Parameters
                                                     .Any(p => p.Key == nameof(fParams.FunctionInstanceIdentifier) 
                                                                && p.Value == indicator));

            if(function != null)
            {
                fParams.CandleTimeInterval = Enum.Parse<CandleTimeInterval>(
                    function.Parameters.First(f => f.Key == nameof(fParams.CandleTimeInterval)).Value);
                fParams.AssetPair = function.Parameters.First(f => f.Key == nameof(fParams.AssetPair)).Value;
                fParams.StartingDate = 
                    DateTime.Parse(function.Parameters.First(f => f.Key == nameof(fParams.StartingDate)).Value);
                fParams.EndingDate = 
                    DateTime.Parse(function.Parameters.First(f => f.Key == nameof(fParams.EndingDate)).Value);

                return fParams;
            }

            if (data.AuthToken != indicator) return null;

            fParams.CandleTimeInterval = Enum.Parse<CandleTimeInterval>(
                data.AlgoMetaDataInformation.Parameters.First(f => f.Key == "CandleInterval").Value);
            fParams.AssetPair = 
                data.AlgoMetaDataInformation.Parameters.First(f => f.Key == nameof(fParams.AssetPair)).Value;
            fParams.StartingDate =
                DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(f => f.Key == "StartFrom").Value);
            fParams.EndingDate =
                DateTime.Parse(data.AlgoMetaDataInformation.Parameters.First(f => f.Key == "EndOn").Value);

            return fParams;
        }
    }
}
