// <copyright file="ApiConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Constants
{
    public class ApiConstants
    {
        public const string UserId = "userId";
        public const string UserIds = "userIds";
        public const string SegmentType = "segmentType";
        public const int MaxRetryCount = 5;

        // write api constants
        public const string EventHubConnStrName = "EventHubConnectionString";
        public const string EventHubName = "activities";
        public const string ActivityStorageContainerName = "activity-logging";
        public const string SegmentOnboardActivitiesContainerName = "segment-onboard-activities";
    }
}
