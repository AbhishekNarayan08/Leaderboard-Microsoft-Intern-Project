// <copyright file="UserProfileAttributes.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using Newtonsoft.Json;

    public class UserProfileAttributes
    {
        [JsonProperty(PropertyName = "aboutMe")]
        public string AboutMe { get; set; }

        [JsonProperty(PropertyName = "followerCount")]
        public string FollowerCount { get; set; }
    }
}
