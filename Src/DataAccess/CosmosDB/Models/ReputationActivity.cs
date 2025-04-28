// <copyright file="ReputationActivity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using Newtonsoft.Json;
    using System;

    public class ReputationActivity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "docType")]
        public string DocType { get; set; }

        [JsonProperty(PropertyName = "pointValue")]
        public long PointValue { get; set; }

        [JsonProperty(PropertyName = "activityType")]
        public string ActivityType { get; set; }

        [JsonProperty(PropertyName = "segment")]
        public string Segment { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTimeOffset TimeStamp { get; set; }

        [JsonProperty(PropertyName = "content")]
        public ContentMetaData Content { get; set; }
    }
}
