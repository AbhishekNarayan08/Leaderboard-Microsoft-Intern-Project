// <copyright file="ReputationByUserIdRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using Newtonsoft.Json;

    public class ReputationByUserIdRequest
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "activityFeedSnippetRequired")]
        public bool ActivityFeedSnippetRequired { get; set; }
    }
}
