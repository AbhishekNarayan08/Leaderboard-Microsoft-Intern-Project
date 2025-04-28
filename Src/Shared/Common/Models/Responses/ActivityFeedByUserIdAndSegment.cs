// <copyright file="ActivityFeedByUserIdAndSegment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ActivityFeedByUserIdAndSegment : IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFeedByUserIdAndSegment"/> class.
        /// </summary>
        /// <param name="code">ResponseCode</param>
        public ActivityFeedByUserIdAndSegment(ResponseCode code, List<Activity> activityFeed = null)
            : base(code)
        {
            this.ActivityFeed = activityFeed;
        }

        [JsonProperty(PropertyName = "ActivityFeed")]
        public List<Activity> ActivityFeed { get; set; }
    }
}
