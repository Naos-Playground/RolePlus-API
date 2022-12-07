// -----------------------------------------------------------------------
// <copyright file="INetworkMessage.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// Basic implementation to handle network messages.
    /// </summary>
    public interface INetworkMessage<T1, T2> : INetworkMessage<T1>
    {
        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        T2 Content { get; set; }

        /// <summary>
        /// Resets the content.
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// Basic implementation to handle network messages.
    /// </summary>
    public interface INetworkMessage<T1>
    {
        /// <summary>
        /// Gets the source of the message.
        /// </summary>
        T1 Source { get; }
    }
}
