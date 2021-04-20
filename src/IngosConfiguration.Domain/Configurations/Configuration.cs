// -----------------------------------------------------------------------
// <copyright file= "Configuration.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2021-04-20 21:51
// Modified by:
// Description:
// -----------------------------------------------------------------------

using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace IngosConfiguration.Domain.Configurations
{
    public class Configuration : FullAuditedAggregateRoot<Guid>
    {
        private Configuration()
        {
        }

        public Guid ApplicationId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}