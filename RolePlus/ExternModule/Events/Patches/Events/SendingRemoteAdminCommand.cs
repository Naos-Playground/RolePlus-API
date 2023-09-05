// -----------------------------------------------------------------------
// <copyright file="SendingRemoteAdminCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;

    using RemoteAdmin;

    // [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), typeof(string), typeof(CommandSender))]
    [PatchGroup(nameof(RolePlus))]
    internal static class SendingRemoteAdminCommand
    {
        private static bool Prefix(ref string q, ref CommandSender sender)
        {
            QueryProcessor queryProcessor = null; /*sender is PlayerCommandSender playerCommandSender ? playerCommandSender.q : null;*/
            (string name, string[] arguments) = q.ExtractCommand();
            HLAPI.LogCommandUsed(queryProcessor, HLAPI.FormatArguments(arguments, 0));

            EventArgs.SendingRemoteAdminCommandEventArgs ev =
                new(sender, string.IsNullOrEmpty(sender.SenderId) ? Server.Host : Player.Get(sender.SenderId) ?? Server.Host, name, arguments.ToList());

            IdleMode.PreauthStopwatch.Restart();
            IdleMode.SetIdleMode(false);

            if (q.StartsWith("gban-kick", System.StringComparison.OrdinalIgnoreCase))
            {
                if (queryProcessor == null || !queryProcessor._sender.ServerRoles.RaEverywhere || !queryProcessor._sender.ServerRoles.Staff)
                {
                    sender.RaReply(
                        $"GBAN-KICK# Permission to run command denied by the server. If this is an unexpected error, contact RolePlus developers.",
                        false,
                        true,
                        string.Empty);

                    Log.Error($"A user {sender.Nickname} attempted to run GBAN-KICK and was denied permission. If this is an unexpected error, contact RolePlus developers.");

                    ev.IsAllowed = false;
                }
            }

            if (q == "REQUEST_DATA PLAYER_LIST SILENT")
                return true;

            _ = Handlers.Server.SendingRemoteAdminCommandDispatcher[ev];

            if (!string.IsNullOrEmpty(ev.ReplyMessage))
                sender.RaReply(ev.ReplyMessage, ev.Success, true, string.Empty);

            return ev.IsAllowed;
        }
    }
}