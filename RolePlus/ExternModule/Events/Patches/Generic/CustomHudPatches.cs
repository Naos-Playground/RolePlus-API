﻿// -----------------------------------------------------------------------
// <copyright file="CustomHudPatches.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RolePlus.ExternModule.API.Engine.Framework.Bootstrap;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomHud;

    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(Player), nameof(Player.ShowHint))]
    [PatchGroup(nameof(RolePlus))]
    internal static class CustomHudPatches
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.Clear();
#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.NetworkIdentity))),
                new(OpCodes.Brfalse, ret),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.GameObject))),
                new(OpCodes.Brfalse, ret),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldc_I4_S, (int)ScreenLocation.MiddleBottom),
                new(OpCodes.Call, Method(typeof(HudManager), nameof(HudManager.ShowHint))),
                new CodeInstruction(OpCodes.Ret).WithLabels(ret),
            });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}