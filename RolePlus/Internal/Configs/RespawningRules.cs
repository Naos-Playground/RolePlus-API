// -----------------------------------------------------------------------
// <copyright file="RespawningRules.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Configs
{
    using System.ComponentModel;
    using global::RolePlus.ExternModule.API.Features.Configs;

    [Config(nameof(RolePlus), nameof(RespawningRules))]
    internal sealed class RespawningRules
    {
        /// <summary>
        /// Gets or sets the respawn cooldown.
        /// </summary>
        [Description("The respawn cooldown.")]
        public int RespawnCooldown { get; set; } = 280;

        /// <summary>
        /// Gets or sets the maximum amount of MTF units that can be spawned at once.
        /// </summary>
        [Description("The maximum amount of MTF units that can be spawned at once.")]
        public uint MaxSpawnableMTFUnits { get; set; } = 12;

        /// <summary>
        /// Gets or sets the maximum amount of Chaos Insurgency units that can be spawned at once.
        /// </summary>
        [Description("The maximum amount of Chaos Insurgency units that can be spawned at once.")]
        public uint MaxSpawnableCHIUnits { get; set; } = 12;
    }
}
