// <copyright file="ReputationByUserIdAndSegmentRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class ReputationByUserIdAndSegmentRequest
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "segmentType")]
        public SegmentType SegmentType { get; set; }

        [JsonProperty(PropertyName = "activityFeedSnippetRequired")]
        public bool ActivityFeedSnippetRequired { get; set; }
    }
}
