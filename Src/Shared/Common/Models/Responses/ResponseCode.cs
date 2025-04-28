// <copyright file="ResponseCode.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    public enum ResponseCode
    {
        SUCCESS,
        INVALID_USER_ID,
        INVALID_REQUEST_BODY,
        INVALID_ACTIVITY_ID,
        INVALID_SEGMENT_ID,
        INVALID_ACTIVITY_TYPE,
        FAILED_ADD_USER_ACTIVITY,
        ACTIVITY_ID_ALREADY_EXISTS,
        INVALID_USER_ID_OR_SEGMENT_TYPE,
        UNKNOWN_ERROR,
    }
}