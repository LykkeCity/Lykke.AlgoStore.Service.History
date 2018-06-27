﻿// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.Service.History.Client.AutorestClient.Models
{
    using Newtonsoft.Json;

    internal partial class CandleModel
    {
        /// <summary>
        /// Initializes a new instance of the CandleModel class.
        /// </summary>
        public CandleModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CandleModel class.
        /// </summary>
        public CandleModel(System.DateTime dateTime, double open, double close, double high, double low)
        {
            DateTime = dateTime;
            Open = open;
            Close = close;
            High = high;
            Low = low;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dateTime")]
        public System.DateTime DateTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "open")]
        public double Open { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "close")]
        public double Close { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "high")]
        public double High { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "low")]
        public double Low { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
