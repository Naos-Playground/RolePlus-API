// -----------------------------------------------------------------------
// <copyright file="BypassNoclip.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Commands.Test
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    using global::RolePlus.ExternModule;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class BypassNoclip : ICommand
    {
        /// <inheritdoc/>
        string ICommand.Command => "bypassnoclip";

        /// <inheritdoc/>
        string[] ICommand.Aliases => new string[] { "bpnoclip" };

        /// <inheritdoc/>
        string ICommand.Description => "Adds the player to the noclip whitelist.";

        /// <inheritdoc/>
        bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!HLAPI.ValidateCommandUsage(this, sender, new[] { PlayerPermissions.Noclip }, out response) ||
                !HLAPI.ValidateCommandUsage(this, arguments, new[] { 0 }, out response))
                return false;

            Player target = Player.Get(sender);
            bool removed = HLAPI.IgnoredNoClipPlayers.Remove(target);
            if (!removed)
                HLAPI.IgnoredNoClipPlayers.Add(target);

            response = $"You've been {(removed ? "removed from" : "added to")} the noclip whitelist.";
            return true;
        }
    }
}
