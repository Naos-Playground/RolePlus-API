// -----------------------------------------------------------------------
// <copyright file="ServerSendingConsoleCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Attributes;

    using HarmonyLib;

    using RemoteAdmin;

    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    [PatchGroup(nameof(RolePlus))]
    internal static class ServerSendingConsoleCommand
    {
        internal static bool Prefix(QueryProcessor __instance, string query)
        {
            (string name, string[] arguments) = query.ExtractCommand();
            HLAPI.LogCommandUsed(__instance, HLAPI.FormatArguments(arguments, 0));
            Exiled.API.Features.Player player = Exiled.API.Features.Player.Get(__instance._sender);
            EventArgs.SendingConsoleCommandEventArgs ev = new(player ?? Exiled.API.Features.Server.Host, name, arguments.ToList());
            Handlers.Server.OnSendingConsoleCommand(ev);
            return ev.IsAllowed;
        }
    }
}