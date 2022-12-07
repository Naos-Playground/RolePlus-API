// -----------------------------------------------------------------------
// <copyright file="FixDefaultSegmentPatch.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using MEC;

    using NorthwoodLib.Pools;

    using RolePlus.ExternModule.API.Engine.Framework.Bootstrap;

    [HarmonyPatch(typeof(Timing), nameof(Timing.RunCoroutine), new Type[] { typeof(IEnumerator<float>) })]
    [PatchGroup(nameof(RolePlus))]
    internal static class FixDefaultSegmentPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Find(x => x.opcode == OpCodes.Ldc_I4_0).opcode = OpCodes.Ldc_I4_1;

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}