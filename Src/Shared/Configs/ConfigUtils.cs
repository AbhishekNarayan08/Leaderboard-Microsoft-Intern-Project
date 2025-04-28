// <copyright file="ConfigUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;

    public static class ConfigUtils
    {
        private const string KeyVaultValuePrefix = "KeyVault=";
        private static readonly KeyVaultClient vaultClient;

        static ConfigUtils()
        {
            var isBuildPipeline = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BUILD_BUILDID"));

            // How to check Azure function is running on local environment? - https://stackoverflow.com/questions/45026215/how-to-check-azure-function-is-running-on-local-environment-roleenvironment-i
            var isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")) && !isBuildPipeline;

            // https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity?tabs=dotnet#asal
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        public static bool RetrieveVerifyBooleanConfigValue(string keyName, bool defaultValue = false)
        {
            return bool.TryParse(RetrieveVerifyConfigValue(keyName), out var configVaule) ? configVaule : defaultValue;
        }

        public static int RetrieveVerifyNumberConfigValue(string keyName, int defaultValue = 0)
        {
            return int.TryParse(RetrieveVerifyConfigValue(keyName), out var configVaule) ? configVaule : defaultValue;
        }

        public static string RetrieveVerifyConfigValue(string keyName, bool verify = true)
        {
            var configValueTask = RetrieveVerifyConfigValueAsync(keyName, verify);
            configValueTask.Wait();
            return configValueTask.Result;
        }

        public static async Task<string> RetrieveVerifyConfigValueAsync(string keyName, bool verify = true)
        {
            var configValue = Environment.GetEnvironmentVariable(keyName, EnvironmentVariableTarget.Process);

            if (verify && string.IsNullOrWhiteSpace(configValue))
            {
                throw new Exception($"Config value for key {keyName} had a null or empty value");
            }

            // If we get key vault reference then get secret uri from it and resolve it manually
            if (configValue?.StartsWith(KeyVaultValuePrefix) == true)
            {
                var secretUri = configValue.Substring(KeyVaultValuePrefix.Length, configValue.Length - KeyVaultValuePrefix.Length);

                configValue = await GetVaultValue(secretUri);

                if (string.IsNullOrWhiteSpace(configValue))
                {
                    throw new Exception($"After reading from Key Vault, config value for key {keyName} had a null or empty value");
                }
            }

            return configValue;
        }

        public static async Task<string> GetVaultValue(string key, int maxAttempts = 3)
        {
            var exceptionList = new List<Exception>();

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    var secret = await vaultClient.GetSecretAsync(key);
                    return secret.Value;
                }
                catch (Exception ex)
                {
                    exceptionList.Add(ex);
                }
            }

            var builder = new StringBuilder($"Failed to read config value for key: {key} from key vault.");
            builder.AppendLine();

            var cnt = 1;

            if (exceptionList.Count > 0)
            {
                foreach (var exception in exceptionList)
                {
                    builder.Append($"Exception {cnt} of {exceptionList.Count}: ");
                    builder.Append(exception.ToString());
                    builder.AppendLine();
                    cnt++;
                }
            }

            throw new Exception(builder.ToString());
        }
    }
}
