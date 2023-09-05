// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using global::RolePlus.ExternModule.API.Engine.Framework;
    using global::RolePlus.ExternModule.API.Enums;
    using global::RolePlus.ExternModule.API.Features;

    internal class ServerHandler
    {
        internal void OnVerified(VerifiedEventArgs ev)
        {
            Player.Dictionary.Remove(ev.Player.GameObject);
            Player.Dictionary[ev.Player.GameObject] = new Pawn(ev.Player.ReferenceHub);
        }

        internal void OnWaitingForPlayers() => Server.Host.Role.Set(RoleType.Tutorial);

        internal void OnRoundEnding(EndingRoundEventArgs ev) => ev.IsRoundEnded = !StaticActor.Get<RoundManager>().IsLocked;
    }
}
