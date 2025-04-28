// <copyright file="ActivityFeedByUserIdAndSegmentRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class ActivityFeedByUserIdAndSegmentRequest
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "segmentType")]
        public SegmentType SegmentType { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; } = 0;

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; } = 25;
    }
}
