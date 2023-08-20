// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Server;
    using global::RolePlus.ExternModule.API.Enums;

    internal class ServerHandler
    {
        internal void OnWaitingForPlayers() => Server.Host.Role.Set(RoleType.Tutorial);

        internal void OnRoundEnding(EndingRoundEventArgs ev) => ev.IsRoundEnded = !RoundManager.IsLocked;
    }
}
