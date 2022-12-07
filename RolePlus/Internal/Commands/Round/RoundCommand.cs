// -----------------------------------------------------------------------
// <copyright file="RoundCommand.cs" company="NaoUnderscore">
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
    internal class RoundCommand : ParentCommand
    {
        public RoundCommand() => LoadGeneratedCommands();

        public static RoundCommand Instance { get; } = new();

        /// <inheritdoc/>
        public override string Command => "round";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => string.Empty;

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(ChangeLockCommand.Instance);
            RegisterCommand(ForceEndCommand.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response) => HLAPI.InvalidateCommandUsage(this, out response);
    }
}
