// -----------------------------------------------------------------------
// <copyright file="Handlers.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Handlers
{
    using Exiled.Events.Extensions;

    using RolePlus.ExternModule.API.Engine.Framework.Events;
    using RolePlus.ExternModule.Events.EventArgs;

    using static RolePlus.ExternModule.API.Engine.Framework.Events.Delegates;

    /// <summary>
    /// Server related events.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Invoked before escaping.
        /// </summary>
        public static event TEventHandler<EscapingEventArgs> Escaping;

        /// <summary>
        /// Invoked before escaping.
        /// </summary>
        public static event TEventHandler<EscapedEventArgs> Escaped;

        /// <summary>
        /// Called before escaping.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public static void OnEscaping(EscapingEventArgs ev) => Escaping.InvokeSafely(ev);

        /// <summary>
        /// Called before escaping.
        /// </summary>
        /// <param name="ev">The <see cref="EscapedEventArgs"/> instance.</param>
        public static void OnEscaped(EscapedEventArgs ev) => Escaped.InvokeSafely(ev);
    }
}