// <copyright file="ContentMetadata.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System;
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class ContentMetadata
    {
        [JsonProperty(PropertyName = "ContentId")]
        public string ContentId { get; set; }

        [JsonProperty(PropertyName = "CreationDateTime")]
        public DateTimeOffset CreationDateTime { get; set; }

        [JsonProperty(PropertyName = "LastAccessDateTime")]
        public DateTimeOffset LastAccessDateTime { get; set; }

        [JsonProperty(PropertyName = "ContentType")]
        public ContentType ContentType { get; set; }

        [JsonProperty(PropertyName = "ContentTypeString")]
        public string ContentTypeString { get; set; }

        [JsonProperty(PropertyName = "RequestUrl")]
        public string RequeryURL { get; set; }
    }
}
