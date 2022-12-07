// -----------------------------------------------------------------------
// <copyright file="ChangeLockCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Commands.Round
{
    using System;

    using CommandSystem;

    using global::RolePlus.ExternModule;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class ChangeLockCommand : ICommand
    {
        public static ChangeLockCommand Instance { get; } = new();

        /// <inheritdoc/>
        public string Command => "--r-changelock";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "--r-cl" };

        /// <inheritdoc/>
        public string Description => "Locks the current round.";

        /// <inheritdoc/>
        bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!HLAPI.ValidateCommandUsage(this, sender, "roleplus.round", out response) ||
                !HLAPI.ValidateCommandUsage(this, arguments, new[] { 0 }, out response))
                return false;

            RoundManager.IsLocked = !RoundManager.IsLocked;

            response = $"Round lock state has been changed to: {(RoundManager.IsLocked ? "locked" : "unlocked")}.";
            return true;
        }
    }
}
