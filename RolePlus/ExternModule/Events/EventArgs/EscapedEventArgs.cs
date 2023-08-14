// -----------------------------------------------------------------------
// <copyright file="EscapedEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before invoking an event.
    /// </summary>
    public sealed class EscapedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public EscapedEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }
    }
}
