using IngosConfiguration.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace IngosConfiguration.Infrastructure
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See IngosConfigurationMigrationsDbContext for migrations.
     */

    [ConnectionStringName("Default")]
    public class IngosConfigurationDbContext : AbpDbContext<IngosConfigurationDbContext>
    {
        /* Add DbSet properties for your Aggregate Roots / Entities here.
         * Also map them inside IngosConfigurationDbContextModelCreatingExtensions.ConfigureIngosConfiguration
         */

        public IngosConfigurationDbContext(DbContextOptions<IngosConfigurationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Configure the shared tables (with included modules) here */
            builder.ConfigureAbpEntities();

            /* Configure your own tables/entities inside the ConfigureIngosConfiguration method */
            builder.ConfigureIngosConfiguration();

            // Due to https://github.com/abpframework/abp/pull/7849 has not release, adopt the temporary method
            // ConfigureNamingConversion(builder);
        }
    }
}