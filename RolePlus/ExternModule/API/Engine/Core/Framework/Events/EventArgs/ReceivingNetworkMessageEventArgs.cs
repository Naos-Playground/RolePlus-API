// -----------------------------------------------------------------------
// <copyright file="ReceivingNetworkMessageEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs
{
    using RolePlus.ExternModule.API.Engine.Framework.Network;

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// Contains all the information before a <see cref="TNetworkMessage{T1, T2}"/> is sent.
    /// </summary>
    public class ReceivingNetworkMessageEventArgs<T1, T2> : MultiPlexEventArgs<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingNetworkMessageEventArgs{T1, T2}"/> class.
        /// </summary>
        /// <param name="oldMsg"><inheritdoc cref="NewMessage"/></param>
        /// <param name="newMsg"><inheritdoc cref="OldMessage"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ReceivingNetworkMessageEventArgs{T1}.IsAllowed"/></param>
        public ReceivingNetworkMessageEventArgs(TNetworkMessage<T1, T2> oldMsg, TNetworkMessage<T1, T2> newMsg, bool isAllowed = true)
        {
            NewMessage = newMsg;
            OldMessage = oldMsg;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the received <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        public TNetworkMessage<T1, T2> NewMessage { get; }

        /// <summary>
        /// Gets the old <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        public TNetworkMessage<T1, T2> OldMessage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the message can be sent.
        /// </summary>
        public bool IsAllowed { get; set; }
    }

    /// <summary>
    /// Contains all the information before a <see cref="TNetworkMessage{T1, T2}"/> is sent.
    /// </summary>
    public class ReceivingNetworkMessageEventArgs<T1> : PlexEventArgs<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingNetworkMessageEventArgs{T1}"/> class.
        /// </summary>
        protected ReceivingNetworkMessageEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingNetworkMessageEventArgs{T1}"/> class.
        /// </summary>
        /// <param name="oldMsg"><inheritdoc cref="NewMessage"/></param>
        /// <param name="newMsg"><inheritdoc cref="OldMessage"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReceivingNetworkMessageEventArgs(TNetworkMessage<T1> oldMsg, TNetworkMessage<T1> newMsg, bool isAllowed = true)
        {
            NewMessage = newMsg;
            OldMessage = oldMsg;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the received <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        public TNetworkMessage<T1> NewMessage { get; }

        /// <summary>
        /// Gets the old <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        public TNetworkMessage<T1> OldMessage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the message can be sent.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
