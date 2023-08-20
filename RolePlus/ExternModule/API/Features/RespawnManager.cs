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
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles;
    using Respawning;

    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.API.Features.CustomTeams;
    using RolePlus.Internal;

    /// <summary>
    /// A tool which handles all the respawn properties and its behavior.
    /// <br>Overrides the base-game RespawnManager</br>
    /// <br>Implements new and enhanced functions which also support custom roles and custom teams.</br>
    /// </summary>
    public sealed class RespawnManager
    {
        private static readonly List<CoroutineHandle> _respawnHandle = new();
        private static CoroutineHandle _respawnCoreHandle;

        private static RoleType[] CHIRoles { get; } = Enum.GetValues(typeof(RoleType)).ToArray<RoleType>().Where(role => role.ToString().Contains("Chaos")).ToArray();

        private static RoleType[] MTFRoles { get; } = Enum.GetValues(typeof(RoleType)).ToArray<RoleType>().Where(role => role.ToString().Contains("Ntf")).ToArray();

        /// <summary>
        /// Gets or sets the default respawn cooldown.
        /// </summary>
        public static int DefaultRespawnCooldown { get; set; } = 280;

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
        public static IEnumerable<CustomRole> MTFCustomRoles => CustomRole.Registered.Where(customRole => customRole.RespawnTeam is PlayerRoles.Team.FoundationForces && !customRole.IsTeamUnit);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing all the custom roles belonging to CHI team.
        /// </summary>
        public static IEnumerable<CustomRole> CHICustomRoles => CustomRole.Registered.Where(customRole => customRole.RespawnTeam is PlayerRoles.Team.ChaosInsurgency && !customRole.IsTeamUnit);

        /// <summary>
        /// Gets or sets the current respawn state.
        /// </summary>
        public static RespawnStateBase State { get; set; } = RespawnStateBase.Enabled;

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="State"/> is enabled.
        /// </summary>
        public static bool IsEnabled
        {
            get => State == RespawnStateBase.Enabled;
            set => State = value ? RespawnStateBase.Enabled : RespawnStateBase.Disabled;
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
        public static void RespawnTeam(SpawnableTeamType team, uint spawnLimit = 12)
        {
            if (team is not SpawnableTeamType.NineTailedFox and not SpawnableTeamType.ChaosInsurgency)
                return;

            HashSet<Player> spawnedPlayers = new();
            List<CustomRole> customRoles = team is SpawnableTeamType.NineTailedFox ? MTFCustomRoles.ToList() : CHICustomRoles.ToList();

            int unitNumber = UnityEngine.Random.Range(0, 20);

            List<Player> toSpawn = Player.Get(p => p.Role.Team is Team.SCPs && ManagedRoles.Select(role => CustomRole.Get(role).Owners.Contains(p)).FirstOrDefault()).ToList();
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
                    if ((team is SpawnableTeamType.NineTailedFox && i == 0 && customRole.Role != RoleType.NtfCaptain) || !customRole.CanSpawnByProbability())
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

                    if (team is SpawnableTeamType.NineTailedFox)
                        Timing.CallDelayed(0.5f, () => ply.Role.Set(i == 0 ? MTFRoles.FirstOrDefault(role => role.ToString() == RoleType.NtfCaptain.ToString()) : MTFRoles.Random(), SpawnReason.Respawn));
                    else
                        Timing.CallDelayed(0.5f, () => ply.Role.Set(CHIRoles.Random(), SpawnReason.Respawn));
                }

                spawnedPlayers.Add(ply);
            }

            if (team is SpawnableTeamType.NineTailedFox)
                Respawn.SummonNtfChopper();
            else
                Respawn.SummonChaosInsurgencyVan();
        }

        /// <summary>
        /// Forces the respawn given a <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The team to be forced to respawn.</param>
        public static void ForceRespawn(SpawnableTeamType team)
        {
            if (team is not SpawnableTeamType.NineTailedFox and not SpawnableTeamType.ChaosInsurgency)
                return;

            IEnumerable<CustomTeam> customTeams = CustomTeam.Registered.Where(t =>
            t.RespawnTeam == (team is not SpawnableTeamType.NineTailedFox ? SpawnableTeamType.NineTailedFox : SpawnableTeamType.ChaosInsurgency));

            CustomTeam customTeam = null;

            foreach (CustomTeam toSpawn in customTeams)
            {
                if ((!toSpawn.CanSpawnWithoutScps && !SpawnManager.IsAlive(Team.SCPs)) ||
                    (toSpawn.RequiredRoleToSpawn != RoleType.None && !Player.Get(toSpawn.RequiredRoleToSpawn).Any()) ||
                    (toSpawn.RequiredCustomRoleToSpawn is not null && !CustomRole.TryGet(toSpawn.RequiredCustomRoleToSpawn, out CustomRole customRole) &&
                    toSpawn.Owners.Any()) || !toSpawn.Probability.EvaluateProbability() || toSpawn.Tickets <= 0)
                    continue;

                customTeam = toSpawn;
                break;
            }

            if (CustomTeam.TrySpawn(customTeam))
                return;

            RespawnTeam(team, team is SpawnableTeamType.ChaosInsurgency ? CHIUnitsSpawnedAtOnce : MTFUnitsSpawnedAtOnce);
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
            State = RespawnStateBase.Enabled;
        }

        private static IEnumerator<float> RespawnHandler()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                TimeUntilNextRespawn--;
                if (TimeUntilNextRespawn != 10)
                    continue;

                while (Player.Get(p => p.Role.Team is Team.SCPs && ManagedRoles.Select(role => CustomRole.Get(role).Owners.Contains(p)).FirstOrDefault()).IsEmpty())
                {
                    IsWaitingForPlayers = true;
                    yield return Timing.WaitForSeconds(2f);
                }

                IsWaitingForPlayers = false;
                ForceRespawn(UnityEngine.Random.Range(0, 101) <= 50 ? SpawnableTeamType.NineTailedFox : SpawnableTeamType.ChaosInsurgency);
                TimeUntilNextRespawn = DefaultRespawnCooldown;
            }
        }

        private static IEnumerator<float> RespawnStateCheck()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                if (State == RespawnStateBase.Enabled && !Timing.IsRunning(_respawnCoreHandle))
                    _respawnCoreHandle = Timing.RunCoroutine(RespawnHandler());
                else if (State == RespawnStateBase.Disabled && Timing.IsRunning(_respawnCoreHandle))
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
