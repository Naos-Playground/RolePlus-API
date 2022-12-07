// -----------------------------------------------------------------------
// <copyright file="MainCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Commands
{
    using System;

    using CommandSystem;

    using global::RolePlus.ExternModule;
    using global::RolePlus.Internal.Commands.Round;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class MainCommand : ParentCommand
    {
        public MainCommand() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "roleplus";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "rlps", "rp", };

        /// <inheritdoc/>
        public override string Description => string.Empty;

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(RoundCommand.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response) => HLAPI.InvalidateCommandUsage(this, out response);
    }
}
