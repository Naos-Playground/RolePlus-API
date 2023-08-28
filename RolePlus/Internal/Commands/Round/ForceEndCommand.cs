// -----------------------------------------------------------------------
// <copyright file="ForceEndCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Commands.Round
{
    using System;

    using CommandSystem;

    using global::RolePlus.ExternModule;
    using global::RolePlus.ExternModule.API.Engine.Framework;
    using global::RolePlus.ExternModule.API.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class ForceEndCommand : ICommand
    {
        public static ForceEndCommand Instance { get; } = new();

        /// <inheritdoc/>
        public string Command => "--r-forceend";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "--r-fe", "--r-endround", "--r-er" };

        /// <inheritdoc/>
        public string Description => "Forces the current round to end.";

        /// <inheritdoc/>
        bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!HLAPI.ValidateCommandUsage(this, sender, "roleplus.round", out response) ||
                !HLAPI.ValidateCommandUsage(this, arguments, new[] { 0 }, out response))
                return false;

            StaticActor.Get<RoundManager>().EndRound();

            response = "Round ended.";
            return true;
        }
    }
}
