// <copyright file="Badge.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System;
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class Badge
    {
        [JsonProperty(PropertyName = "BadgeType")]
        public BadgeType BadgeType { get; set; }

        [JsonProperty(PropertyName = "BadgeName")]
        public string BadgeName { get; set; }

        [JsonProperty(PropertyName = "CreationDate")]
        public DateTimeOffset CreationDate { get; set; }
    }
}
