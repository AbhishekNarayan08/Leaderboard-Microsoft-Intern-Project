// <copyright file="IBlobStorageContext.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace AzureStorage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs.Models;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public interface IBlobStorageContext
    {
        public Task<ResponseCode> UploadAddActivityRequest(ActivityRequest activityRequest, string containerName, ILogger logger);

        public AsyncPageable<BlobItem> ListBlobs(
            string containerName,
            string prefix = null);

        public Task<T> DownloadJSONObjectAsync<T>(
            string containerName,
            string blobPath,
            JsonSerializerSettings serializerSettings = null);

        public Task<bool> DeleteBlob(string containerName, string blobName);

        public IAsyncEnumerable<Page<BlobItem>> GetBlobItemPages(string containerName, int segmentSize);
    }
}
