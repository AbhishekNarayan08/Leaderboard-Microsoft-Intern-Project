// <copyright file="UserProfile.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserProfile
    {
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "docType")]
        public string DocType { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "reputation")]
        public IDictionary<string, Reputation> ReputationBySegment { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public UserProfileAttributes Attributes { get; set; }
    }
}
