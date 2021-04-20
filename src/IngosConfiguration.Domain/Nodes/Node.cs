// -----------------------------------------------------------------------
// <copyright file= "Node.cs">
//     Copyright (c) Danvic.Wang All rights reserved.
// </copyright>
// Author: Danvic.Wang
// Created DateTime: 2021-04-20 21:38
// Modified by:
// Description: Server node info
// -----------------------------------------------------------------------

using System;
using IngosConfiguration.Domain.Shared.Nodes;
using Volo.Abp.Domain.Entities;

namespace IngosConfiguration.Domain.Nodes
{
    public class Node : AggregateRoot<Guid>
    {
        /// <summary>
        /// ctor(just for orm)
        /// </summary>
        private Node()
        {
        }

        public Node(Guid id, string ip, NodeStatus nodeStatus, DateTimeOffset? lastActiveTime)
            : base(id)
        {
            Ip = ip;
            NodeStatus = nodeStatus;
            LastActiveTime = lastActiveTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NodeStatus NodeStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset? LastActiveTime { get; set; }

        public void Online()
        {
        }

        public void Offline()
        {
        }
    }
}