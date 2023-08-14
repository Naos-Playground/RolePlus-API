// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="NaoUnderscore">
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
    public static class Server
    {
        /// <summary>
        /// Invoked before invoking an event.
        /// </summary>
        public static event TEventHandler<InvokingHandlerEventArgs> InvokingHandler;

        /// <summary>
        /// Invoked before sending a console command.
        /// </summary>
        public static event TEventHandler<SendingConsoleCommandEventArgs> SendingConsoleCommand;

        /// <summary>
        /// Invoked before sending a RA command.
        /// </summary>
        public static event TEventHandler<SendingRemoteAdminCommandEventArgs> SendingRemoteAdminCommand;

        /// <summary>
        /// Called before invoking an event.
        /// </summary>
        /// <param name="ev">The <see cref="InvokingHandlerEventArgs"/> instance.</param>
        public static void OnInvokingHandler(InvokingHandlerEventArgs ev) => InvokingHandler.InvokeSafely(ev);

        /// <summary>
        /// Called before sending a console command.
        /// </summary>
        /// <param name="ev">The <see cref="SendingConsoleCommandEventArgs"/> instance.</param>
        public static void OnSendingConsoleCommand(SendingConsoleCommandEventArgs ev) => SendingConsoleCommand.InvokeSafely(ev);

        /// <summary>
        /// Called before sending a RA command.
        /// </summary>
        /// <param name="ev">The <see cref="SendingRemoteAdminCommandEventArgs"/> instance.</param>
        public static void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev) => SendingRemoteAdminCommand.InvokeSafely(ev);
    }
}