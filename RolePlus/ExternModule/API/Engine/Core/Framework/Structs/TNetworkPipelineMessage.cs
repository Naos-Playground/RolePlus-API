// -----------------------------------------------------------------------
// <copyright file="TNetworkPipelineMessage.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Structs
{
    using RolePlus.ExternModule.API.Engine.Framework.Network;

#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// A message that is being sent before a new <see cref="UNetObject"/> is created.
    /// </summary>
    public struct TNetworkPipelineMessage<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TNetworkPipelineMessage{T1}"/> struct.
        /// </summary>
        /// <param name="obj">The network object.</param>
        public TNetworkPipelineMessage(TNetworkMessage<T1> obj)
        {
            Object = obj;
        }

        /// <summary>
        /// Gets the network object.
        /// </summary>
        public TNetworkMessage<T1> Object { get; }
    }
}
