// -----------------------------------------------------------------------
// <copyright file="OverrideSegmentPatch.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Attributes;

    using HarmonyLib;

    using MEC;

    using NorthwoodLib.Pools;

#pragma warning disable SA1118 // Parameter should not span multiple lines

    [HarmonyPatch(typeof(Timing), nameof(Timing.RunCoroutine), new Type[] { typeof(IEnumerator<float>), typeof(Segment) })]
    [PatchGroup(nameof(RolePlus))]
    internal static class OverrideSegmentPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == AccessTools.Method(typeof(string), "op_Equality"));
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Starg_S, 1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}