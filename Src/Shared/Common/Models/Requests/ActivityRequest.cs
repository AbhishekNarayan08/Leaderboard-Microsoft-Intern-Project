// <copyright file="ActivityRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Requests
{
    using System;
    using Common.Models.Enums;
    using Common.Models.Responses;
    using Common.Utils;
    using Newtonsoft.Json;

    public class ActivityRequest
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "activityId")]
        public string ActivityId { get; set; }

        [JsonProperty(PropertyName = "segmentId")]
        public string SegmentId { get; set; }

        [JsonProperty(PropertyName = "activityType")]
        public string ActivityType { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? TimeStamp { get; set; }

        [JsonProperty(PropertyName = "requestInfo")]
        public RequestInfo RequestInfo { get; set; }

        [JsonProperty(PropertyName = "contentInfo")]
        public ContentInfo ContentInfo { get; set; }

        /// <summary>
        /// validate activity data is validate.
        /// </summary>
        /// <returns>SUCCESS response if everything is as expected else suitable error code</returns>
        public ResponseCode ValidateData()
        {
            if (string.IsNullOrWhiteSpace(this.ActivityId))
            {
                return ResponseCode.INVALID_ACTIVITY_ID;
            }

            var activity = EnumParserUtil.GetEnum<ActivityType>(this.ActivityType);
            if (activity == Enums.ActivityType.Unknown)
            {
                return ResponseCode.INVALID_ACTIVITY_TYPE;
            }

            var segment = EnumParserUtil.GetEnum<SegmentType>(this.SegmentId);
            if (segment == SegmentType.Unknown)
            {
                return ResponseCode.INVALID_SEGMENT_ID;
            }

            return ResponseCode.SUCCESS;
        }
    }
}
