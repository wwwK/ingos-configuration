// -----------------------------------------------------------------------
// <copyright file= "Application.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2021-04-20 21:52
// Modified by:
// Description:
// -----------------------------------------------------------------------

using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace IngosConfiguration.Domain.Applications
{
    public class Application : FullAuditedAggregateRoot<Guid>
    {
        private Application()
        {
        }

        public string Name { get; set; }

        public string AppId { get; set; }
        
        public string Secret { get; set; }

        public bool Enabled { get; set; }
    }
}