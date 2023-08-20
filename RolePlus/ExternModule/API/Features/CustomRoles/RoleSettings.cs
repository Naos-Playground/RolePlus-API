// -----------------------------------------------------------------------
// <copyright file="RoleSettings.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using PlayerRoles;
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;
    using RolePlus.ExternModule.API.Enums;

    /// <summary>
    /// A tool to easily setup roles.
    /// </summary>
    public class RoleSettings : IAddittiveProperty
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
            DoesLookingAffectScp096 = false,
            DoesLookingAffectScp173 = false,
        };

        /// <summary>
        /// Gets or sets a value indicating whether the player's role is dynamic.
        /// </summary>
        public bool IsRoleDynamic { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player's role should use the specified <see cref="Role"/> only.
        /// </summary>
        public bool UseDefaultRoleOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets the player's scale.
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
        public RoomType[] Spawnpoints { get; set; } = new RoomType[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the allowed roles.
        /// </summary>
        public RoleType[] AllowedRoles { get; set; } = new RoleType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the ignored damage types.
        /// </summary>
        public DamageType[] IgnoredDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the allowed damage types.
        /// </summary>
        public DamageType[] AllowedDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all doors that can be bypassed.
        /// </summary>
        public DoorType[] BypassableDoors { get; set; } = new DoorType[] { };

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
        /// Gets or sets a value indicating whether the player can select items from their inventory.
        /// </summary>
        public bool CanSelectItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can look at Scp173.
        /// </summary>
        public bool DoesLookingAffectScp173 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player can trigger Scp096.
        /// </summary>
        public bool DoesLookingAffectScp096 { get; set; } = true;

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        public string RankName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's rank color.
        /// </summary>
        public string RankColor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's custom info.
        /// </summary>
        public string CustomInfo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the player's <see cref="PlayerInfoArea"/> should be hidden.
        /// </summary>
        public bool HideInfoArea { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the C.A.S.S.I.E death announcement can be played when the player dies.
        /// </summary>
        public bool IsDeathAnnouncementEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the C.A.S.S.I.E announcement to be played when the player dies from an unhandled or unknown termination cause.
        /// </summary>
        public string UnknownTerminationCauseAnnouncement { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the player gets killed by a player with the corresponding <see cref="RoleType"/>.
        /// </summary>
        public Dictionary<RoleType, string> KilledByRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the player gets killed by a player with the corresponding <see cref="object"/>.
        /// </summary>
        public Dictionary<object, string> KilledByCustomRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the player gets killed by a player belonging to the corresponding <see cref="Team"/>.
        /// </summary>
        public Dictionary<Team, string> KilledByTeamAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the player gets killed by a player belonging to the corresponding <see cref="object"/>.
        /// </summary>
        public Dictionary<object, string> KilledByCustomTeamAnnouncements { get; set; } = new();
    }
}
