// -----------------------------------------------------------------------
// <copyright file="NetworkAuthority.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network
{
    using System;

    /// <summary>
    /// All the network authority flags.
    /// </summary>
    [Flags]
    public enum ENetworkAuthority : byte
    {
        /// <summary>
        /// Defines a none authority.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Defines a threat authority.
        /// </summary>
        Threat = 0x1,

        /// <summary>
        /// Defines a user authority.
        /// </summary>
        User = 0x02,

        /// <summary>
        /// Defines a server authority.
        /// </summary>
        Server = 0x04,

        /// <summary>
        /// Defines a network authority.
        /// </summary>
        Network = 0x08,
    }
}
