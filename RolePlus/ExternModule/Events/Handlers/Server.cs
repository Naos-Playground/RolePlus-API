// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Handlers
{
    using RolePlus.ExternModule.Events.DynamicEvents;
    using RolePlus.ExternModule.Events.EventArgs;

    /// <summary>
    /// Server related events.
    /// </summary>
    public static class Server
    {
        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which is fired before invoking an event.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<InvokingHandlerEventArgs> InvokingHandlerDispatcher { get; } = new();

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which is fired before sending a console command.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<SendingConsoleCommandEventArgs> SendingConsoleCommandDispatcher { get; } = new();

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which is fired before sending a remote admin console command.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<SendingRemoteAdminCommandEventArgs> SendingRemoteAdminCommandDispatcher { get; } = new();
    }
}