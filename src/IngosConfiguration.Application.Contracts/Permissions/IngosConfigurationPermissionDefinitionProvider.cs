using IngosConfiguration.Domain.Shared.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IngosConfiguration.Application.Contracts.Permissions
{
    public class IngosConfigurationPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(IngosConfigurationPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(IngosConfigurationPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<IngosConfigurationResource>(name);
        }
    }
}