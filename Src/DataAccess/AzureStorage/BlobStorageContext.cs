// <copyright file="BlobStorageContext.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Common.Constants;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using Common.Utils;
    using Configs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// blob storage context.
    /// </summary>
    /// <seealso cref="AzureStorage.IBlobStorageContext" />
    public class BlobStorageContext : IBlobStorageContext
    {
        private readonly BlobServiceClient blobServiceClient;

        public BlobStorageContext(IAzureStorageConfig azureStorageConfig)
        {
            this.blobServiceClient = new BlobServiceClient(azureStorageConfig.ConnectionString);
        }

        public BlobStorageContext(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Uploads the add activity request.
        /// </summary>
        /// <param name="activityRequest">The activity request.</param>
        /// <param name="containerName">storage containerName</param>
        /// <param name="logger">logger</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<ResponseCode> UploadAddActivityRequest(ActivityRequest activityRequest, string containerName, ILogger logger)
        {
            var request = JsonConvertUtil.SerializeActivityRequest(activityRequest);
            if (request == null)
            {
                logger.LogError("activityRequest is null");
                return ResponseCode.INVALID_REQUEST_BODY;
            }

            // process the stream
            var isContainerExist = await this.CreateContainerIfNotExists(containerName, logger);
            if (!isContainerExist)
            {
                logger.LogError($"Failed to create container {containerName}");
                return ResponseCode.FAILED_ADD_USER_ACTIVITY;
            }

            var blobPath = GetBlobNameFromActivityRequest(activityRequest);
            var bytes = Encoding.UTF8.GetBytes(request);

            var containerClient = this.blobServiceClient.GetBlobContainerClient(
                blobContainerName: containerName);

            var blobClient = containerClient.GetBlobClient(
                blobName: blobPath);

            using (var memoryStream = new MemoryStream(bytes))
            {
                try
                {
                    var response = await blobClient.UploadAsync(content: memoryStream, overwrite: false);

                    // httpStatus code should be 2xx always as we create/update blob storage.
                    var httpStatus = response?.GetRawResponse()?.Status;
                    if (httpStatus >= 200 && httpStatus < 300)
                    {
                        return ResponseCode.SUCCESS;
                    }
                    else
                    {
                        logger.LogError($"Blob upload failed. Reason: {response?.GetRawResponse()?.ReasonPhrase}");
                        return ResponseCode.FAILED_ADD_USER_ACTIVITY;
                    }
                }
                catch (Exception ex)
                {
                    if (ex?.Message?.Contains("ErrorCode: BlobAlreadyExists") == true)
                    {
                        return ResponseCode.ACTIVITY_ID_ALREADY_EXISTS;
                    }

                    return ResponseCode.FAILED_ADD_USER_ACTIVITY;
                }
            }
        }

        public AsyncPageable<BlobItem> ListBlobs(
            string containerName,
            string prefix = null)
        {
            var containerClient = this.blobServiceClient.GetBlobContainerClient(
                blobContainerName: containerName);

            return containerClient.GetBlobsAsync(
                prefix: prefix);
        }

        public async Task<T> DownloadJSONObjectAsync<T>(
            string containerName,
            string blobPath,
            JsonSerializerSettings serializerSettings = null)
        {
            var stream = await this.DownloadStreamAsync(
                containerName: containerName,
                blobPath: blobPath);

            if (stream == null)
            {
                return default(T);
            }

            var serializer = JsonSerializer.Create(serializerSettings ?? JsonConvertUtil.SerializationSettings);

            using (var reader = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public async Task<bool> DeleteBlob(string containerName, string blobName)
        {
            var containerClient = this.blobServiceClient.GetBlobContainerClient(
                blobContainerName: containerName);
            return await containerClient.DeleteBlobIfExistsAsync(blobName, DeleteSnapshotsOption.IncludeSnapshots);
        }

        public IAsyncEnumerable<Page<BlobItem>> GetBlobItemPages(string containerName, int segmentSize)
        {
            try
            {
                var blobContainerClient = this.blobServiceClient.GetBlobContainerClient(
                blobContainerName: containerName);

                // Call the listing operation and return pages of the specified size.
                return blobContainerClient.GetBlobsAsync().AsPages(default, segmentSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        private static string GetBlobNameFromActivityRequest(ActivityRequest activityRequest)
        {
            return activityRequest.ActivityId;
        }

        private async Task<Stream> DownloadStreamAsync(
            string containerName,
            string blobPath)
        {
            var response = await this.DownloadBlobAsync(
                containerName: containerName,
                blobPath: blobPath);

            return response.Content;
        }

        private async Task<BlobDownloadInfo> DownloadBlobAsync(
            string containerName,
            string blobPath)
        {
            var containerClient = this.blobServiceClient.GetBlobContainerClient(
                blobContainerName: containerName);

            var blobClient = containerClient.GetBlobClient(
                blobName: blobPath);

            var response = await blobClient.DownloadAsync();
            return response?.Value;
        }

        // This function only returns containerInfo != null if we just created the container
        // In all other circumstances it returns null (for example if container exists)
        private async Task<bool> CreateContainerIfNotExists(
            string containerName, ILogger logger)
        {
            for (var currentTry = 1; currentTry <= ApiConstants.MaxRetryCount; currentTry++)
            {
                try
                {
                    var containerClient = this.blobServiceClient.GetBlobContainerClient(
                        blobContainerName: containerName);

                    // PublicAccessType.Blob = Blob data within this container can be read via anonymous request, but container data is not available. Clients cannot enumerate blobs within the container via anonymous request.
                    await containerClient.CreateIfNotExistsAsync(
                        publicAccessType: PublicAccessType.Blob);
                    return true;
                }
                catch (Exception ex)
                {
                    if (currentTry < ApiConstants.MaxRetryCount)
                    {
                        // Retry with exponential backoff
                        // It will retry after 1s, 2s, 4s, 8s, etc.
                        var waitMilli = TimeSpan.FromMilliseconds(Math.Pow(2, currentTry - 1) * 1000);
                        await Task.Delay(waitMilli);
                    }
                    else
                    {
                        logger.LogError(ex.ToString());
                    }
                }
            }

            return false;
        }
    }
}
