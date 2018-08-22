using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.Service.CandlesHistory.Client.Models;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Models;

namespace Lykke.AlgoStore.Service.History.MapperProfiles
{
    internal class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<HistoryRequestModel, CandleRequest>();
            CreateMap<Candle, CandleModel>();
            CreateMap<FunctionChartingUpdateData, FunctionChartingUpdate>();
            CreateMap<QuoteChartingUpdateData, QuoteChartingUpdate>();
        }
    }
}
