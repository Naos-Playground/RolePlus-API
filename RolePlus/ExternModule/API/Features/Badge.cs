// -----------------------------------------------------------------------
// <copyright file="Badge.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    /// <summary>
    /// A tool to easily manage in-game badges.
    /// </summary>
    public class Badge
    {
        private readonly string _oldRank;
        private readonly string _oldColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Badge"/> class.
        /// </summary>
        /// <param name="player">The owner of the badge.</param>
        /// <param name="rank">The rank to be set.</param>
        /// <param name="color">The color of the rank.</param>
        public Badge(Player player, string rank, string color)
        {
            try
            {
                Owner = player ?? throw new NullReferenceException("Badge::.ctor: Player is NULL.");
                Rank = rank;
                Color = color;
                _oldRank = string.IsNullOrEmpty(Owner.RankName) ? string.Empty : Owner.RankName;
                _oldColor = string.IsNullOrEmpty(Owner.RankColor) ? "default" : Owner.RankColor;
                IsHidden = player.BadgeHidden;

                if (TryGet(Owner, out Badge badge))
                {
                    badge.Unload();
                    BadgeValues.Remove(badge);
                }

                BadgeValues.Add(this);
            }
            catch (Exception ex)
            {
                Log.Debug($"Couldn't initialize in-game badge instance\nException: {ex}");
            }
        }

        internal static HashSet<Badge> BadgeValues { get; set; } = new();

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/>.
        /// </summary>
        public static HashSet<Badge> List => BadgeValues;

        /// <summary>
        /// Gets the owner of the badge.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets the badge content.
        /// </summary>
        public string Rank { get; }

        /// <summary>
        /// Gets the color of the content.
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the badge is hidden.
        /// </summary>
        public bool IsHidden
        {
            get => Owner.BadgeHidden;
            set
            {
                Owner.BadgeHidden = value;

                if (value)
                    Hide();
                else
                    Reload();
            }
        }

        /// <summary>
        /// Gets a <see cref="Badge"/> belonging to a <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>The belonging <see cref="Badge"/> object or <see langword="default"/> if not found.</returns>
        public static Badge Get(Player player) => List.FirstOrDefault(badge => badge.Owner == player);

        /// <summary>
        /// Tries to get a <see cref="Badge"/> belonging to a <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="badge">The belonging <see cref="Badge"/> object or <see langword="default"/> if not found.</param>
        /// <returns><see langword="true"/> if the badge was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player player, out Badge badge)
        {
            badge = Get(player);
            return badge.Owner is null;
        }

        /// <summary>
        /// Hides the badge.
        /// </summary>
        /// <param name="player">The owner of the <see cref="Badge"/>.</param>
        /// <returns><see langword="true"/> if the owner's badge was hidden successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Hide(Player player)
        {
            Badge badge = Get(player);

            if (badge.Owner is null)
                return false;

            badge.Hide();

            return true;
        }

        /// <summary>
        /// Reloads the badge.
        /// </summary>
        /// <param name="player">The owner of the <see cref="Badge"/>.</param>
        /// <returns><see langword="true"/> if the owner's badge was reloaded successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Reload(Player player)
        {
            Badge badge = Get(player);

            if (badge.Owner is null)
                return false;

            badge.Reload();

            return true;
        }

        /// <summary>
        /// Unloads the badge.
        /// </summary>
        /// <param name="player">The owner of the <see cref="Badge"/>.</param>
        /// <returns><see langword="true"/> if the owner's badge was unloaded successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Unload(Player player)
        {
            Badge badge = Get(player);

            if (badge.Owner is null)
                return false;

            badge.Unload();

            return false;
        }

        /// <summary>
        /// Loads the badge.
        /// </summary>
        /// <param name="player">The owner of the <see cref="Badge"/>.</param>
        /// <param name="rank">The content of the <see cref="Badge"/>.</param>
        /// <param name="color">The color of the content.</param>
        /// <returns>The loaded <see cref="Badge"/> object.</returns>
        public static Badge Load(Player player, string rank, string color)
        {
            try
            {
                Badge toLoad = new(player, rank, color);
                toLoad.Load();

                return toLoad;
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't load in-game badge: Player ({player.Nickname}) Id ({player.Id}) Rank ({rank}) Color ({color})\nException: {ex}");
                return default;
            }
        }

        /// <summary>
        /// Unloads the badge.
        /// </summary>
        public void Unload()
        {
            Owner.RankName = _oldRank;
            Owner.RankColor = _oldColor;
            Reload();
            Hide();
        }

        /// <summary>
        /// Loads the badge.
        /// </summary>
        public void Load()
        {
            Owner.BadgeHidden = false;
            Owner.RankName = Rank;
            Owner.RankColor = Color;
        }

        /// <summary>
        /// Reloads the badge.
        /// </summary>
        public void Reload()
        {
            Owner.ReferenceHub.serverRoles.HiddenBadge = null;
            Owner.ReferenceHub.serverRoles.RpcResetFixed();
            Owner.ReferenceHub.serverRoles.RefreshPermissions(true);
        }

        /// <summary>
        /// Hides the badge.
        /// </summary>
        public void Hide()
        {
            Owner.ReferenceHub.serverRoles.HiddenBadge = Owner.ReferenceHub.serverRoles.MyText;
            Owner.ReferenceHub.serverRoles.NetworkGlobalBadge = null;
            Owner.ReferenceHub.serverRoles.SetText(null);
            Owner.ReferenceHub.serverRoles.SetColor(null);
            Owner.ReferenceHub.serverRoles.RefreshHiddenTag();
            Owner.ReferenceHub.characterClassManager.CmdRequestHideTag();
        }
    }
}
