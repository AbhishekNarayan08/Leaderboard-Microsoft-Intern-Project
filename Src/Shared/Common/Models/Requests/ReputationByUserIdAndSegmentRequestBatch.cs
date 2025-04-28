// <copyright file="ReputationByUserIdAndSegmentRequestBatch.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using System.Collections.Generic;
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class ReputationByUserIdAndSegmentRequestBatch
    {
        [JsonProperty(PropertyName = "userIds")]
        public List<string> UserIds { get; set; }

        [JsonProperty(PropertyName = "segmentType")]
        public SegmentType SegmentType { get; set; }

        [JsonProperty(PropertyName = "activityFeedSnippetRequired")]
        public bool ActivityFeedSnippetRequired { get; set; }
    }
}
