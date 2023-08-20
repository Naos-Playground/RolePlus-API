// -----------------------------------------------------------------------
// <copyright file="TemporaryStats.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;

    using MEC;
    using RolePlus.ExternModule.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A tool to keep track of temporary <see cref="Player"/>'s properties.
    /// </summary>
    public sealed class TemporaryStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryStats"/> class.
        /// </summary>
        public TemporaryStats()
        {
        }

        /// <inheritdoc cref="TemporaryStats()"/>
        public TemporaryStats(Player player)
        {
            TemporaryStats customPlayerStats = new(
                player.Health,
                player.MaxHealth,
                player.ArtificialHealth,
                player.MaxArtificialHealth,
                RoleType.Cast(player.Role.Type),
                player.GetCustomRoleType(),
                player.Position,
                player.Rotation,
                player.Scale,
                player.Nickname,
                player.CustomInfo,
                player.RankName,
                player.RankColor,
                player.BadgeHidden,
                player.IsOverwatchEnabled,
                player.IsBypassModeEnabled,
                player.IsGodModeEnabled,
                player.IsUsingStamina,
                player.IsMuted,
                player.IsIntercomMuted,
                player.Items.GetItemTypes(),
                player.GetCustomItems().Select(item => (object)item.Id),
                player.GetEffectTypes());

            if (player.Role.Is(out Scp079Role scp079Role))
            {
                customPlayerStats.Energy = scp079Role.Energy;
                customPlayerStats.MaxEnergy = scp079Role.MaxEnergy;
                customPlayerStats.Level = scp079Role.Level;
                customPlayerStats.Experience = scp079Role.Experience;
            }
        }

        /// <inheritdoc cref="TemporaryStats()"/>
        public TemporaryStats(
            float health = 0f,
            float maxHealth = 0,
            float artificialHealth = 0f,
            float maxArtificialHealth = 0f,
            RoleType role = null,
            object customRole = null,
            Vector3 position = default,
            Vector2 rotation = default,
            Vector3 scale = default,
            string nickname = "",
            string customInfo = "",
            string rankName = "",
            string rankColor = "",
            bool badgeHidden = false,
            bool isOverwatchEnabled = false,
            bool isBypassModeEnabled = false,
            bool isGodModeEnabled = false,
            bool isUsingStamina = true,
            bool isMuted = false,
            bool isIntercomMuted = false,
            IEnumerable<ItemType> inventory = null,
            IEnumerable<object> customItems = null,
            IEnumerable<EffectType> effects = null)
        {
            Health = health;
            MaxHealth = maxHealth;
            Role = role;
            CustomRole = customRole;
            ArtificialHealth = artificialHealth;
            MaxArtificialHealth = maxArtificialHealth;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Nickname = nickname;
            CustomInfo = customInfo;
            RankName = rankName;
            RankColor = rankColor;
            BadgeHidden = badgeHidden;
            IsOverwatchEnabled = isOverwatchEnabled;
            IsBypassModeEnabled = isBypassModeEnabled;
            IsGodModeEnabled = isGodModeEnabled;
            IsUsingStamina = isUsingStamina;
            IsMuted = isMuted;
            IsIntercomMuted = isIntercomMuted;
            Inventory = inventory;
            CustomInventory = customItems;
            Effects = effects;
        }

        /// <inheritdoc cref="Player.Health"/>
        public float Health { get; set; }

        /// <inheritdoc cref="Player.MaxHealth"/>
        public float MaxHealth { get; set; }

        /// <inheritdoc cref="Player.ArtificialHealth"/>
        public float ArtificialHealth { get; set; }

        /// <inheritdoc cref="Player.MaxArtificialHealth"/>
        public float MaxArtificialHealth { get; set; }

        /// <inheritdoc cref="Role.Type"/>
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the player's custom role.
        /// </summary>
        public object CustomRole { get; set; }

        /// <inheritdoc cref="Player.Items"/>
        public IEnumerable<ItemType> Inventory { get; set; }

        /// <summary>
        /// Gets or sets the player's custom items.
        /// </summary>
        public IEnumerable<object> CustomInventory { get; set; }

        /// <inheritdoc cref="Player.ActiveEffects"/>
        public IEnumerable<EffectType> Effects { get; set; }

        /// <inheritdoc cref="Player.Position"/>
        public Vector3 Position { get; set; }

        /// <inheritdoc cref="Player.Rotation"/>
        public Vector2 Rotation { get; set; }

        /// <inheritdoc cref="Player.Scale"/>
        public Vector3 Scale { get; set; }

        /// <inheritdoc cref="Player.CurrentItem"/>
        public ItemType CurrentItem { get; set; }

        /// <inheritdoc cref="Player.Nickname"/>
        public string Nickname { get; set; }

        /// <inheritdoc cref="Player.CustomInfo"/>
        public string CustomInfo { get; set; }

        /// <inheritdoc cref="Player.IsOverwatchEnabled"/>
        public bool IsOverwatchEnabled { get; set; }

        /// <inheritdoc cref="Player.IsBypassModeEnabled"/>
        public bool IsBypassModeEnabled { get; set; }

        /// <inheritdoc cref="Player.IsMuted"/>
        public bool IsMuted { get; set; }

        /// <inheritdoc cref="Player.IsIntercomMuted"/>
        public bool IsIntercomMuted { get; set; }

        /// <inheritdoc cref="Player.IsGodModeEnabled"/>
        public bool IsGodModeEnabled { get; set; }

        /// <inheritdoc cref="Scp079Role.Experience"/>
        public float Experience { get; set; }

        /// <inheritdoc cref="Scp079Role.Level"/>
        public int Level { get; set; }

        /// <inheritdoc cref="Scp079Role.MaxEnergy"/>
        public float MaxEnergy { get; set; }

        /// <inheritdoc cref="Scp079Role.Energy"/>
        public float Energy { get; set; }

        /// <inheritdoc cref="Scp079Role.Camera"/>
        public Exiled.API.Features.Camera Camera { get; set; }

        /// <inheritdoc cref="Player.RankName"/>
        public string RankName { get; set; }

        /// <inheritdoc cref="Player.RankColor"/>
        public string RankColor { get; set; }

        /// <inheritdoc cref="Player.BadgeHidden"/>
        public bool BadgeHidden { get; set; }

        /// <inheritdoc cref="Player.IsUsingStamina"/>
        public bool IsUsingStamina { get; set; }

        /// <summary>
        /// Applies the current <see cref="TemporaryStats"/> instance to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The target.</param>
        /// <param name="newInventory">A value indicating whether the inventory should be reset.</param>
        /// <param name="lite">A value indicating whether the player should preserve the same vectors.</param>
        public void Apply(Player player, bool newInventory = true, bool lite = true)
        {
            bool flag = CustomRole is not null;

            if (flag)
                player.SetRole(CustomRole, lite);
            else
                player.SetRole(Role, lite);

            Timing.CallDelayed(flag ? 1.5f : 0.5f, () =>
            {
                player.Health = Health;
                player.MaxHealth = MaxHealth;
                player.ArtificialHealth = ArtificialHealth;
                player.MaxArtificialHealth = MaxArtificialHealth;

                if (newInventory && !player.IsScp)
                {
                    player.ResetInventory(Inventory);
                    Timing.CallDelayed(1f, () => player.AddItem(CustomInventory));
                }
            });
        }
    }
}