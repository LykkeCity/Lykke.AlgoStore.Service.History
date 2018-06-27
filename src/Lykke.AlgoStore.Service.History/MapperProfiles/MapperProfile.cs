using AutoMapper;
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
        }
    }
}
