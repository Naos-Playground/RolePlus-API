// -----------------------------------------------------------------------
// <copyright file="UNetEntity.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using RolePlus.ExternModule.API.Engine.Framework.Structs;

    /// <summary>
    /// The network entity which encapsulates the <see cref="Player"/> object.
    /// </summary>
    public sealed class UNetEntity : UNetObject
    {
        private static readonly List<UNetEntity> _netEntities = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UNetEntity"/> class.
        /// </summary>
        /// <param name="userId"><inheritdoc cref="UserId"/></param>
        /// <param name="isSteamLimited"><inheritdoc cref="IsSteamLimited"/></param>
        /// <param name="isSteamVACBanned"><inheritdoc cref="IsSteamVACBanned"/></param>
        /// <param name="lastUpdate"><inheritdoc cref="LastUpdate"/></param>
        public UNetEntity(string userId, bool isSteamLimited = false, bool isSteamVACBanned = false, DateTime lastUpdate = default)
            : base()
        {
            NetworkAuthority |= ENetworkAuthority.User;

            Player player = Player.Get(userId);
            if (player is not null && player.IsHost)
                return;

            _netEntities.Add(this);

            LastUpdate = lastUpdate;
            IsSteamLimited = isSteamLimited;
            IsSteamVACBanned = isSteamVACBanned;
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="UNetEntity"/> containing all the entities.
        /// </summary>
        public static IReadOnlyList<UNetEntity> Entities => _netEntities;

        /// <summary>
        /// Gets the owner of the entity.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="FVirtualPrivateNetwork"/>.
        /// </summary>
        public FVirtualPrivateNetwork VirtualPrivateNetworkData { get; }

        /// <summary>
        /// Gets a value indicating whether the entity is using a VPN.
        /// </summary>
        public bool IsUsingVPN => !VirtualPrivateNetworkData.IP.IsEmpty();

        /// <summary>
        /// Gets the entity's UserId.
        /// </summary>
        public string UserId { get; internal set; }

#pragma warning disable SA1623 // Property summary documentation should match accessors
        /// <summary>
        /// <inheritdoc cref="Player.Nickname"/>
        /// </summary>
        public string Network_Name => Owner.Nickname;

        /// <summary>
        /// <inheritdoc cref="Player.Ping"/>
        /// </summary>
        public int Latency => Owner.Ping;
#pragma warning restore SA1623 // Property summary documentation should match accessors

        /// <summary>
        /// Gets a value indicating whether the <see cref="Owner"/> is steam limited.
        /// </summary>
        public bool IsSteamLimited { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Owner"/> is steam VAC banned.
        /// </summary>
        public bool IsSteamVACBanned { get; internal set; }

        /// <summary>
        /// Gets the last update of this <see cref="UNetEntity"/>.
        /// </summary>
        public DateTime LastUpdate { get; internal set; }

        /// <summary>
        /// Gets the <see cref="UNetServer"/> on which the <see cref="Owner"/> is currently playing on.
        /// </summary>
        public UNetServer Server { get; internal set; }

        /// <summary>
        /// Gets the <see cref="UNetEntity"/> belonging to the given <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>The corresponding <see cref="UNetEntity"/> or <see langword="null"/> if not found.</returns>
        public static UNetEntity Get(Player player) => Entities.FirstOrDefault(ent => ent.Owner == player);

        /// <summary>
        /// Gets the a <see cref="IEnumerable{T}"/> of <see cref="UNetEntity"/> belonging to the given <see cref="IEnumerable{T}"/> of <see cref="Player"/> objects.
        /// </summary>
        /// <param name="players">The players to check.</param>
        /// <returns>The corresponding <see cref="UNetEntity"/> for each <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<UNetEntity> Get(IEnumerable<Player> players)
        {
            foreach (Player player in players)
                yield return Get(player);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{UserId}";
    }
}
