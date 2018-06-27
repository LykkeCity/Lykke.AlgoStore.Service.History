using System;
using Common.Log;

namespace Lykke.AlgoStore.Service.History.Client
{
    public class HistoryClient : IHistoryClient, IDisposable
    {
        private readonly ILog _log;

        public HistoryClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
