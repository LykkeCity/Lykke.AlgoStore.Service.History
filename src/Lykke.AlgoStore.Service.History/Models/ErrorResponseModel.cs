using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Service.History.Models
{
    public class ErrorResponseModel
    {
        [JsonProperty("errors")]
        public IEnumerable<string> Errors { get; set; }
    }
}
