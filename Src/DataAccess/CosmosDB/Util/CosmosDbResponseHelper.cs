// <copyright file="CosmosDbResponseHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Util
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Models.Enums;
    using Common.Models.Responses;
    using Common.Utils;
    using CosmosDbModels = CosmosDB.Models;

    public static class CosmosDbResponseHelper
    {
        public static ReputationByUserId UserProfileToReputationByUserId(CosmosDbModels.UserProfile userProfile)
        {
            var reputationListBySegment = new List<Reputation>();
            var reputationBySegment = userProfile.ReputationBySegment;

            foreach (var segmentKey in reputationBySegment.Keys)
            {
                var segmentType = EnumParserUtil.GetEnum<SegmentType>(segmentKey);

                if (segmentType == SegmentType.Unknown)
                {
                    // TODO : Log
                    continue;
                }

                var reputationValue = reputationBySegment[segmentKey];

                var reputation = new Reputation();
                reputation.SegmentType = segmentType;
                reputation.SegmentTypeString = EnumParserUtil.GetString(segmentType);
                reputation.UserId = EncodeDecodeIdHelper.Decode(userProfile.UserId);
                reputation.TotalScore = reputationValue.PointValue;

                UpdateStats(reputation, reputationValue.ActivityStats);
                UpdateLevel(reputation, reputationValue.Level);
                UpdateBadges(reputation, reputationValue.Badges);

                reputationListBySegment.Add(reputation);
            }

            if (reputationListBySegment.Count == 0)
            {
                return new ReputationByUserId(ResponseCode.UNKNOWN_ERROR);
            }

            return new ReputationByUserId(ResponseCode.SUCCESS, null, reputationListBySegment);
        }

        public static ReputationByUserIdAndSegment UserProfileToReputationByUserIdAndSegment(
            CosmosDbModels.UserProfile userProfile,
            SegmentType segmentType)
        {
            var reputationByUserId = UserProfileToReputationByUserId(userProfile);

            var segmentReputation = reputationByUserId?.ReputationBySegment?.Where(rep => segmentType.Equals(rep.SegmentType)).FirstOrDefault();

            if (segmentReputation == null)
            {
                return new ReputationByUserIdAndSegment(ResponseCode.INVALID_REQUEST_BODY);
            }

            return new ReputationByUserIdAndSegment(ResponseCode.SUCCESS, segmentReputation);
        }

        public static ActivityFeedByUserId ReputationActivityListToActivityFeedByUserId(List<CosmosDbModels.ReputationActivity> reputationActivityList)
        {
            var activityList = new List<Activity>();
            if (reputationActivityList == null || reputationActivityList.Count == 0)
            {
                return new ActivityFeedByUserId(ResponseCode.INVALID_USER_ID);
            }

            foreach (var reputationActivity in reputationActivityList)
            {
                if (reputationActivity == null)
                {
                    continue;
                }

                var segmentType = EnumParserUtil.GetEnum<SegmentType>(reputationActivity.Segment);
                if (segmentType == SegmentType.Unknown)
                {
                    // TODO : Log
                    continue;
                }

                var activityType = EnumParserUtil.GetEnum<ActivityType>(reputationActivity.ActivityType);
                if (activityType == ActivityType.Unknown)
                {
                    // TODO : Log
                    continue;
                }

                var activity = new Activity();
                activity.ActivityId = EncodeDecodeIdHelper.Decode(reputationActivity.Id);
                activity.ActivityPoints = reputationActivity.PointValue;
                activity.UserId = EncodeDecodeIdHelper.Decode(reputationActivity.UserId);
                activity.SegmentType = segmentType;
                activity.SegmentTypeString = EnumParserUtil.GetString(segmentType);
                activity.ActivityType = activityType;
                activity.ActivityTypeString = EnumParserUtil.GetString(activityType);
                UpdateContentMetadata(activity, reputationActivity.Content);

                activityList.Add(activity);
            }

            return new ActivityFeedByUserId(ResponseCode.SUCCESS, activityList);
        }

        public static ActivityFeedByUserIdAndSegment ReputationActivityListToActivityFeedByUserIdAndSegment(
            List<CosmosDbModels.ReputationActivity> reputationActivityList,
            SegmentType segmentType)
        {
            var activityFeedByUserId = ReputationActivityListToActivityFeedByUserId(reputationActivityList);
            var activityListBySegment = activityFeedByUserId?.ActivityFeed?.Where(feed => feed.SegmentType == segmentType).ToList();

            if (activityListBySegment == null || activityListBySegment.Count == 0)
            {
                return new ActivityFeedByUserIdAndSegment(ResponseCode.INVALID_USER_ID_OR_SEGMENT_TYPE);
            }

            return new ActivityFeedByUserIdAndSegment(ResponseCode.SUCCESS, activityListBySegment);
        }

        private static void UpdateContentMetadata(Activity activity, CosmosDbModels.ContentMetaData content)
        {
            if (content == null)
            {
                return;
            }

            var contentType = EnumParserUtil.GetEnum<ContentType>(content.ContentType);
            if (contentType == ContentType.Unknown)
            {
                // TODO : Log
                return;
            }

            activity.Content = new ContentMetadata();
            activity.Content.ContentId = content.ContentId;
            activity.Content.CreationDateTime = content.CreationDateTime;
            activity.Content.LastAccessDateTime = content.LastAccessDateTime;
            activity.Content.ContentType = contentType;
            activity.Content.ContentTypeString = EnumParserUtil.GetString(contentType);
            activity.Content.RequeryURL = content.RequeryURL;
        }

        private static void UpdateBadges(Reputation reputation, List<CosmosDbModels.Badge> badges)
        {
            if (badges == null)
            {
                return;
            }

            reputation.Badges = new List<Badge>();
            foreach (var badge in badges)
            {
                var badgeType = EnumParserUtil.GetEnum<BadgeType>(badge.BadgeId);

                if (badgeType == BadgeType.Unknown)
                {
                    continue;
                }

                var reputationBadge = new Badge();
                reputationBadge.BadgeType = badgeType;
                reputationBadge.BadgeName = badge.Name;
                reputationBadge.CreationDate = badge.CreationDate;

                reputation.Badges.Add(reputationBadge);
            }
        }

        private static void UpdateLevel(Reputation reputation, CosmosDbModels.Level level)
        {
            if (level == null)
            {
                return;
            }

            var reputationLevel = new Level();
            reputationLevel.CurrentLevel = level.CurrentLevel;
            reputationLevel.PointsAtNextMilestone = level.PointsAtNextMilestone;

            reputation.Level = reputationLevel;
        }

        private static void UpdateStats(Reputation reputation, IDictionary<string, CosmosDbModels.ActivityStats> activityStats)
        {
            if (activityStats == null)
            {
                return;
            }

            reputation.Stats = new List<Stats>();

            foreach (var activityStatsKey in activityStats.Keys)
            {
                var activityType = EnumParserUtil.GetEnum<ActivityType>(activityStatsKey);

                if (activityType == ActivityType.Unknown)
                {
                    continue;
                }

                var activityStatsResponse = activityStats[activityStatsKey];

                var stats = new Stats();
                stats.ActivityType = activityType;
                stats.ActivityTypeString = EnumParserUtil.GetString(activityType);
                stats.TotalCount = activityStatsResponse.CountValue;
                stats.TotalScore = activityStatsResponse.PointValue;

                reputation.Stats.Add(stats);
            }
        }
    }
}
