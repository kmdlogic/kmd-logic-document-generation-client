using Serilog;

namespace Kmd.Logic.DocumentGeneration.Client.GenerationSample
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

            if (this._configuration.GenerationSample.ConfigurationId == null)
            {
                Log.Error(
                    "Invalid GenerationSample ConfigurationId. Please provide proper information to `appsettings.json`. Current data is: {@Settings}",
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

            return true;
        }
    }
}
