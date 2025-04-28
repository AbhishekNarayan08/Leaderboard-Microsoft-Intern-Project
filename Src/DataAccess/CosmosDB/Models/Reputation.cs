// <copyright file="Reputation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Reputation
    {
        [JsonProperty(PropertyName = "pointValue")]
        public long PointValue { get; set; }

        [JsonProperty(PropertyName = "segment")]
        public string Segment { get; set; }

        [JsonProperty(PropertyName = "lastUpdated")]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty(PropertyName = "badges")]
        public List<Badge> Badges { get; set; }

        [JsonProperty(PropertyName = "level")]
        public Level Level { get; set; }

        [JsonProperty(PropertyName = "activityStats")]
        public IDictionary<string, ActivityStats> ActivityStats { get; set; }
    }
}
