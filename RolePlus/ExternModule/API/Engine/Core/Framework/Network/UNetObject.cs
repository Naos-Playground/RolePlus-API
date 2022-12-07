// -----------------------------------------------------------------------
// <copyright file="UNetObject.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using RolePlus.ExternModule.API.Engine.Core;

    /// <summary>
    /// The base class of all network objects.
    /// </summary>
    public abstract class UNetObject : UObject
    {
        private static readonly HashSet<TNetworkMessage<UNetObject>> _networkMessages = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UNetObject"/> class.
        /// </summary>
        protected UNetObject()
            : base()
        {
            IsEditable = false;
            Base = Server.Host.GameObject;
            NetworkAuthority = ENetworkAuthority.None;

            TNetworkMessage<UNetObject> msg = new(this);
            if (msg is null)
            {
                Destroy();
                return;
            }

            _networkMessages.Add(msg);
        }

        /// <summary>
        /// Gets the cached <see cref="IReadOnlyCollection{T}"/> of <see cref="TNetworkMessage{T1}"/>.
        /// </summary>
        public static IReadOnlyCollection<TNetworkMessage<UNetObject>> NetworkMessages => _networkMessages.ToList().AsReadOnly();

        /// <summary>
        /// Gets or sets the <see cref="ENetworkAuthority"/>.
        /// </summary>
        public ENetworkAuthority NetworkAuthority { get; protected set; }
    }
}
