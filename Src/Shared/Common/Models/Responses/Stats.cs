// <copyright file="Stats.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class Stats
    {
        [JsonProperty(PropertyName = "ActivityType")]
        public ActivityType ActivityType { get; set; }

        [JsonProperty(PropertyName = "ActivityTypeString")]
        public string ActivityTypeString { get; set; }

        [JsonProperty(PropertyName = "TotalScore")]
        public long TotalScore { get; set; }

        [JsonProperty(PropertyName = "TotalCount")]
        public long TotalCount { get; set; }
    }
}
