// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.Handlers
{
    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;

    using static RolePlus.ExternModule.API.Engine.Framework.Events.Delegates;

    /// <summary>
    /// Handles all the network events.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Invoked before an object interaction occurs.
        /// </summary>
        public static event TEventHandler<InteractingObjectEventArgs> InteractingObject;

        /// <summary>
        /// Called before an object interaction occurs.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingObjectEventArgs"/> instance.</param>
        public static void OnInteractingObject(InteractingObjectEventArgs ev) => InteractingObject.InvokeSafely(ev);
    }
}
