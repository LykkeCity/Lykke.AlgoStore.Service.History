namespace Lykke.AlgoStore.Service.History.Core.Domain
{
    public interface IErrorDictionary
    {
        bool IsValid { get; }

        void Add(string key, string error);
    }
}
