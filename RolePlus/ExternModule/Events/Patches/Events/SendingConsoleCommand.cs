// -----------------------------------------------------------------------
// <copyright file="SendingConsoleCommand.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using HarmonyLib;

    using RolePlus.ExternModule.API.Engine.Framework.Bootstrap;

    [HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), nameof(RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery))]
    [PatchGroup(nameof(RolePlus))]
    internal static class SendingConsoleCommand
    {
        private static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query)
        {
            (string name, string[] arguments) = query.ExtractCommand();
            HLAPI.LogCommandUsed(__instance, HLAPI.FormatArguments(arguments, 0));
            EventArgs.SendingConsoleCommandEventArgs ev = new(Player.Get(__instance.gameObject), name, arguments.ToList());
            Handlers.Server.OnSendingConsoleCommand(ev);
            if (!string.IsNullOrEmpty(ev.ReturnMessage))
                __instance.GCT.SendToClient(__instance.connectionToClient, ev.ReturnMessage, ev.Color);

            return ev.IsAllowed;
        }
    }
}