﻿using Serilog;

namespace Kmd.Logic.DocumentGeneration.Client.ConfigurationSample
{
    internal class ConfigurationValidator
    {
        private readonly AppConfiguration _configuration;

        public ConfigurationValidator(AppConfiguration configuration)
        {
            this._configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(this._configuration.TokenProvider?.ClientId)
                || string.IsNullOrWhiteSpace(this._configuration.TokenProvider?.ClientSecret)
                || string.IsNullOrWhiteSpace(this._configuration.TokenProvider?.AuthorizationScope))
            {
                Log.Error(
                    "Invalid configuration. Please provide proper information to `appsettings.json`. Current data is: {@Settings}",
                    this._configuration);

                return false;
            }

            if (this._configuration.DocumentGeneration?.SubscriptionId == null)
            {
                Log.Error(
                    "Invalid configuration. DocumentGeneration must have a configured SubscriptionId in `appsettings.json`. Current data is: {@Settings}",
                    this._configuration);

                return false;
            }

            if (this._configuration.DocumentGeneration?.DocumentGenerationServiceUri == null)
            {
                Log.Error(
                    "Invalid configuration. DocumentGeneration must have a configured DocumentGenerationServiceUri in `appsettings.json`. Current data is: {@Settings}",
                    this._configuration);

                return false;
            }

            if (this._configuration.ConfigurationSample?.ConfigurationId == null)
            {
                Log.Error(
                    "Invalid configuration. DocumentGeneration must have a configured DocumentGenerationServiceUri in `appsettings.json`. Current data is: {@Settings}",
                    this._configuration);

                return false;
            }

            if (this._configuration.ConfigurationSample.UseAzureBlob && this._configuration.ConfigurationSample.UseSharePointOnline)
            {
                Log.Error(
                    "Invalid configuration. ConfigurationSample:UseAzureBlob and ConfigurationSample:UseSharePointOnline cannot both be true",
                    this._configuration);

                return false;
            }

            if (!this._configuration.ConfigurationSample.UseAzureBlob && !this._configuration.ConfigurationSample.UseSharePointOnline)
            {
                Log.Error(
                    "Invalid configuration. One of ConfigurationSample:UseAzureBlob and ConfigurationSample:UseSharePointOnline must be true",
                    this._configuration);

                return false;
            }

            return true;
        }
    }
}
