// -----------------------------------------------------------------------
// <copyright file="ShowTpsCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Commands.Test
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class ShowTpsCommand : ICommand
    {
        /// <inheritdoc/>
        string ICommand.Command => "showtps";

        /// <inheritdoc/>
        string[] ICommand.Aliases => new string[] { "tps" };

        /// <inheritdoc/>
        string ICommand.Description => "Show the TPS.";

        /// <inheritdoc/>
        bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = Server.Tps.ToString();
            return true;
        }
    }
}
