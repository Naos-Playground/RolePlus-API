// -----------------------------------------------------------------------
// <copyright file="ReceivingPipelineMessageEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs
{
    using RolePlus.ExternModule.API.Engine.Framework.Network;
    using RolePlus.ExternModule.API.Engine.Framework.Structs;

#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// Contains all the information before a <see cref="TNetworkPipelineMessage{T1}"/> reaches the pipeline.
    /// </summary>
    public class ReceivingPipelineMessageEventArgs<T1> : PlexEventArgs<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingPipelineMessageEventArgs{T1}"/> class.
        /// </summary>
        /// <param name="msg"><inheritdoc cref="Message"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReceivingPipelineMessageEventArgs(TNetworkMessage<T1> msg, bool isAllowed = true)
        {
            Message = new TNetworkPipelineMessage<T1>(msg);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the received <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        public TNetworkPipelineMessage<T1> Message { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the message can reach the pipeline.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
