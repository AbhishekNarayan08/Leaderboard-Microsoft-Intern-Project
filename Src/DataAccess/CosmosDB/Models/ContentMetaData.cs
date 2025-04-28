// <copyright file="ContentMetaData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ContentMetaData
    {
        [JsonProperty(PropertyName = "contentId")]
        public string ContentId { get; set; }

        [JsonProperty(PropertyName = "creationDateTime")]
        public DateTimeOffset CreationDateTime { get; set; }

        [JsonProperty(PropertyName = "lastAccessDateTime")]
        public DateTimeOffset LastAccessDateTime { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "requestUrl")]
        public string RequeryURL { get; set; }
    }
}
