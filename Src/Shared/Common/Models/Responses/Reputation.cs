// <copyright file="Reputation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System.Collections.Generic;
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class Reputation
    {
        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "TotalScore")]
        public long TotalScore { get; set; }

        [JsonProperty(PropertyName = "Badges")]
        public List<Badge> Badges { get; set; }

        [JsonProperty(PropertyName = "Level")]
        public Level Level { get; set; }

        [JsonProperty(PropertyName = "Stats")]
        public List<Stats> Stats { get; set; }

        [JsonProperty(PropertyName = "ActivityFeed")]
        public List<Activity> ActivityFeed { get; set; }

        [JsonProperty(PropertyName = "SegmentType")]
        public SegmentType SegmentType { get; set; }

        [JsonProperty(PropertyName = "SegmentTypeString")]
        public string SegmentTypeString { get; set; }
    }
}
