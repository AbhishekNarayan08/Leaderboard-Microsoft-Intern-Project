// <copyright file="BulkWriteResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using Newtonsoft.Json;

    public class BulkWriteResponse
    {
        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}