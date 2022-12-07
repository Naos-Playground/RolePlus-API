// -----------------------------------------------------------------------
// <copyright file="TeleportCommand.cs" company="NaoUnderscore">
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
    internal class TeleportCommand : ICommand
    {
        /// <inheritdoc/>
        string ICommand.Command => "teleport";

        /// <inheritdoc/>
        string[] ICommand.Aliases => new string[] { "tp" };

        /// <inheritdoc/>
        string ICommand.Description => "Telports to a specified position.";

        /// <inheritdoc/>
        bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!HLAPI.ValidateCommandUsage(this, sender, new[] { PlayerPermissions.Noclip }, out response) ||
                !HLAPI.ValidateCommandUsage(this, arguments, new[] { 3 }, out response))
                return false;

            if (!float.TryParse(arguments.At(0), out float x) || !float.TryParse(arguments.At(1), out float y) || !float.TryParse(arguments.At(2), out float z))
                return HLAPI.InvalidateCommandUsage(this, "Invalid arguments", out response);

            Player target = Player.Get((CommandSender)sender);
            target.Position = new(x, y, z);

            response = $"Player \"{target.Nickname}\" has been teleported to {target.Position}.";
            return true;
        }
    }
}
