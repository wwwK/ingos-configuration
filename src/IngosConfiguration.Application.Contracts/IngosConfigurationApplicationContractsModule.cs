using IngosConfiguration.Domain.Shared;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;

namespace IngosConfiguration.Application.Contracts
{
    [DependsOn(
        typeof(IngosConfigurationDomainSharedModule),
        typeof(AbpObjectExtendingModule),
        typeof(AbpPermissionManagementApplicationContractsModule)
    )]
    public class IngosConfigurationApplicationContractsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            IngosConfigurationDtoExtensions.Configure();
        }
    }
}