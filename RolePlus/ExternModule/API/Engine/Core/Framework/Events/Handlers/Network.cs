// -----------------------------------------------------------------------
// <copyright file="Network.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.Handlers
{
    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;
    using RolePlus.ExternModule.API.Engine.Framework.Network;

    using static RolePlus.ExternModule.API.Engine.Framework.Events.Delegates;

#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// Handles all the network events.
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// Called before receiving a <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <typeparam name="T2">The content.</typeparam>
        /// <param name="ev">The <see cref="ReceivingNetworkMessageEventArgs{T1, T2}"/> instance.</param>
        public static void OnReceivingNetworkMessage<T1, T2>(ReceivingNetworkMessageEventArgs<T1, T2> ev) => MultiPlex<T1, T2>.OnReceivingNetworkMessage(ev);

        /// <summary>
        /// Called before receiving a <see cref="TNetworkMessage{T1}"/>.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <param name="ev">The <see cref="ReceivingNetworkMessageEventArgs{T1}"/> instance.</param>
        public static void OnReceivingNetworkMessage<T1>(ReceivingNetworkMessageEventArgs<T1> ev) => Plex<T1>.OnReceivingNetworkMessage(ev);

        /// <summary>
        /// Called before a <see cref="TNetworkMessage{T1}"/> reaches the pipeline.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <param name="ev">The <see cref="ReceivingPipelineMessageEventArgs{T1}"/> instance.</param>
        public static void OnReceivingPipelineMessage<T1>(ReceivingPipelineMessageEventArgs<T1> ev) => Plex<T1>.OnReceivingPipelineMessage(ev);

        /// <summary>
        /// Handles all the network events which require a generic type.
        /// </summary>
        public static class Plex<T1>
        {
            /// <summary>
            /// Invoked before receiving a <see cref="TNetworkMessage{T1}"/>.
            /// </summary>
            public static event TEventHandler<ReceivingNetworkMessageEventArgs<T1>> ReceivingNetworkMessage;

            /// <summary>
            /// Invoked before receiving a <see cref="TNetworkMessage{T1, T2}"/>.
            /// </summary>
            public static event TEventHandler<ReceivingPipelineMessageEventArgs<T1>> ReceivingPipelineMessage;

            /// <summary>
            /// Called before receiving a <see cref="TNetworkMessage{T1}"/>.
            /// </summary>
            /// <param name="ev">The <see cref="ReceivingNetworkMessageEventArgs{T1}"/> instance.</param>
            public static void OnReceivingNetworkMessage(ReceivingNetworkMessageEventArgs<T1> ev) => ReceivingNetworkMessage.InvokeSafely(ev);

            /// <summary>
            /// Called before a <see cref="TNetworkMessage{T1}"/> reaches the pipeline.
            /// </summary>
            /// <param name="ev">The <see cref="ReceivingPipelineMessageEventArgs{T1}"/> instance.</param>
            public static void OnReceivingPipelineMessage(ReceivingPipelineMessageEventArgs<T1> ev) => ReceivingPipelineMessage.InvokeSafely(ev);
        }

        /// <summary>
        /// Handles all the network events which require two generic types.
        /// </summary>
        public static class MultiPlex<T1, T2>
        {
            /// <summary>
            /// Invoked before receiving a <see cref="TNetworkMessage{T1, T2}"/>.
            /// </summary>
            public static event TEventHandler<ReceivingNetworkMessageEventArgs<T1, T2>> ReceivingNetworkMessage;

            /// <summary>
            /// Called before receiving a <see cref="TNetworkMessage{T1, T2}"/>.
            /// </summary>
            /// <param name="ev">The <see cref="ReceivingNetworkMessageEventArgs{T1, T2}"/> instance.</param>
            public static void OnReceivingNetworkMessage(ReceivingNetworkMessageEventArgs<T1, T2> ev) => ReceivingNetworkMessage.InvokeSafely(ev);
        }
    }
}
