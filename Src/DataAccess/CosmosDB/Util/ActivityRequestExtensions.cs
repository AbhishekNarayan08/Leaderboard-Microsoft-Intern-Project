// <copyright file="ActivityRequestExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Util
{
    using System;
    using System.Collections.Generic;
    using Common.Helpers;
    using Common.Models.Requests;
    using Common.Utils;
    using CosmosDB.Models;
    using CosmosDB.Services.Data;

    public static class ActivityRequestExtensions
    {
        public static UserProfile ToUserProfile(this ActivityRequest activityRequest, bool encodeIds = true)
        {
            var userId = encodeIds
                ? EncodeDecodeIdHelper.Encode(activityRequest.UserId)
                : activityRequest.UserId;

            return new UserProfile
            {
                Id = userId,
                UserId = userId,
                DocType = EnumParserUtil.GetString(CosmosDocType.UserProfile),
                ReputationBySegment = new Dictionary<string, Reputation>()
                {
                    {
                        activityRequest.SegmentId,
                        new Reputation
                        {
                            PointValue = 0,
                            Level = new Level(),
                            Segment = activityRequest.SegmentId,
                            LastUpdated = activityRequest.TimeStamp ?? DateTimeOffset.UtcNow,
                            Badges = new List<Badge>(),
                            ActivityStats = new Dictionary<string, ActivityStats>(),
                        }
                    },
                },
                Attributes = new UserProfileAttributes(),
            };
        }

        public static ReputationActivity ToReputationActivity(this ActivityRequest activityRequest, bool encodeIds = true)
        {
            return new ReputationActivity
            {
                Id = encodeIds
                ? EncodeDecodeIdHelper.Encode(activityRequest.ActivityId)
                : activityRequest.ActivityId,
                PointValue = 0,
                ActivityType = activityRequest.ActivityType,
                Segment = activityRequest.SegmentId,
                DocType = EnumParserUtil.GetString(CosmosDocType.Activity),
                UserId = encodeIds
                ? EncodeDecodeIdHelper.Encode(activityRequest.UserId)
                : activityRequest.UserId,
                TimeStamp = activityRequest.TimeStamp ?? DateTimeOffset.UtcNow,
                Content = new ContentMetaData
                {
                    ContentId = activityRequest.ContentInfo?.Id,
                    CreationDateTime = activityRequest.ContentInfo?.CreationDateTime ?? DateTimeOffset.UtcNow,
                    LastAccessDateTime = activityRequest.ContentInfo?.LastAccessDateTime ?? DateTimeOffset.UtcNow,
                    ContentType = activityRequest.ContentInfo?.Type,
                    RequeryURL = activityRequest.ContentInfo?.RequestURL,
                },
            };
        }
    }
}
