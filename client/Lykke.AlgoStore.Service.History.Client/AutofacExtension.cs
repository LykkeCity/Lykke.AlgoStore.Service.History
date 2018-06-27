using System;
using Autofac;
using Common.Log;

namespace Lykke.AlgoStore.Service.History.Client
{
    public static class AutofacExtension
    {
        public static void RegisterHistoryClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<HistoryClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<IHistoryClient>()
                .SingleInstance();
        }

        public static void RegisterHistoryClient(this ContainerBuilder builder, HistoryServiceClientSettings settings, ILog log)
        {
            builder.RegisterHistoryClient(settings?.ServiceUrl, log);
        }
    }
}
