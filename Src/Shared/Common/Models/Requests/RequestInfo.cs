// <copyright file="RequestInfo.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RequestInfo
    {
        [JsonProperty(PropertyName = "searchinfo")]
        public IDictionary<string, string> Searchinfo { get; set; }

        [JsonProperty(PropertyName = "userinfo")]
        public IDictionary<string, string> Userinfo { get; set; }

        [JsonProperty(PropertyName = "activityinfo")]
        public IDictionary<string, string> Activityinfo { get; set; }
    }
}
