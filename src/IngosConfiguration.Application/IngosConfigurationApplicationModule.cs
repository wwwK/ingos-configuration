using IngosConfiguration.Application.Contracts;
using IngosConfiguration.Domain;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace IngosConfiguration.Application
{
    /// <summary>
    /// Application Module
    /// </summary>
    [DependsOn(
        typeof(IngosConfigurationDomainModule),
        typeof(IngosConfigurationApplicationContractsModule),
        typeof(AbpPermissionManagementApplicationModule)
    )]
    public class IngosConfigurationApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options => { options.AddMaps<IngosConfigurationApplicationModule>(); });
        }
    }
}