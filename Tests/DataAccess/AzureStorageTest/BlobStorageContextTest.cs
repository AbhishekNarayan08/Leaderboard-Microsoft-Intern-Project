// <copyright file="BlobStorageContextTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AzureStorageTest
{
    using System.IO;
    using System.Threading;
    using Azure;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using AzureStorage;
    using Common.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using UserActivityProcessorTest;

    [TestClass]
    public class BlobStorageContextTest
    {
        private IBlobStorageContext blobStorageContext = new BlobStorageContext(GetMockBlobServiceClient());

        [TestMethod]
        public void UploadValidActivity()
        {
            var response = this.blobStorageContext.UploadAddActivityRequest(ActivityRequestUtil.GetActivityRequest(), "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response);
            Assert.AreEqual(ResponseCode.SUCCESS, response.Result);
        }

        [TestMethod]
        public void UploadInvalidActivity()
        {
            var response = this.blobStorageContext.UploadAddActivityRequest(null, "activities", new Mock<ILogger>().Object);
            Assert.IsNotNull(response);
            Assert.AreEqual(ResponseCode.INVALID_REQUEST_BODY, response.Result);
        }

        private static BlobServiceClient GetMockBlobServiceClient()
        {
            var mockBlobServiceClient = new Mock<BlobServiceClient>();
            var mockContainerClient = new Mock<BlobContainerClient>();
            var blobContentInfo = new Mock<Response<BlobContentInfo>>();
            var mockBlobClient = new Mock<BlobClient>();

            blobContentInfo.SetupGet(s => s.GetRawResponse().Status).Returns(200);
            mockBlobClient.Setup(s => s.UploadAsync(It.IsAny<Stream>(), false, default).Result).Returns(blobContentInfo.Object);
            mockContainerClient.Setup(s => s.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);
            mockBlobServiceClient.Setup(s => s.GetBlobContainerClient(It.IsAny<string>())).Returns(mockContainerClient.Object);
            return mockBlobServiceClient.Object;
        }
    }
}