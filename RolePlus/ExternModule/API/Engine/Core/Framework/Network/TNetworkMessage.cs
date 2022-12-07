// -----------------------------------------------------------------------
// <copyright file="TNetworkMessage.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network
{
    using System;
    using System.Collections.Generic;

    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// Object that keeps track of the given data and can be sent across the network.
    /// </summary>
    public class TNetworkMessage<T1, T2> : TNetworkMessage<T1>
    {
        private readonly T2 _default;
        private T2 _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="TNetworkMessage{T1, T2}"/> class.
        /// </summary>
        /// <param name="source">The source to be set.</param>
        /// <param name="tDefault">The deafult value of the content.</param>
        public TNetworkMessage(T1 source, T2 tDefault)
            : base(source)
        {
            _default = tDefault;
            Content = _default;
        }

        /// <inheritdoc/>
        public T2 Content
        {
            get => _content;
            set
            {
                ReceivingNetworkMessageEventArgs<T1, T2> ev = new(this, new TNetworkMessage<T1, T2>(Source, value));
                Events.Handlers.Network.OnReceivingNetworkMessage(ev);
                if (ev.NewMessage is null)
                    return;

                _content = ev.NewMessage.Content;
            }
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        /// <summary>
        /// Sends a <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <typeparam name="T2">The content.</typeparam>
        /// <param name="instance">The instance to modify.</param>
        /// <param name="msg">The new message.</param>
        /// <returns><see langword="true"/> if the message was sent successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Send<T1, T2>(TNetworkMessage<T1, T2> instance, TNetworkMessage<T1, T2> msg)
        {
            ReceivingNetworkMessageEventArgs<T1, T2> ev = new(instance, msg);
            Events.Handlers.Network.OnReceivingNetworkMessage(ev);
            if (!ev.IsAllowed)
                return false;

            instance.Content = msg.Content;

            return true;
        }

        /// <summary>
        /// Sends a <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <typeparam name="T2">The content.</typeparam>
        /// <param name="instance">The instance to modify.</param>
        /// <param name="content">The new content.</param>
        /// <returns><see langword="true"/> if the message was sent successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Send<T1, T2>(TNetworkMessage<T1, T2> instance, T2 content) => Send(instance, new TNetworkMessage<T1, T2>(instance.Source, content));
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type

        /// <summary>
        /// Sends the current <see cref="TNetworkMessage{T1, T2}"/> instance.
        /// </summary>
        /// <param name="instance">The instance to modify.</param>
        /// <returns><see langword="true"/> if the message was sent successfully; otherwise, <see langword="false"/>.</returns>
        public bool Send(TNetworkMessage<T1, T2> instance) => Send(instance, this);

        /// <inheritdoc/>
        public void Reset() => Content = _default;
    }

    /// <summary>
    /// Object that keeps track of the given data and can be sent across the network.
    /// </summary>
    public class TNetworkMessage<T1> : INetworkMessage<T1>
    {
        private static readonly List<TNetworkMessage<T1>> _netPipeline = new();
        private T1 _source;
        private bool _destroyedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TNetworkMessage{T1}"/> class.
        /// </summary>
        /// <param name="source">The source to be set.</param>
        public TNetworkMessage(T1 source)
        {
            _source = source;

            if (!_netPipeline.Contains(this))
            {
                ReceivingPipelineMessageEventArgs<T1> ev = new(this);
                Events.Handlers.Network.OnReceivingPipelineMessage(ev);
                if (!ev.IsAllowed)
                {
                    Destroy();
                    return;
                }
            }

            _netPipeline.Add(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TNetworkMessage{T1}"/> class.
        /// </summary>
        ~TNetworkMessage() => _netPipeline.Remove(this);

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> containing all the active <see cref="TNetworkMessage{T1}"/> instances.
        /// </summary>
        public static IReadOnlyCollection<TNetworkMessage<T1>> NetworkPipeline => _netPipeline.AsReadOnly();

        /// <inheritdoc/>
        public T1 Source
        {
            get => _source;
            protected set => _source = value;
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        /// <summary>
        /// Sends a <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <param name="instance">The instance to modify.</param>
        /// <param name="msg">The new message.</param>
        /// <returns><see langword="true"/> if the message was sent successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Send<T1>(TNetworkMessage<T1> instance, TNetworkMessage<T1> msg)
        {
            ReceivingNetworkMessageEventArgs<T1> ev = new(instance, msg);
            Events.Handlers.Network.OnReceivingNetworkMessage(ev);
            if (!ev.IsAllowed)
                return false;

            instance.Source = msg.Source;

            return true;
        }

        /// <summary>
        /// Sends a <see cref="TNetworkMessage{T1, T2}"/>.
        /// </summary>
        /// <typeparam name="T1">The source.</typeparam>
        /// <param name="instance">The instance to modify.</param>
        /// <returns><see langword="true"/> if the message was sent successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Send<T1>(TNetworkMessage<T1> instance) => Send(instance, new TNetworkMessage<T1>(instance.Source));
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type

        /// <summary>
        /// Fired before the <see cref="TNetworkMessage{T1}"/> is destroyed.
        /// </summary>
        protected virtual void NetworkDestroy()
        {
            _source = default;
            Destroy();
        }

        /// <summary>
        /// Sends the current <see cref="TNetworkMessage{T1, T2}"/> instance.
        /// </summary>
        /// <param name="instance">The instance to modify.</param>
        /// <returns><see langword="true"/> if the message was sent successfully; otherwise, <see langword="false"/>.</returns>
        public bool Send(TNetworkMessage<T1> instance) => Send(instance, this);

        /// <summary>
        /// Destroys the <see cref="TNetworkMessage{T1, T2}"/> instance.
        /// </summary>
        public void Destroy()
        {
            Destroy(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="Destroy()"/>
        private void Destroy(bool destroying)
        {
            if (!_destroyedValue)
            {
                if (destroying)
                {
                    NetworkDestroy();
                    _netPipeline.Remove(this);
                }

                _destroyedValue = true;
            }
        }
    }
}
