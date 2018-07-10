using Lykke.AlgoStore.Service.History.Core.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Lykke.AlgoStore.Service.History.Utils
{
    internal class ModelStateWrapper : IErrorDictionary
    {
        private readonly ModelStateDictionary _modelState;

        public bool IsValid => _modelState.IsValid;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState ?? throw new ArgumentNullException(nameof(modelState));
        }

        public void Add(string key, string error)
        {
            _modelState.AddModelError(key, error);
        }
    }
}
