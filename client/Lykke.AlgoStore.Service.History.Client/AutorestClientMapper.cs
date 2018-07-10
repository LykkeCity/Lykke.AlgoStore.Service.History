using Lykke.AlgoStore.Service.History.Client.AutorestClient.Models;
using Lykke.AlgoStore.Service.History.Client.Models;

namespace Lykke.AlgoStore.Service.History.Client
{
    internal static class AutorestClientMapper
    {
        public static Candle ToCandle(this CandleModel candleModel)
        {
            return new Candle
            {
                Open = candleModel.Open,
                Close = candleModel.Close,
                Low = candleModel.Low,
                High = candleModel.High,
                DateTime = candleModel.DateTime
            };
        }
    }
}
