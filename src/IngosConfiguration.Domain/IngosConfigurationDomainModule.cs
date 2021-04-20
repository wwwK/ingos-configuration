using IngosConfiguration.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace IngosConfiguration.Domain
{
    [DependsOn(
        typeof(IngosConfigurationDomainSharedModule),
        typeof(AbpAuditLoggingDomainModule),
        typeof(AbpBackgroundJobsDomainModule),
        typeof(AbpEmailingModule),
        typeof(AbpPermissionManagementDomainModule)
    )]
    public class IngosConfigurationDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}