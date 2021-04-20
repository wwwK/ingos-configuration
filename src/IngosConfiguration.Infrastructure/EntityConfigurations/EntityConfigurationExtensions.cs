﻿//-----------------------------------------------------------------------
// <copyright file= "EntityConfigurationExtensions.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2021/3/7 14:44:35
// Modified by:
// Description: Entity to table configuration
//-----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

namespace IngosConfiguration.Infrastructure.EntityConfigurations
{
    public static class EntityConfigurationExtensions
    {
        /// <summary>
        ///     Configure Abp framework own tables/entities
        /// </summary>
        public static void ConfigureAbpEntities(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.ConfigureAuditLogging();
            builder.ConfigureBackgroundJobs();
            builder.ConfigurePermissionManagement();

//             // Due to https://github.com/abpframework/abp/pull/7849 has not release, adopt the temporary method
//             //
//             builder.ConfigureIngosAuditLogging();
//             builder.ConfigureIngosBackgroundJobs();
//             builder.ConfigureIngosPermissionManagement();
        }

        /// <summary>
        ///     Configure project own tables/entities
        /// </summary>
        public static void ConfigureIngosConfiguration(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(IngosConfigurationConsts.DbTablePrefix + "YourEntities", IngosConfigurationConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}