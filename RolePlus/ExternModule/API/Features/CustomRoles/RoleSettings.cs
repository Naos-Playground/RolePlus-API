// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using RolePlus.ExternModule.API.Enums;

    /// <summary>
    /// A tool to easily setup custom roles .
    /// </summary>
    public class RoleSettings
    {
        /// <summary>
        /// Gets the default <see cref="RoleSettings"/> values.
        /// <para>It refers to the base-game human roles behavior.</para>
        /// </summary>
        public static RoleSettings Default { get; } = new();

        /// <summary>
        /// Gets the SCP preset referring to the base-game SCPs behavior.
        /// </summary>
        public static RoleSettings ScpPreset { get; } = new()
        {
            CanHurtScps = false,
            CanPlaceBlood = false,
            CanBypassCheckpoints = true,
            CanEnterPocketDimension = false,
            CanUseIntercom = false,
            CanActivateGenerators = false,
            CanActivateWorkstations = false,
            CanBeHandcuffed = false,
            CanSelectItems = false,
            CanDropItems = false,
            CanPickupItems = false,
        };

        /// <summary>
        /// Gets or sets the player's scale. <see langword=""/>
        /// </summary>
        public float Scale { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the player's health.
        /// </summary>
        public float Health { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the player's max health.
        /// </summary>
        public int MaxHealth { get; set; } = 100;

        /// <summary>
        /// Gets or sets the player's artificial health.
        /// </summary>
        public float ArtificialHealth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the player's max artificial health.
        /// </summary>
        public float MaxArtificialHealth { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the broadcast to be displayed as soon as the player spawns.
        /// </summary>
        public Broadcast Broadcast { get; set; }

        /// <summary>
        /// Gets or sets the role's spawn points.
        /// </summary>
        public List<RoomType> Spawnpoints { get; set; }

        /// <summary>
        /// Gets a <see cref="RoleType"/>[] containing all the allowed roles.
        /// </summary>
        public RoleType[] AllowedRoles { get; } = new RoleType[] { };

        /// <summary>
        /// Gets a <see cref="DamageType"/>[] containing all the ignored damage types.
        /// </summary>
        public DamageType[] IgnoredDamageTypes { get; } = new DamageType[] { };

        /// <summary>
        /// Gets a <see cref="DamageType"/>[] containing all the allowed damage types.
        /// </summary>
        public DamageType[] AllowedDamageTypes { get; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a value indicating whether the player's role is dynamic.
        /// </summary>
        public bool IsRoleDynamic { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player can hurt SCPs.
        /// </summary>
        public bool CanHurtScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can be hurted by SCPs.
        /// </summary>
        public bool CanBeHurtByScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can enter pocket dimension.
        /// </summary>
        public bool CanEnterPocketDimension { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can use intercom.
        /// </summary>
        public bool CanUseIntercom { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can voicechat.
        /// </summary>
        public bool CanUseVoiceChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can place blood.
        /// </summary>
        public bool CanPlaceBlood { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can be handcuffed.
        /// </summary>
        public bool CanBeHandcuffed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can use elevators.
        /// </summary>
        public bool CanUseElevators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can bypass checkpoints.
        /// </summary>
        public bool CanBypassCheckpoints { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player can activate warhead.
        /// </summary>
        public bool CanActivateWarhead { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can activate workstations.
        /// </summary>
        public bool CanActivateWorkstations { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can activate generators.
        /// </summary>
        public bool CanActivateGenerators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can pickup items.
        /// </summary>
        public bool CanPickupItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can drop items.
        /// </summary>
        public bool CanDropItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can select items from its inventory.
        /// </summary>
        public bool CanSelectItems { get; set; } = true;
    }
}
