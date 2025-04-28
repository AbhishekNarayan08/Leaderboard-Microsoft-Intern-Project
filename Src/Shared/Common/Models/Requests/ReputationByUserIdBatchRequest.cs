// <copyright file="ReputationByUserIdBatchRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ReputationByUserIdBatchRequest
    {
        [JsonProperty(PropertyName = "userIds")]
        public List<string> UserIds { get; set; }

        [JsonProperty(PropertyName = "activityFeedSnippetRequired")]
        public bool ActivityFeedSnippetRequired { get; set; }
    }
}
