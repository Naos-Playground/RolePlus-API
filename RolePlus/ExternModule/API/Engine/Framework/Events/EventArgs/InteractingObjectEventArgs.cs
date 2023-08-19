// -----------------------------------------------------------------------
// <copyright file="InteractingObjectEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using RolePlus.ExternModule.API.Engine.Components;

    /// <summary>
    /// Contains all the information before an object interaction occurs.
    /// </summary>
    public class InteractingObjectEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingObjectEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="interactableFrame"><inheritdoc cref="InteractableFrame"/></param>
        public InteractingObjectEventArgs(Player player, AInteractableFrameComponent interactableFrame)
        {
            Player = player;
            InteractableFrame = interactableFrame;
        }

        /// <summary>
        /// Gets the player who's trying to interact with the <see cref="InteractableFrame"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="AInteractableFrameComponent"/>.
        /// </summary>
        public AInteractableFrameComponent InteractableFrame { get; }
    }
}
