using System;
using Autofac;
using Common.Log;

namespace Lykke.AlgoStore.Service.History.Client
{
    /// <summary>
    /// Used to register the history client in the AutoFac container
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers the history client using a given service URL and log
        /// </summary>
        /// <param name="builder">The container to register the history client in</param>
        /// <param name="serviceUrl">The history client URL</param>
        /// <param name="log">The logger to use</param>
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

        /// <summary>
        /// Registers the history client using a given settings class and log
        /// </summary>
        /// <param name="builder">The container to register the history client in</param>
        /// <param name="settings">The settings to use for configuring the client</param>
        /// <param name="log">The logger to use</param>
        public static void RegisterHistoryClient(this ContainerBuilder builder, HistoryServiceClientSettings settings, ILog log)
        {
            builder.RegisterHistoryClient(settings?.ServiceUrl, log);
        }
    }
}
