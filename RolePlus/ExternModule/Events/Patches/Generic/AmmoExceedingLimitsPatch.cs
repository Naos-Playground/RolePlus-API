// -----------------------------------------------------------------------
// <copyright file="AmmoExceedingLimitsPatch.cs" company="NaoUnderscore">
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

    // [HarmonyPatch(typeof(Firearm), nameof(Firearm.OnHolstered))]
    internal static class AmmoExceedingLimitsPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt) + offset;

            newInstructions.RemoveRange(index, 15);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}