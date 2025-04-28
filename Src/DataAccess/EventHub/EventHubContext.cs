// <copyright file="EventHubContext.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EventHub
{
    using System.Threading.Tasks;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using Common.Utils;
    using Microsoft.Azure.WebJobs;

    public class EventHubContext : IEventHubContext
    {
        /// <inheritdoc/>
        public async Task<IBaseResponse> AddActivityMessage(ActivityRequest activityRequest, IAsyncCollector<string> outputEvents)
        {
            var message = GetActivityMessage(activityRequest);
            if (message == null)
            {
                return new IBaseResponse(ResponseCode.INVALID_REQUEST_BODY);
            }

            await outputEvents.AddAsync(message);
            return new IBaseResponse(ResponseCode.SUCCESS);
        }

        private static string GetActivityMessage(ActivityRequest activityRequest)
        {
            return JsonConvertUtil.SerializeActivityRequest(activityRequest);
        }
    }
}
