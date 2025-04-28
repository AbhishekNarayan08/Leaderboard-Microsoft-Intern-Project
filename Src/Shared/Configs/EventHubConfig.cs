// <copyright file="EventHubConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    public class EventHubConfig : IEventHubConfig
    {
        public string ConnectionString { get; private set; } = ConfigUtils.RetrieveVerifyConfigValue("EventHubConnectionString");
    }
}
