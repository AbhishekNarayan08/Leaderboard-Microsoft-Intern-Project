// <copyright file="ActivityStats.cs" company="Microsoft">
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

    public class ActivityStats
    {
        [JsonProperty(PropertyName = "activityType")]
        public int ActivityType { get; set; }

        [JsonProperty(PropertyName = "pointValue")]
        public long PointValue { get; set; } = 0;

        [JsonProperty(PropertyName = "countValue")]
        public long CountValue { get; set; } = 0;
    }
}
