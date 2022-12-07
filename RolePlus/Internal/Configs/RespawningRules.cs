// -----------------------------------------------------------------------
// <copyright file="RespawningRules.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Configs
{
    using System.Collections.Generic;

    using global::RolePlus.ExternModule.API.Features.Configs;

    [Config(nameof(RolePlus), nameof(RespawningRules))]
    internal sealed class RespawningRules
    {
        public int RespawnCooldown { get; set; } = 280;

        public uint MaxSpawnableMTFUnits { get; set; } = 12;

        public uint MaxSpawnableCHIUnits { get; set; } = 12;

        public Dictionary<string, string> AllUnitNames { get; set; } = new();
    }
}
