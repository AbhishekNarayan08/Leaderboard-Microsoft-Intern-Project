// <copyright file="ContentInfo.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using System;
    using Newtonsoft.Json;

    public class ContentInfo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "creationTime")]
        public DateTimeOffset? CreationDateTime { get; set; }

        [JsonProperty(PropertyName = "lastAccessTime")]
        public DateTimeOffset? LastAccessDateTime { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "requestUrl")]
        public string RequestURL { get; set; }
    }
}
