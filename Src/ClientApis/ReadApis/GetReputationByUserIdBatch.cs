// <copyright file="GetReputationByUserIdBatch.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ReadApis
{
    using System.Threading.Tasks;
    using Common.Constants;
    using GetReputationProcessor;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using ReadApis.Factories;

    public class GetReputationByUserIdBatch
    {
        private readonly IGetReputationService getReputationService;
        private readonly IReputationServiceFactory reputationFactory;

        public GetReputationByUserIdBatch(IReputationServiceFactory reputationFactory)
        {
            this.reputationFactory = reputationFactory;
            this.getReputationService = this.reputationFactory.GetReputationService<GetReputationByUserIdBatchService>();
        }

        [FunctionName(FunctionNames.GetReputationByUserIdBatch)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger logger)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var output = await this.getReputationService.GetReputation(req, logger);

            return new OkObjectResult(output);
        }
    }
}
