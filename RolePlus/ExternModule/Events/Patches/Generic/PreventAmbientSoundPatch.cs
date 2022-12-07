// -----------------------------------------------------------------------
// <copyright file="PreventAmbientSoundPatch.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RolePlus.ExternModule.API.Engine.Framework.Bootstrap;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1118 // Parameter should not span multiple lines

    [HarmonyPatch(typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.GenerateRandom))]
    [PatchGroup(nameof(RolePlus))]
    internal static class PreventAmbientSoundPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label cdc = generator.DefineLabel();

            newInstructions[0].labels.Add(cdc);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, Method(typeof(RoundSummary), nameof(RoundSummary.RoundInProgress))),
                new(OpCodes.Brtrue_S, cdc),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}