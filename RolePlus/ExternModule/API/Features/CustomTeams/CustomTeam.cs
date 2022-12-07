// -----------------------------------------------------------------------
// <copyright file="CustomTeam.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomTeams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using Respawning;
    using Respawning.NamingRules;

    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.Events.EventArgs;
    using RolePlus.Internal;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    /// <summary>
    /// A tool to easily manage custom teams.
    /// </summary>
    public class CustomTeam
    {
        private static readonly Dictionary<Player, CustomTeam> _playerValues = new();
        private uint _tickets;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeam"/> class.
        /// </summary>
        public CustomTeam()
        {
            if (SpawnOnEvents.Any())
                Events.Handlers.Server.InvokingHandler += HandleSpawnOnEvents;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CustomTeam"/> class.
        /// </summary>
        ~CustomTeam()
        {
            if (SpawnOnEvents.Any())
                Events.Handlers.Server.InvokingHandler -= HandleSpawnOnEvents;
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomTeam"/>'s.
        /// </summary>
        public static List<CustomTeam> Registered { get; private set; } = new();

        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> which contains all <see cref="Player"/> instances beloging to all <see cref="CustomTeam"/> instances.
        /// </summary>
        public static IReadOnlyDictionary<Player, CustomTeam> Players => _playerValues;

        /// <summary>
        /// Gets the team manager which contains all the players belonging to a <see cref="CustomTeam"/>.
        /// </summary>
        public static HashSet<Player> Manager { get; internal set; } = new();

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Type"/> which contains all types to be used as unit.
        /// </summary>
        public virtual IEnumerable<object> Units => new object[] { };

        /// <summary>
        /// Gets the id.
        /// </summary>
        public virtual uint Id { get; }

        /// <summary>
        /// Gets the relative spawn probability of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual int Probability { get; }

        /// <summary>
        /// Gets the <see cref="Team"/> which is being spawned from.
        /// </summary>
        public virtual Team RespawnTeam { get; }

        /// <summary>
        /// Gets the name of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets the name of the <see cref="CustomTeam"/> to be displayed.
        /// </summary>
        public virtual string DisplayName { get; }

        /// <summary>
        /// Gets the color of the <see cref="Name"/> to be displayed.
        /// </summary>
        public virtual string DisplayColor { get; }

        /// <summary>
        /// Gets the size of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual uint Size { get; }

        /// <summary>
        /// Gets the tickets of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual uint Tickets { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomTeam"/> can be spawned without any scp alive.
        /// </summary>
        public virtual bool CanSpawnWithoutScps => true;

        /// <summary>
        /// Gets the required <see cref="RoleType"/> to allow the <see cref="CustomTeam"/> to spawn.
        /// </summary>
        public virtual RoleType RequiredRoleToSpawn => RoleType.None;

        /// <summary>
        /// Gets the required <see cref="CustomRole"/> to allow the <see cref="CustomTeam"/> to spawn.
        /// </summary>
        public virtual uint? RequiredCustomRoleToSpawn { get; }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all the events, and the relative probability, on which spawn the custom team.
        /// </summary>
        public virtual Dictionary<string, int> SpawnOnEvents { get; } = new();

        /// <summary>
        /// Gets the required leading teams to win.
        /// </summary>
        public virtual Team[] LeadingTeam => new Team[] { };

        /// <summary>
        /// Gets the path of the custom audio file played after the <see cref="CustomTeam"/> is spawned.
        /// </summary>
        public virtual string PathToAlert { get; }

        /// <summary>
        /// Gets the name of the <see cref="CustomTeam"/> unit.
        /// </summary>
        public virtual string UnitName { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomTeam"/> is registered.
        /// </summary>
        public bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all players belonging to this <see cref="CustomTeam"/>.
        /// </summary>
        public IEnumerable<Player> AlivePlayers => Players.Where(x => x.Value == this).Select(j => j.Key);

        /// <summary>
        /// Gets a random <see cref="CustomRole"/>.
        /// </summary>
        public CustomRole RandomUnit => CustomRole.Get(Units.Random());

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomTeam left, object right) => right is uint value && left.Id == value;

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomTeam left, object right) => right is uint value && left.Id != value;

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(object left, CustomTeam right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomTeam right) => right != left;

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomTeam left, CustomTeam right) => left.Id == right.Id;

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomTeam left, CustomTeam right) => left.Id != right.Id;

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> given the specified <see cref="object"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CustomTeam"/> type.</typeparam>
        /// <param name="customRoleType">The <see cref="object"/> to look for.</param>
        /// <param name="customTeam">The found <see cref="CustomTeam"/>, null if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomTeam"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet<T>(object customRoleType, out CustomTeam customTeam)
        {
            customTeam = Get<T>(customRoleType);

            return customTeam is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> given the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="customTeam">The found <see cref="CustomTeam"/>, null if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomTeam"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomTeam customTeam)
        {
            customTeam = Registered.FirstOrDefault(x => x.Name == name);

            return customTeam is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> belonging to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to look for.</param>
        /// <param name="customTeam">The found <see cref="CustomTeam"/>, null if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomTeam"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player player, out CustomTeam customTeam)
        {
            customTeam = null;

            if (!_playerValues.ContainsKey(player))
                return false;

            customTeam = Get(player);

            return true;
        }

        /// <summary>
        /// Tries to spawn the specified <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="customTeam">The <see cref="CustomTeam"/> to be spawned.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(CustomTeam customTeam)
        {
            if (!Player.Get(p => p.IsDead).Any() || customTeam is null)
                return false;

            customTeam.Spawn();

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="CustomTeam"/> given the specified <see cref="object"/>.
        /// </summary>
        /// <param name="customTeamType">The specified <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(object customTeamType)
        {
            if (!Player.Get(p => p.IsDead).Any() || TryGet<CustomTeam>(customTeamType, out CustomTeam customTeam))
                return false;

            customTeam.Spawn();

            return true;
        }

        /// <summary>
        /// Tries to spawn a player as a <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customTeam">The <see cref="CustomTeam"/> unit to be assigned.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Player player, CustomTeam customTeam)
        {
            if (customTeam is null)
                return false;

            customTeam.Spawn(player);

            return true;
        }

        /// <summary>
        /// Tries to spawn a player as a <see cref="CustomTeam"/> unit given the specified <see cref="object"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customTeamType">The specified <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Player player, object customTeamType)
        {
            if (!TryGet<CustomTeam>(customTeamType, out CustomTeam customTeam))
                return false;

            customTeam.Spawn(player);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="IEnumerable{T}"/> of <see cref="Player"/> as a <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="players">The players to be spawned.</param>
        /// <param name="customTeam">The <see cref="CustomTeam"/> unit to be assigned.</param>
        /// <returns><see langword="true"/> if the players were spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(IEnumerable<Player> players, CustomTeam customTeam)
        {
            if (customTeam is null)
                return false;

            customTeam.Spawn(players);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="IEnumerable{T}"/> of <see cref="Player"/> as a <see cref="CustomTeam"/> unit given the specified <see cref="object"/>.
        /// </summary>
        /// <param name="players">The players to be spawned.</param>
        /// <param name="customTeamType">The specified <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the players were spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(IEnumerable<Player> players, object customTeamType)
        {
            if (!TryGet<CustomTeam>(customTeamType, out CustomTeam customTeam))
                return false;

            customTeam.Spawn(players);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="CustomTeam"/> given a specified amount of units.
        /// </summary>
        /// <param name="amount">The amount of units to be spawned.</param>
        /// <param name="customTeam">The <see cref="CustomTeam"/> to be spawned.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(uint amount, CustomTeam customTeam)
        {
            if (customTeam is null)
                return false;

            customTeam.Spawn(amount);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="CustomTeam"/> given the specified <see cref="object"/>.
        /// </summary>
        /// <param name="amount">The amount of units to be spawned.</param>
        /// <param name="customTeamType">The specified <see cref="object"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(uint amount, object customTeamType)
        {
            if (TryGet<CustomTeam>(customTeamType, out CustomTeam customTeam))
                return false;

            customTeam.Spawn(amount);

            return true;
        }

        /// <summary>
        /// Enables all the custom teams present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all the enabled custom teams.</returns>
        public static IEnumerable<CustomTeam> RegisterTeams()
        {
            List<CustomTeam> customTeams = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (type.BaseType != typeof(CustomTeam) || type.GetCustomAttribute(typeof(CustomTeamAttribute)) is null)
                    continue;

                CustomTeam customTeam = Activator.CreateInstance(type) as CustomTeam;

                customTeam.TryRegister();
                customTeams.Add(customTeam);
            }

            if (customTeams.Count() != Registered.Count())
                Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customTeams.Count()} custom teams have been successfully registered!", ConsoleColor.Cyan);

            return customTeams;
        }

        /// <summary>
        /// Disables all the custom teams present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all the disabled custom teams.</returns>
        public static IEnumerable<CustomTeam> UnregisterTeams()
        {
            List<CustomTeam> customTeams = new();
            foreach (CustomTeam customTeam in Registered)
            {
                customTeam.TryUnregister();
                customTeams.Remove(customTeam);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customTeams.Count()} custom teams have been successfully unregistered!", ConsoleColor.Cyan);

            return customTeams;
        }

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> given the specified <see cref="object"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CustomTeam"/> type.</typeparam>
        /// <param name="value">The <see cref="object"/> to check.</param>
        /// <returns>The <see cref="CustomTeam"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomTeam Get<T>(object value) =>
            typeof(T) == typeof(CustomRole) ? Registered.FirstOrDefault(x => x.Units.Contains(value)) :
            typeof(T) == typeof(CustomTeam) ? Registered.FirstOrDefault(x => x == value) : null;

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomTeam"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomTeam Get(string name) => Registered.FirstOrDefault(x => x.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>The <see cref="CustomTeam"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomTeam Get(Player player)
        {
            CustomTeam customTeam = default;

            foreach (KeyValuePair<Player, CustomTeam> kvp in Players)
            {
                if (kvp.Key != player)
                    continue;

                customTeam = Get<CustomTeam>(kvp.Value.Id);
            }

            return customTeam;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomTeam"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was registered; otherwise, <see langword="false"/>.</returns>
        public bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(x => x.Id == Id))
                {
                    Log.Debug(
                        $"[CustomTeams] Couldn't register {Name}. " +
                        $"Another custom team has been registered with the same CustomTeamType:" +
                        $" {Registered.FirstOrDefault(x => x.Id == Id)}",
                        RolePlus.Singleton.Config.ShowDebugMessages);

                    return false;
                }

                Registered.Add(this);

                return true;
            }

            Log.Debug(
                $"[CustomTeams] Couldn't register {Name}. This custom team has been already registered.",
                RolePlus.Singleton.Config.ShowDebugMessages);

            return false;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomTeam"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        public bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug(
                    $"[CustomTeams] Couldn't unregister {Name}. This custom team hasn't been registered yet.",
                    RolePlus.Singleton.Config.ShowDebugMessages);

                return false;
            }

            Registered.Remove(this);

            return true;
        }

        /// <summary>
        /// Spawns a <see cref="Player"/> as <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        public void Spawn(Player player)
        {
            CustomRole.UnsafeSpawn(player, RandomUnit);
            _playerValues.Add(player, this);
        }

        /// <summary>
        /// Spawns the <see cref="CustomTeam"/> given the specified amount of units.
        /// </summary>
        /// <param name="amount">The amount of units to be spawned.</param>
        public void Spawn(uint amount)
        {
            if (_tickets > 0)
                _tickets--;

            for (int i = 0; i < amount; i++)
            {
                Log.Debug(i);
                Player[] players = Player.Get(Team.RIP).ToArray();
                if (players.IsEmpty())
                    return;

                Spawn(players[i]);
            }

            if (RespawnTeam is Team.MTF)
            {
                SyncUnit mtfUnit = new()
                {
                    SpawnableTeam = (byte)SpawnableTeamType.NineTailedFox,
                    UnitName = UnitName,
                };

                RespawnManager.Singleton.NamingManager.AllUnitNames.Add(mtfUnit);
            }
        }

        /// <summary>
        /// Spawns the <see cref="CustomTeam"/>.
        /// </summary>
        public void Spawn() => Spawn(Size);

        /// <summary>
        /// Spawns a <see cref="IEnumerable{T}"/> of <see cref="Player"/> as this <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="players">The players to be spawned.</param>
        /// <param name="keepSize">A value indicating whether the team size should be the same as the specified one.</param>
        public void Spawn(IEnumerable<Player> players, bool keepSize = true)
        {
            if (_tickets > 0)
                _tickets--;

            int count = 0;
            foreach (Player player in players)
            {
                if (!keepSize && count >= Size)
                    break;

                if (player is null)
                    continue;

                Spawn(player);
                count++;
            }

            if (RespawnTeam is Team.MTF)
            {
                SyncUnit mtfUnit = new()
                {
                    SpawnableTeam = (byte)SpawnableTeamType.NineTailedFox,
                    UnitName = UnitName,
                };

                RespawnManager.Singleton.NamingManager.AllUnitNames.Add(mtfUnit);
            }
        }

        /// <summary>
        /// Adds respawn tickets to the current <see cref="CustomTeam"/> instance given a specified amount.
        /// </summary>
        /// <param name="amount">The amount of tickets to add.</param>
        public void AddTickets(uint amount) => _tickets += amount;

        /// <summary>
        /// Removes respawn tickets to the current <see cref="CustomTeam"/> instance given a specified amount.
        /// </summary>
        /// <param name="amount">The amount of tickets to remove.</param>
        public void RemoveTickets(uint amount)
        {
            if (_tickets < amount)
                return;

            _tickets -= amount;
        }

        private void HandleSpawnOnEvents(InvokingHandlerEventArgs ev)
        {
            foreach ((KeyValuePair<string, int> kvp, string[] eventName) in
                from KeyValuePair<string, int> kvp in SpawnOnEvents
                let eventName = ev.Name.ToLower().SplitCamelCase().Split(' ')
                select (kvp, eventName))
            {
                if ((kvp.Key.ToLower() != ev.Name.ToLower() &&
                    !eventName.Contains(kvp.Key.ToLower())) ||
                    !kvp.Value.EvaluateProbability())
                    continue;

                IEnumerable<Player> players = Player.Get(Team.RIP);
                if (players.IsEmpty())
                    return;

                Spawn(players);
            }
        }

        /// <summary>
        /// A tool to easily handle respawn settings.
        /// </summary>
        public struct RespawnInfo
        {
            /// <summary>
            /// Gets or sets the respawn unit name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the respawn unit color.
            /// </summary>
            public string Color { get; set; }
        }
    }
}
