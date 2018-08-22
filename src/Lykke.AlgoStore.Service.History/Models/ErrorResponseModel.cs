using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.AlgoStore.Service.History.Models
{
    public class ErrorResponseModel
    {
        [JsonProperty("errors")]
        public IEnumerable<string> Errors { get; set; }

        public static ErrorResponseModel CreateFromModelState(ModelStateDictionary modelState)
        {
            return new ErrorResponseModel
            {
                Errors = modelState.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage)
            };
        }
    }
}
