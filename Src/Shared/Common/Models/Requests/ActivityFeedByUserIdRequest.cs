// <copyright file="ActivityFeedByUserIdRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using Newtonsoft.Json;

    public class ActivityFeedByUserIdRequest
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; } = 0;

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; } = 25;
    }
}
