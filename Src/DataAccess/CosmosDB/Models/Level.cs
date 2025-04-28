// <copyright file="Level.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using Newtonsoft.Json;

    public class Level
    {
        [JsonProperty(PropertyName = "currentLevel")]
        public uint CurrentLevel { get; set; }

        [JsonProperty(PropertyName = "pointsAtNextMilestone")]
        public uint? PointsAtNextMilestone { get; set; }
    }
}
