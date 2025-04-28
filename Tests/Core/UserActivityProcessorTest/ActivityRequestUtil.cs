// <copyright file="ActivityRequestUtil.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UserActivityProcessorTest
{
    using System;
    using System.IO;
    using System.Text;
    using AzureStorage;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using CosmosDB.Interfaces;
    using CosmosDB.Services.Data;
    using EventHub;
    using LeaderboardProcessor;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Moq;
    using Newtonsoft.Json;
    using RuleEngine.Core;
    using UserActivityProcessor;

    public class ActivityRequestUtil
    {
        public static ActivityRequest GetActivityRequest(string? activityId = "activity01", string? segment = "local", string? userId = null, string? activityType = "comment")
        {
            return new ActivityRequest()
            {
                ActivityId = activityId,
                ActivityType = activityType,
                SegmentId = segment,
                TimeStamp = DateTime.Now,
                UserId = userId,
            };
        }

        public static byte[] CreateActivityData(object activity)
        {
            var json = JsonConvert.SerializeObject(activity);
            return Encoding.ASCII.GetBytes(json);
        }

        public static Mock<HttpRequest> CreateMockRequest(object activity, string? userId = "test01")
        {
            var byteArray = CreateActivityData(activity);

            var memoryStream = new MemoryStream(byteArray);
            memoryStream.Flush();
            memoryStream.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(memoryStream);

            mockRequest.Setup(x => x.Query["userId"]).Returns(userId);
            return mockRequest;
        }

        public static IUserActivityService GetUserActivityService()
        {
            var blobStorageContext = new Mock<IBlobStorageContext>();
            var eventHubContext = new Mock<IEventHubContext>();
            var cosmosDbReadClient = new Mock<ICosmosDbReadClient>();
            var cosmosDbWriteClient = new Mock<ICosmosDbWriteClient>();
            var ruleEngine = new Mock<IRuleEngine>();
            eventHubContext.Setup(s => s.AddActivityMessage(It.IsAny<ActivityRequest>(), It.IsAny<IAsyncCollector<string>>()).Result).Returns(new IBaseResponse(ResponseCode.SUCCESS));

            return new UserActivityService(blobStorageContext.Object, eventHubContext.Object, cosmosDbReadClient.Object, cosmosDbWriteClient.Object, ruleEngine.Object);
        }
    }
}
