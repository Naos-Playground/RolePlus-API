// -----------------------------------------------------------------------
// <copyright file="RespawnManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using MEC;

    using MonoMod.Utils;

    using Respawning;
    using Respawning.NamingRules;

    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.API.Features.CustomTeams;
    using RolePlus.Internal;
    using RolePlus.Internal.Configs;

    using static RolePlus.ExternModule.API.Features.CustomTeams.CustomTeam;

    using Config = Configs.Config;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A tool which handles all the respawn properties and its behavior.
    /// <br>Overrides the base-game RespawnManager</br>
    /// <br>Implements new and enhanced functions which also support custom roles and custom teams.</br>
    /// </summary>
    public sealed class RespawnManager
    {
        private static readonly List<CoroutineHandle> _respawnHandle = new();
        private static Dictionary<string, string> _unitNames;
        private static CoroutineHandle _respawnCoreHandle;

        private static RoleType[] CHIRoles { get; } = Enum.GetValues(typeof(RoleType)).ToArray<RoleType>().Where(role => role.ToString().Contains("Chaos")).ToArray();

        private static RoleType[] MTFRoles { get; } = Enum.GetValues(typeof(RoleType)).ToArray<RoleType>().Where(role => role.ToString().Contains("Ntf")).ToArray();

        /// <summary>
        /// Gets or sets the default respawn cooldown.
        /// </summary>
        public static int DefaultRespawnCooldown { get; set; } = 280;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all the assignable unit names.
        /// </summary>
        public static Dictionary<string, string> UnitNames
        {
            get
            {
                if (_unitNames is not null)
                    return _unitNames;

                _unitNames = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.ToDictionary(kvp => kvp.UnitName, kvp => $"NATO_{kvp.UnitName}");
                _unitNames.AddRange(Config.Get<RootConfig>().Read<RespawningRules>()?.AllUnitNames);

                return _unitNames;
            }
        }

        /// <summary>
        /// Gets or sets the maximum amount of MTF units to be spawned at once.
        /// </summary>
        public static uint MTFUnitsSpawnedAtOnce { get; set; } = 12;

        /// <summary>
        /// Gets or sets the maximum amount of Chaos Insurgency units to be spawned at once.
        /// </summary>
        public static uint CHIUnitsSpawnedAtOnce { get; set; } = 12;

        /// <summary>
        /// Gets the respawn queue containing all the players to spawn during the next respawn wave.
        /// </summary>
        public static HashSet<Player> RespawnQueue { get; private set; } = new();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing all the custom roles belonging to MTF team.
        /// </summary>
        public static IEnumerable<CustomRole> MTFCustomRoles => CustomRole.Registered.Where(customRole => customRole.RespawnTeam is Team.MTF && !customRole.IsTeamComponent);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing all the custom roles belonging to CHI team.
        /// </summary>
        public static IEnumerable<CustomRole> CHICustomRoles => CustomRole.Registered.Where(customRole => customRole.RespawnTeam == Team.CHI && !customRole.IsTeamComponent);

        /// <summary>
        /// Gets or sets the current respawn state.
        /// </summary>
        public static RespawnState State { get; set; } = RespawnState.Enabled;

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="State"/> is enabled.
        /// </summary>
        public static bool IsEnabled
        {
            get => State is RespawnState.Enabled;
            set => State = value ? RespawnState.Enabled : RespawnState.Disabled;
        }

        /// <summary>
        /// Gets or sets the timer until next respawn.
        /// </summary>
        public static int TimeUntilNextRespawn { get; set; } = DefaultRespawnCooldown;

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> containing all the players from the last respawn wave.
        /// </summary>
        public static HashSet<Player> LastSpawnedPlayers { get; set; } = new();

        /// <summary>
        /// Gets a value containing all the information about the next team to be respawned.
        /// </summary>
        public static RespawnInfo NextKnownTeam { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the respawn system is waiting for players.
        /// </summary>
        public static bool IsWaitingForPlayers { get; private set; }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> containing all the managed roles during the respawn.
        /// </summary>
        public static HashSet<object> ManagedRoles { get; private set; } = new();

        /// <summary>
        /// Starts the process.
        /// </summary>
        public static void Start()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public static void Stop()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
        }

        /// <summary>
        /// Respawns the specified <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The team to be respawned.</param>
        /// <param name="spawnLimit">The maximum amount of units spawned at once.</param>
        public static void RespawnTeam(Team team, uint spawnLimit = 12)
        {
            if (team is not Team.MTF && team is not Team.CHI)
                return;

            HashSet<Player> spawnedPlayers = new();
            List<CustomRole> customRoles = team is Team.MTF ? MTFCustomRoles.ToList() : CHICustomRoles.ToList();

            int unitNumber = Random.Range(0, 20);

            List<Player> toSpawn = Player.Get(p => p.Role.Team is Team.SCP && ManagedRoles.Select(role => CustomRole.Get(role).Players.Contains(p)).FirstOrDefault()).ToList();
            toSpawn.ShuffleList();

            for (int i = 0; i < spawnLimit; i++)
            {
                Player ply = toSpawn[i];
                customRoles.ShuffleList();
                int classIndex = 0;
                bool hasRole = false;
                while (classIndex < customRoles.Count)
                {
                    CustomRole customRole = customRoles[classIndex];
                    if ((team is Team.MTF && i == 0 && customRole.Role is not RoleType.NtfCaptain) || !customRole.CanSpawnByProbability())
                        classIndex++;
                    else
                    {
                        hasRole = true;
                        CustomRole.UnsafeSpawn(ply, customRole);
                        break;
                    }
                }

                if (!hasRole)
                {
                    ply.SetRole(RoleType.Spectator);

                    if (team is Team.MTF)
                        Timing.CallDelayed(0.5f, () => ply.SetRole(i == 0 ? MTFRoles.FirstOrDefault(role => role.ToString() == RoleType.NtfCaptain.ToString()) : MTFRoles.Random(), SpawnReason.Respawn));
                    else
                        Timing.CallDelayed(0.5f, () => ply.SetRole(CHIRoles.Random(), SpawnReason.Respawn));
                }

                spawnedPlayers.Add(ply);
            }

            if (team is Team.MTF)
            {
                KeyValuePair<string, string> displayUnit = UnitNames.Random();
                string cassieReadableUnit = displayUnit.Key;
                SyncUnit syncUnit = new()
                {
                    SpawnableTeam = (byte)SpawnableTeamType.NineTailedFox,
                    UnitName = $"{displayUnit.Value.ToUpper()}-{(unitNumber.ToString().Length < 2 ? $"0-{unitNumber}" : unitNumber)}",
                };

                foreach (Player player in LastSpawnedPlayers)
                    player.UnitName = syncUnit.UnitName;

                Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Add(syncUnit);
                Respawn.SummonNtfChopper();
            }
            else
                Respawn.SummonChaosInsurgencyVan();
        }

        /// <summary>
        /// Forces the respawn given a <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The team to be forced to respawn.</param>
        public static void ForceRespawn(Team team)
        {
            if (team != Team.MTF && team != Team.CHI)
                return;

            IEnumerable<CustomTeam> customTeams = Registered.Where(t => t.RespawnTeam == (team is Team.MTF ? Team.MTF : Team.CHI));

            CustomTeam customTeam = null;

            foreach (CustomTeam toSpawn in customTeams)
            {
                if ((!toSpawn.CanSpawnWithoutScps && !SpawnManager.IsAlive(Team.SCP)) ||
                    (toSpawn.RequiredRoleToSpawn != RoleType.None && !Player.Get(toSpawn.RequiredRoleToSpawn).Any()) ||
                    (toSpawn.RequiredCustomRoleToSpawn is not null && !CustomRole.TryGet(toSpawn.RequiredCustomRoleToSpawn, out CustomRole customRole) &&
                    toSpawn.AlivePlayers.Any()) || !toSpawn.Probability.EvaluateProbability() || toSpawn.Tickets <= 0)
                    continue;

                customTeam = toSpawn;
                break;
            }

            NextKnownTeam = new()
            {
                Name = customTeam is null ? team is Team.MTF ? "Mobile Task Force" : "Chaos Insurgency" : customTeam.DisplayName,
                Color = customTeam is null ? team is Team.MTF ? "#0b7ce7" : "#0be718" : customTeam.DisplayColor,
            };

            if (TrySpawn(customTeam))
                return;

            RespawnTeam(team, team is Team.CHI ? CHIUnitsSpawnedAtOnce : MTFUnitsSpawnedAtOnce);
        }

        private static void OnRoundStarted()
        {
            _respawnCoreHandle = Timing.RunCoroutine(RespawnHandler());
            _respawnHandle.Add(Timing.RunCoroutine(RespawnStateCheck()));
            _respawnHandle.Add(Timing.RunCoroutine(RespawnTimerHandler()));
        }

        private static void OnRoundEnded(RoundEndedEventArgs _)
        {
            Timing.KillCoroutines(_respawnCoreHandle);
            Timing.KillCoroutines(_respawnHandle.ToArray());
            _respawnHandle.Clear();
            State = RespawnState.Enabled;
        }

        private static IEnumerator<float> RespawnHandler()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                TimeUntilNextRespawn--;
                if (TimeUntilNextRespawn != 10)
                    continue;

                while (Player.Get(p => p.Role.Team is Team.SCP && ManagedRoles.Select(role => CustomRole.Get(role).Players.Contains(p)).FirstOrDefault()).IsEmpty())
                {
                    IsWaitingForPlayers = true;
                    yield return Timing.WaitForSeconds(2f);
                }

                IsWaitingForPlayers = false;
                ForceRespawn(Random.Range(0, 101) <= 50 ? Team.MTF : Team.CHI);
                TimeUntilNextRespawn = DefaultRespawnCooldown;
            }
        }

        private static IEnumerator<float> RespawnStateCheck()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                if (State is RespawnState.Enabled && !Timing.IsRunning(_respawnCoreHandle))
                    _respawnCoreHandle = Timing.RunCoroutine(RespawnHandler());
                else if (State is RespawnState.Disabled && Timing.IsRunning(_respawnCoreHandle))
                    Timing.KillCoroutines(_respawnCoreHandle);
            }
        }

        private static IEnumerator<float> RespawnTimerHandler()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                if (TimeUntilNextRespawn < 0)
                    TimeUntilNextRespawn = DefaultRespawnCooldown;
            }
        }
    }
}
