// <copyright file="FunctionNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Constants
{
    public class FunctionNames
    {
        // Write APIs
        public const string UserActivity = "UserActivity";
        public const string ProcessUgcEvents = "ProcessUgcEvents";
        public const string ProcessBulkEvents = "ProcessBulkEvents";

        // Read APIs
        public const string GetReputationByUserId = "GetReputationByUserId";
        public const string GetReputationByUserIdBatch = "GetReputationByUserIdBatch";
        public const string GetReputationByUserIdAndSegment = "GetReputationByUserIdAndSegment";
        public const string GetReputationByUserIdAndSegmentBatch = "GetReputationByUserIdAndSegmentBatch";

        public const string GetActivityFeedByUserId = "GetActivityFeedByUserId";
        public const string GetActivityFeedByUserIdAndSegment = "GetActivityFeedByUserIdAndSegment";
    }
}
