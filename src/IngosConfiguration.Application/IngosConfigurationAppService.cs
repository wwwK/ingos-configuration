using IngosConfiguration.Domain.Shared.Localization;
using Volo.Abp.Application.Services;

namespace IngosConfiguration.Application
{
    /// <summary>
    /// Inherit your application services from this class.
    /// </summary>
    public abstract class IngosConfigurationAppService : ApplicationService
    {
        /// <summary>
        /// Base application service
        /// </summary>
        protected IngosConfigurationAppService()
        {
            LocalizationResource = typeof(IngosConfigurationResource);
        }
    }
}