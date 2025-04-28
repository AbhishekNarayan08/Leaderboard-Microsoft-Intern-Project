// <copyright file="JsonConvertUtilTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CommonTest
{
    using Common.Models.Enums;
    using Common.Models.Requests;
    using Common.Utils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonConvertUtilTest
    {
        [TestMethod]
        public void TestDeserializeActivityRequest()
        {
            // -ve scenarios
            Assert.IsNull(JsonConvertUtil.DeserializeActivityRequest(" "));
            Assert.IsNull(JsonConvertUtil.DeserializeActivityRequest(null));

            // +ve scenarios
            var request = JsonConvertUtil.DeserializeActivityRequest("{\"activityId\" : \"W2244U5D2434W3555OGI2UGOW5WDW1630\",\"segmentId\": \"travel\",\"activityType\":\"comment\",\"timestamp\":\"2022-01-25T12:12:23\",\"requestInfo\" : {\"searchinfo\" : {\"channel\": \"Bing.com\",\"url\": \"https://www.bing.com/test\",\"clientId\":\"IU787680obhd8yw8922b282hr\"},\"userinfo\" : {\"active since\": \"2018\",\"country\": \"india\"},\"activityinfo\" : {\"name\": \"photo comment\",\"segment\": \"travel\"}},\"contentInfo\" : {\"id\": \"nsqi239013bidbidbqe081\",\"creationTime\":\"\",\"lastAccessTime\":\"\",\"requestUrl\": \"https://www.bing.com/test\",\"type\":\"comment\"}}");
            Assert.IsNotNull(request);
            Assert.AreEqual("W2244U5D2434W3555OGI2UGOW5WDW1630", request.ActivityId);
        }

        [TestMethod]
        public void TestSerializeActivityRequest()
        {
            // -ve scenarios
            Assert.IsNull(JsonConvertUtil.SerializeActivityRequest(null));

            var request = new ActivityRequest
            {
                ActivityId = "W2244U5D2434W3555OGI2UGOW5WDW1630",
                SegmentId = SegmentType.Travel.ToString(),
                ActivityType = ActivityType.Comment.ToString(),
                UserId = "IU787680obhd8yw8922b282hr",
                TimeStamp = new System.DateTime(2022, 02, 15, 0, 0, 0),
            };

            string jsonStr = JsonConvertUtil.SerializeActivityRequest(request);

            // +ve scenarios
            Assert.AreEqual("{\"userId\":\"IU787680obhd8yw8922b282hr\",\"activityId\":\"W2244U5D2434W3555OGI2UGOW5WDW1630\",\"segmentId\":\"Travel\",\"activityType\":\"Comment\",\"timestamp\":\"2022-02-15T00:00:00\",\"requestInfo\":null,\"contentInfo\":null}", jsonStr);
        }
    }
}
