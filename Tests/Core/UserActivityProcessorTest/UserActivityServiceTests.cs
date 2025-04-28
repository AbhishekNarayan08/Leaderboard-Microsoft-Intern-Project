// <copyright file="UserActivityServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UserActivityProcessorTest
{
    using Common.Models.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using UserActivityProcessor;

    [TestClass]
    public class UserActivityServiceTests
    {
        private IUserActivityService userActivityService = ActivityRequestUtil.GetUserActivityService();

        [TestMethod]
        public void AddValidActivityRequest()
        {
            Mock<HttpRequest> mockRequest = ActivityRequestUtil.CreateMockRequest(ActivityRequestUtil.GetActivityRequest());
            var response = userActivityService.ProcessAddActivityRequest(mockRequest.Object, null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(ResponseCode.SUCCESS, response.Result.Code);
        }

        [TestMethod]
        public void InValidActivityRequest()
        {
            Mock<HttpRequest> mockRequest = ActivityRequestUtil.CreateMockRequest(ActivityRequestUtil.GetActivityRequest(activityId: null));
            var response = userActivityService.ProcessAddActivityRequest(mockRequest.Object, null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(ResponseCode.INVALID_ACTIVITY_ID, response.Result.Code);

            mockRequest = ActivityRequestUtil.CreateMockRequest(ActivityRequestUtil.GetActivityRequest(activityType: null));
            response = userActivityService.ProcessAddActivityRequest(mockRequest.Object, null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(ResponseCode.INVALID_ACTIVITY_TYPE, response.Result.Code);

            mockRequest = ActivityRequestUtil.CreateMockRequest(ActivityRequestUtil.GetActivityRequest(segment: null));
            response = userActivityService.ProcessAddActivityRequest(mockRequest.Object, null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(ResponseCode.INVALID_SEGMENT_ID, response.Result.Code);

            mockRequest = ActivityRequestUtil.CreateMockRequest(ActivityRequestUtil.GetActivityRequest(activityId: null), userId: null);
            response = userActivityService.ProcessAddActivityRequest(mockRequest.Object, null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(ResponseCode.INVALID_USER_ID, response.Result.Code);

            mockRequest = ActivityRequestUtil.CreateMockRequest(activity: null);
            response = userActivityService.ProcessAddActivityRequest(mockRequest.Object, null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(ResponseCode.INVALID_REQUEST_BODY, response.Result.Code);
        }

        [TestMethod]
        public void ProcessActivityRequest()
        {
            EventData[] eventData = new EventData[]
            {
                new EventData(ActivityRequestUtil.CreateActivityData(ActivityRequestUtil.GetActivityRequest(userId: "user001"))),
                new EventData(ActivityRequestUtil.CreateActivityData(ActivityRequestUtil.GetActivityRequest())),
            };

            var response = userActivityService.ProcessEvents(
                eventData,
                new Mock<ExecutionContext>().Object,
                new Mock<ILogger>().Object);
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
        }

        [TestMethod]
        public void InvalidEventDataProcess()
        {
            EventData[] eventData = new EventData[]
                {
                    new EventData(ActivityRequestUtil.CreateActivityData(activity: null)),
                };

            var response = userActivityService.ProcessEvents(
                eventData,
                new Mock<ExecutionContext>().Object,
                new Mock<ILogger>().Object);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Exception);
            Assert.AreEqual(1, response.Exception.InnerExceptions.Count);
        }
    }
}