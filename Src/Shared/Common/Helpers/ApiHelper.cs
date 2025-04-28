// <copyright file="ApiHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using Common.Constants;
    using Common.Models.Enums;
    using Common.Utils;
    using Microsoft.AspNetCore.Http;

    public static class ApiHelper
    {
        public static string GetUserId(HttpRequest req)
        {
            var userId = string.Empty;

            if (!string.IsNullOrWhiteSpace(req.Query[ApiConstants.UserId]))
            {
                userId = req.Query[ApiConstants.UserId];
            }

            return userId;
        }

        public static List<string> GetUserIds(HttpRequest req)
        {
            var userIdList = new List<string>();

            string userIds = req.Query[ApiConstants.UserIds];
            if (string.IsNullOrWhiteSpace(userIds))
            {
                return userIdList;
            }

            var userIdSplit = userIds.Split(',');

            foreach (var userId in userIdSplit)
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    userIdList.Add(userId);
                }
            }

            return userIdList;
        }

        public static SegmentType GetSegmentType(HttpRequest req)
        {
            var segmentTypeString = req.Query[ApiConstants.SegmentType];

            var segmentType = EnumParserUtil.GetEnum<SegmentType>(segmentTypeString);

            return segmentType;
        }
    }
}
