// <copyright file="UserLeaderboard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserLeaderboard
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "leaderboards")]
        public Dictionary<string, float> Leaderboards { get; set; }
    }
}
