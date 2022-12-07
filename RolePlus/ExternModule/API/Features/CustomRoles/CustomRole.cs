// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using MEC;

    using RolePlus.ExternModule.API.Engine.Core;
    using RolePlus.ExternModule.Events.EventArgs;
    using RolePlus.Internal;

    using UnityEngine;

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    /// <inheritdoc/>
    public abstract class CustomRole<T> : CustomRole
        where T : RoleBuilder
    {
        /// <inheritdoc/>
        public override Type RoleBuilderComponent => typeof(T);
    }

    /// <summary>
    /// A tool to easily manage custom roles.
    /// </summary>
    public abstract class CustomRole : TypeCastObject<CustomRole>
    {
        /// <summary>
        /// Gets the players and their respective <see cref="CustomRole"/>.
        /// </summary>
        internal static readonly Dictionary<Player, CustomRole> _playerValues = new();

        /// <summary>
        /// Gets the players and their respective <see cref="CustomRole"/>.
        /// </summary>
        internal static IReadOnlyDictionary<Player, CustomRole> PlayerValues => _playerValues;

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomRole"/>'s.
        /// </summary>
        public static List<CustomRole> Registered { get; private set; } = new();

        /// <summary>
        /// Gets the role manager which contains all the players with a <see cref="CustomRole"/>.
        /// </summary>
        public static HashSet<Player> Manager => PlayerValues.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type RoleBuilderComponent { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public virtual uint Id { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s <see cref="RoleType"/>.
        /// </summary>
        public virtual RoleType Role { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s escape <see cref="RoleType"/>.
        /// </summary>
        public virtual RoleType EscapeRole { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s escape <see cref="CustomRole"/>.
        /// </summary>
        public virtual uint EscapeCustomRole { get; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all the events, and the relative probability, on which spawn the custom role.
        /// </summary>
        public virtual Dictionary<string, int> SpawnOnEvents { get; } = new();

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="CustomRole"/> is a custom SCP.
        /// </summary>
        public virtual bool IsScp { get; }

        /// <summary>
        /// Gets the relative spawn probability of the <see cref="CustomRole"/>.
        /// </summary>
        public virtual int Probability { get; }

        /// <summary>
        /// Gets a value representing the maximum instances of the <see cref="CustomRole"/> that can be automatically assigned.
        /// </summary>
        public virtual int MaxInstances => IsScp ? 1 : -1;

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s respawn <see cref="Team"/> .
        /// </summary>
        public virtual Team RespawnTeam => Team.TUT;

        /// <summary>
        /// Gets the <see cref="CustomRole"/>'s respawn <see cref="RoleType"/> .
        /// </summary>
        public virtual RoleType RespawnRole { get; }

        /// <summary>
        /// Gets a the <see cref="CustomRole"/>'s name.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is used for custom teams.
        /// </summary>
        public virtual bool IsTeamComponent { get; }

        /// <summary>
        /// Gets a value indicating whether the player should be spawned in the same position.
        /// </summary>
        public virtual bool ShouldKeepPosition { get; }

        /// <summary>
        /// Gets a value indicating whether the escape role should be overridden.
        /// </summary>
        public virtual bool OverrideEscapeRole { get; }

        /// <summary>
        /// Gets a value indicating whether the escape custom role should be overridden.
        /// </summary>
        public virtual bool OverrideEscapeCustomRole { get; }

        /// <summary>
        /// Gets a value indicating whether the player can escape through the escape inner.
        /// </summary>
        public virtual bool CanEscape => true;

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomRole"/> is registered.
        /// </summary>
        public virtual bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a list of players who have this <see cref="CustomRole"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.Get(x => TryGet(x, out CustomRole customRole) && customRole.Id == Id);

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomRole left, object right)
        {
            try
            {
                uint value = (uint)right;
                return left.Id == value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomRole left, object right)
        {
            try
            {
                uint value = (uint)right;
                return left.Id != value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(object left, CustomRole right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomRole right) => right != left;

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomRole left, CustomRole right) => left.Id == right.Id;

        /// <summary>
        /// Compares two operands: <see cref="CustomRole"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomRole"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomRole left, CustomRole right) => left.Id != right.Id;

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given the specified <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="customRoleType">The <see cref="object"/> to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object customRoleType, out CustomRole customRole)
        {
            customRole = Get(customRoleType);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomRole"/> name to look for.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomRole customRole)
        {
            customRole = Registered.FirstOrDefault(cRole => cRole.Name == name);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to get the player's current <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to search on.</param>
        /// <param name="customRole">The found <see cref="CustomRole"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRole"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player player, out CustomRole customRole)
        {
            customRole = Get(player);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to spawn the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customRole">The <see cref="CustomRole"/> to be set.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool SafeSpawn(Player player, CustomRole customRole)
        {
            if (customRole is null)
                return false;

            customRole.Spawn(player);

            return true;
        }

        /// <summary>
        /// Tries to spawn the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customRoleType">The <see cref="object"/> to be set.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool SafeSpawn(Player player, object customRoleType)
        {
            if (!TryGet(customRoleType, out CustomRole customRole))
                return false;

            SafeSpawn(player, customRole);

            return true;
        }

        /// <summary>
        /// Tries to spawn the player as a specific <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="name">The <see cref="CustomRole"/> name to be set.</param>
        /// <returns>Returns a value indicating whether the <see cref="Player"/> was spawned or not.</returns>
        public static bool SafeSpawn(Player player, string name)
        {
            if (!TryGet(name, out CustomRole customRole))
                return false;

            SafeSpawn(player, customRole);

            return true;
        }

        /// <summary>
        /// Tries to force spawn the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customRole">The <see cref="CustomRole"/> to be set.</param>
        /// <param name="shouldKeepPosition">A value indicating whether the <see cref="Player"/> should be spawned in the same position.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool UnsafeSpawn(Player player, CustomRole customRole, bool shouldKeepPosition = false)
        {
            if (customRole is null)
                return false;

            customRole.ForceSpawn(player, shouldKeepPosition);

            return true;
        }

        /// <summary>
        /// Tries to force spawn the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customRoleType">The <see cref="object"/> to be set.</param>
        /// <param name="shouldKeepPosition">A value indicating whether the <see cref="Player"/> should be spawned in the same position.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool UnsafeSpawn(Player player, object customRoleType, bool shouldKeepPosition = false)
        {
            if (!TryGet(customRoleType, out CustomRole customRole))
                return false;

            UnsafeSpawn(player, customRole, shouldKeepPosition);

            return true;
        }

        /// <summary>
        /// Tries to force spawn the player as a specific <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="name">The <see cref="CustomRole"/> name to be set.</param>
        /// <param name="shouldKeepPosition">A value indicating whether the <see cref="Player"/> should be spawned in the same position.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool UnsafeSpawn(Player player, string name, bool shouldKeepPosition = false)
        {
            if (!TryGet(name, out CustomRole customRole))
                return false;

            UnsafeSpawn(player, customRole, shouldKeepPosition);

            return true;
        }

        /// <summary>
        /// Enables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all the enabled custom roles.</returns>
        public static List<CustomRole> RegisterRoles()
        {
            List<CustomRole> customRoles = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if ((type.BaseType != typeof(CustomRole) && !type.IsSubclassOf(typeof(CustomRole))) || type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null)
                    continue;

                CustomRole customRole = Activator.CreateInstance(type) as CustomRole;

                if (!customRole.IsEnabled)
                    continue;

                customRole.TryRegister();
                customRoles.Add(customRole);
            }

            if (customRoles.Count() != Registered.Count())
                Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customRoles.Count()} custom roles have been successfully registered!", ConsoleColor.Cyan);

            return customRoles;
        }

        /// <summary>
        /// Disables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all the disabled custom roles.</returns>
        public static List<CustomRole> UnregisterRoles()
        {
            List<CustomRole> customRoles = new();
            foreach (CustomRole customRole in Registered)
            {
                customRole.TryUnregister();
                customRoles.Add(customRole);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customRoles.Count()} custom roles have been successfully unregistered!", ConsoleColor.Cyan);

            return customRoles;
        }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="Id"/>.
        /// </summary>
        /// <param name="customRoleType">The specified <see cref="Id"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(object customRoleType) => Registered.FirstOrDefault(customRole => customRole == customRoleType && customRole.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(string name) => Registered.FirstOrDefault(customRole => customRole.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <see cref="RoleBuilderComponent"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="RoleBuilderComponent"/>.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomRole Get(Type type) => type.BaseType != typeof(RoleBuilder) ? null : Registered.FirstOrDefault(customRole => customRole.RoleBuilderComponent == type);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="CustomRole"/> owner.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomRole Get(Player player)
        {
            CustomRole customRole = default;

            foreach (KeyValuePair<Player, CustomRole> kvp in _playerValues)
            {
                if (kvp.Key != player)
                    continue;

                customRole = Get(kvp.Value.Id);
            }

            return customRole;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomRole"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(x => x.Id == Id))
                {
                    Log.Debug(
                        $"[CustomRoles] Couldn't register {Name}. " +
                        $"Another custom role has been registered with the same CustomRoleType:" +
                        $" {Registered.FirstOrDefault(x => x.Id == Id)}",
                        RolePlus.Singleton.Config.ShowDebugMessages);

                    return false;
                }

                Registered.Add(this);
                InternalProcesses(true);

                return true;
            }

            Log.Debug(
                $"[CustomRoles] Couldn't register {Name}. This custom role has been already registered.",
                RolePlus.Singleton.Config.ShowDebugMessages);

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomRole"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug(
                    $"[CustomRoles] Couldn't unregister {Name}. This custom role hasn't been registered yet.",
                    RolePlus.Singleton.Config.ShowDebugMessages);

                return false;
            }

            Registered.Remove(this);
            InternalProcesses(false);

            return true;
        }

        /// <summary>
        /// Spawns the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public bool Spawn(Player player)
        {
            if (player.Role.Team != Team.RIP)
                return false;

            RespawnManager.RespawnQueue.Add(player);

            player.GameObject.AddComponent(RoleBuilderComponent);
            _playerValues.Remove(player);
            _playerValues.Add(player, this);

            return true;
        }

        /// <summary>
        /// Force spawns the player as a specific <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="shouldKeepPosition">A value indicating whether the <see cref="Player"/> should be spawned in the same position.</param>
        public void ForceSpawn(Player player, bool shouldKeepPosition = false)
        {
            _playerValues.Remove(player);
            _playerValues.Add(player, this);

            if (player.IsAlive)
            {
                player.Role.Type = RoleType.Spectator;
                Timing.CallDelayed(1.5f, () => ForceSpawn_Internal(player, shouldKeepPosition));
            }
            else
                ForceSpawn_Internal(player, shouldKeepPosition);
        }

        /// <summary>
        /// Gets a value indicating whether a player can be spawned as a specific <see cref="CustomRole"/> given its probability.
        /// </summary>
        /// <returns><see langword="true"/> if the probability condition was satified; otherwise, <see langword="false"/>.</returns>
        public bool CanSpawnByProbability() => UnityEngine.Random.Range(0, 101) <= Probability;

        private void HandleSpawnOnEvents(InvokingHandlerEventArgs ev)
        {
            foreach (KeyValuePair<string, int> kvp in SpawnOnEvents)
            {
                string[] eventName = ev.Name.ToLower().SplitCamelCase().Split(' ');
                if ((kvp.Key.ToLower() != ev.Name.ToLower() &&
                    !eventName.Contains(kvp.Key.ToLower())) ||
                    !kvp.Value.EvaluateProbability())
                    continue;

                IEnumerable<Player> players = Player.Get(Team.RIP);
                if (players.IsEmpty())
                    return;

                SafeSpawn(Player.Get(Team.RIP).Random(), this);
            }
        }

        private void OverrideDefaultSpawnpoint(SpawningEventArgs ev)
        {
            if (!TryGet(ev.Player, out CustomRole customRole) ||
                customRole != this ||
                !ev.Player.GameObject.TryGetComponent(out RoleBuilder builder))
                return;

            bool useCustomSpawnpoint = (bool)builder.GetType().BaseType
                .GetProperty("UseCustomSpawnpoint", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(builder);

            if (!useCustomSpawnpoint)
                return;

            ev.Position = builder.RandomSpawnpoint + (Vector3.up * 1.5f);
        }

        private void OverrideDefaultInventory(ChangingRoleEventArgs ev)
        {
            if (!TryGet(ev.Player, out CustomRole customRole) ||
                customRole != this ||
                !ev.Player.GameObject.TryGetComponent(out RoleBuilder builder))
                return;

            Info.InventoryManager inventoryInfo = (Info.InventoryManager)builder.GetType().BaseType
                .GetProperty("InventoryInfo", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(builder);

            ev.Items.Clear();
            if (inventoryInfo.Items is not null)
                ev.Items.AddRange(inventoryInfo.Items);

            Timing.CallDelayed(1f, () =>
            {
                if (inventoryInfo.CustomItems is not null)
                    ev.Player.AddItem(inventoryInfo.CustomItems);
            });

            ev.Ammo.Clear();

            if (inventoryInfo.AmmoBox is not null)
            {
                foreach (KeyValuePair<AmmoType, ushort> kvp in inventoryInfo.AmmoBox)
                    ev.Ammo?.Add(kvp.Key.GetItemType(), kvp.Value);
            }

            ev.Lite = RoleBuilder.LitePlayers.Contains(ev.Player);
            RoleBuilder.LitePlayers.Remove(ev.Player);
        }

        private void InternalProcesses(bool status)
        {
            if (status)
            {
                if (SpawnOnEvents.Any())
                    Events.Handlers.Server.InvokingHandler += HandleSpawnOnEvents;

                Exiled.Events.Handlers.Player.Spawning += OverrideDefaultSpawnpoint;
                Exiled.Events.Handlers.Player.ChangingRole += OverrideDefaultInventory;
            }
            else
            {
                if (SpawnOnEvents.Any())
                    Events.Handlers.Server.InvokingHandler -= HandleSpawnOnEvents;

                Exiled.Events.Handlers.Player.Spawning -= OverrideDefaultSpawnpoint;
                Exiled.Events.Handlers.Player.ChangingRole -= OverrideDefaultInventory;
            }
        }

        private void ForceSpawn_Internal(Player player, bool shouldKeepPosition)
        {
            RespawnManager.RespawnQueue.Add(player);

            if (shouldKeepPosition)
                RoleBuilder.LitePlayers.Add(player);

            player.GameObject.AddComponent(RoleBuilderComponent);
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        /// <summary>
        /// Defines the contract for config features used by <see cref="CustomRole"/>.
        /// </summary>
        public interface ICustomRoleConfig
        {
            /// <summary>
            /// Defines the contract for config features related to inventory management.
            /// </summary>
            public interface ICustomInventory
            {
                /// <summary>
                /// Gets or sets the items to be given.
                /// </summary>
                public abstract List<ItemType> Items { get; set; }

                /// <summary>
                /// Gets or sets the custom items to be given.
                /// </summary>
                public abstract List<object> CustomItems { get; set; }

                /// <summary>
                /// Gets or sets the ammo box to be given.
                /// </summary>
                public abstract Dictionary<AmmoType, ushort> AmmoBox { get; set; }
            }

            /// <summary>
            /// Gets or sets the <see cref="Player.Scale"/>.
            /// </summary>
            public abstract float Scale { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Player.Health"/>.
            /// </summary>
            public abstract float Health { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Player"/>'s maximum health.
            /// </summary>
            public abstract int MaxHealth { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Player.ArtificialHealth"/>.
            /// </summary>
            public abstract float ArtificialHealth { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Player.MaxArtificialHealth"/>.
            /// </summary>
            public abstract float MaxArtificialHealth { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Exiled.API.Features.Broadcast"/> content.
            /// </summary>
            public abstract Broadcast Broadcast { get; set; }
        }

        /// <summary>
        /// A set of tools to use with <see cref="CustomRole"/>.
        /// </summary>
        public class Info : ICustomRoleConfig
        {
            /// <summary>
            /// A tool to easily handle human <see cref="CustomRole"/> settings.
            /// </summary>
            public class InventoryManager : ICustomRoleConfig.ICustomInventory
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="InventoryManager"/> class.
                /// </summary>
                public InventoryManager()
                {
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="InventoryManager"/> class.
                /// </summary>
                /// <param name="inventory">The items to be given.</param>
                /// <param name="customItems">The custom items to be given.</param>
                /// <param name="ammoBox">The ammobox to be set.</param>
                public InventoryManager(
                    List<ItemType> inventory,
                    List<object> customItems,
                    Dictionary<AmmoType, ushort> ammoBox)
                {
                    Items = inventory;
                    CustomItems = customItems;
                    AmmoBox = ammoBox;
                }

                /// <inheritdoc/>
                public List<ItemType> Items { get; set; }

                /// <inheritdoc/>
                public List<object> CustomItems { get; set; }

                /// <inheritdoc/>
                public Dictionary<AmmoType, ushort> AmmoBox { get; set; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Info"/> class.
            /// </summary>
            public Info()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Info"/> class.
            /// </summary>
            /// <param name="scale">The scale to be set.</param>
            /// <param name="health">The health to be set.</param>
            /// <param name="maxHealth">The max health to be set.</param>
            /// <param name="artificialHealth">The artificial health to be set.</param>
            /// <param name="maxArtificialHealth">The max artificial health to be set.</param>
            /// <param name="broadcast">The broadcast to show.</param>
            /// <param name="spawnPoints">The spawnpoints.</param>
            public Info(
                float scale,
                float health,
                int maxHealth,
                float artificialHealth,
                float maxArtificialHealth,
                Broadcast broadcast,
                List<RoomType> spawnPoints)
            {
                Scale = scale;
                Health = health;
                MaxHealth = maxHealth;
                ArtificialHealth = artificialHealth;
                MaxArtificialHealth = maxArtificialHealth;
                Broadcast = broadcast;
                Spawnpoints = spawnPoints;
            }

            /// <inheritdoc/>
            public float Scale { get; set; }

            /// <inheritdoc/>
            public float Health { get; set; }

            /// <inheritdoc/>
            public int MaxHealth { get; set; }

            /// <inheritdoc/>
            public float ArtificialHealth { get; set; }

            /// <inheritdoc/>
            public float MaxArtificialHealth { get; set; }

            /// <inheritdoc/>
            public Broadcast Broadcast { get; set; }

            /// <inheritdoc/>
            public List<RoomType> Spawnpoints { get; set; }
        }
    }
}
