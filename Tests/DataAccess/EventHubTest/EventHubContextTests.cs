// <copyright file="EventHubContextTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventHubTest
{
    using Common.Models.Responses;
    using EventHub;
    using Microsoft.Azure.WebJobs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using UserActivityProcessorTest;

    [TestClass]
    public class EventHubContextTests
    {
        private readonly IEventHubContext eventHubContext = new EventHubContext();

        [TestMethod]
        public void AddValidMessage()
        {
            var eventQueue = new Mock<IAsyncCollector<string>>().Object;
            var response = this.eventHubContext.AddActivityMessage(ActivityRequestUtil.GetActivityRequest(), eventQueue);
            Assert.IsNotNull(response);
            Assert.AreEqual(ResponseCode.SUCCESS, response.Result.Code);
        }

        [TestMethod]
        public void DiscardInvalidMessage()
        {
            var eventQueue = new Mock<IAsyncCollector<string>>().Object;
            var response = this.eventHubContext.AddActivityMessage(null, eventQueue);
            Assert.IsNotNull(response);
            Assert.AreEqual(ResponseCode.INVALID_REQUEST_BODY, response.Result.Code);
        }
    }
}