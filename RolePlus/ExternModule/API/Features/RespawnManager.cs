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
<<<<<<< HEAD
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
=======
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles;
    using Respawning;
<<<<<<< HEAD
=======
    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Engine.Framework.Generic;
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.API.Features.CustomTeams;
    using RolePlus.Internal;
    using UnityEngine;

    /// <summary>
    /// A tool which handles all the respawn properties and its behavior.
    /// <br>Overrides the base-game RespawnManager</br>
    /// <br>Implements new and enhanced functions which also support custom roles and custom teams.</br>
    /// </summary>
    public sealed class RespawnManager : StaticActor
    {
        private static readonly List<CoroutineHandle> _respawnHandle = new();
        private static CoroutineHandle _respawnCoreHandle;

        private static RoleType[] DefaultChaosInsurgencyRoles { get; } = EnumClass<RoleTypeId, RoleType>.Values.Where(v => v.Value.ToString().Contains("Chaos")).ToArray();

        private static RoleType[] DefaultNtfRoles { get; } = new RoleType[]
        {
            RoleType.NtfSergeant,
            RoleType.NtfPrivate,
        };

        /// <summary>
        /// Gets or sets the default respawn cooldown.
        /// </summary>
        public int DefaultRespawnCooldown { get; set; } = 280;

        /// <summary>
        /// Gets or sets the maximum amount of MTF units to be spawned at once.
        /// </summary>
        public uint MTFUnitsSpawnedAtOnce { get; set; } = 12;

        /// <summary>
        /// Gets or sets the maximum amount of Chaos Insurgency units to be spawned at once.
        /// </summary>
        public uint CHIUnitsSpawnedAtOnce { get; set; } = 12;

        /// <summary>
        /// Gets the respawn queue containing all the players to spawn during the next respawn wave.
        /// </summary>
        public HashSet<Player> RespawnQueue { get; private set; } = new();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing all the custom roles belonging to MTF team.
        /// </summary>
        public IEnumerable<CustomRole> MTFCustomRoles => CustomRole.Registered.Where(customRole => customRole.RespawnTeam is SpawnableTeamType.NineTailedFox && !customRole.IsTeamUnit);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing all the custom roles belonging to CHI team.
        /// </summary>
        public IEnumerable<CustomRole> CHICustomRoles => CustomRole.Registered.Where(customRole => customRole.RespawnTeam is SpawnableTeamType.ChaosInsurgency && !customRole.IsTeamUnit);

        /// <summary>
        /// Gets or sets the current respawn state.
        /// </summary>
        public RespawnStateBase State { get; set; } = RespawnStateBase.Enabled;

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="State"/> is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => State == RespawnStateBase.Enabled;
            set => State = value ? RespawnStateBase.Enabled : RespawnStateBase.Disabled;
        }

        /// <summary>
        /// Gets or sets the timer until next respawn.
        /// </summary>
        public int TimeUntilNextRespawn { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> containing all the players from the last respawn wave.
        /// </summary>
        public HashSet<Player> LastSpawnedPlayers { get; set; } = new();

        /// <summary>
        /// Gets a value indicating whether or not the respawn system is waiting for players.
        /// </summary>
        public bool IsWaitingForPlayers { get; private set; }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> containing all the managed roles during the respawn.
        /// </summary>
        public HashSet<object> ManagedRoles { get; private set; } = new();

        /// <summary>
        /// Gets all the SCPs' death position.
        /// </summary>
        public Dictionary<RoleType, Vector3> ScpsDeathPositions { get; } = new();

        /// <summary>
        /// Gets all the SCPs' spawn position.
        /// </summary>
        public Dictionary<RoleType, Vector3> ScpsSpawnPositions { get; } = new();

        /// <summary>
        /// Gets all the custom SCPs' death position.
        /// </summary>
        public Dictionary<object, Vector3> CustomScpsDeathPositions { get; } = new();

        /// <summary>
        /// Gets all the custom SCPs' spawn position.
        /// </summary>
        public Dictionary<object, Vector3> CustomScpsSpawnPositions { get; } = new();

        /// <summary>
        /// Gets all the contained SCPs.
        /// </summary>
        public List<RoleType> ContainedScps { get; } = new();

        /// <summary>
        /// Gets all the contained custom SCPs.
        /// </summary>
        public List<object> ContainedCustomScps { get; } = new();

        /// <summary>
        /// Gets all the spawned SCPs.
        /// </summary>
        public List<RoleType> SpawnedScps { get; } = new();

        /// <summary>
        /// Gets all the spawned custom SCPs.
        /// </summary>
        public List<object> SpawnedCustomScps { get; } = new();

        /// <summary>
        /// Respawns the specified <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The team to respawn.</param>
        /// <param name="units">The maximum amount of units spawned at once.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> containing all spawned players.</returns>
        public IEnumerable<Player> RespawnTeam(SpawnableTeamType team, uint units = 12) =>
            team is SpawnableTeamType.ChaosInsurgency ? RespawnChaosInsurgencyWave(units) : team is SpawnableTeamType.ChaosInsurgency ? RespawnNineTaledFoxWave(units) : null;

        /// <summary>
        /// Respawns a Chaos Insurgency wave.
        /// </summary>
        /// <param name="units">The maximum amount of units spawned at once.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> containing all spawned players.</returns>
        public IEnumerable<Player> RespawnChaosInsurgencyWave(uint units = 12)
        {
            List<Pawn> toSpawn = Player.Get(p => p.IsDead).Cast<Pawn>().ToList();
            toSpawn.ShuffleListSecure();
            return RespawnChaosInsurgencyWave(toSpawn, units);
        }

        /// <summary>
        /// Respawns a <see cref="SpawnableTeamType.NineTailedFox"/> wave.
        /// </summary>
        /// <param name="units">The maximum amount of units spawned at once.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> containing all spawned players.</returns>
        public IEnumerable<Player> RespawnNineTaledFoxWave(uint units = 12)
        {
            List<Pawn> toSpawn = Player.Get(p => p.IsDead).Cast<Pawn>().ToList();
            toSpawn.ShuffleListSecure();
            return RespawnNineTaledFoxWave(toSpawn, units);
        }

        /// <summary>
        /// Respawns a <see cref="SpawnableTeamType.ChaosInsurgency"/> wave.
        /// </summary>
        /// <param name="players">The players to be respawned.</param>
        /// <param name="units">The maximum amount of units spawned at once.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> containing all spawned players.</returns>
        public IEnumerable<Pawn> RespawnChaosInsurgencyWave(IEnumerable<Player> players, uint units = 12)
        {
            List<CustomRole> customRoles = CustomRole.Registered
                .Where(customRole => customRole.RespawnTeam == SpawnableTeamType.ChaosInsurgency && !customRole.IsTeamUnit)
                .ToList();

            int idx = 0;
            bool hasRole;
            foreach (Pawn player in players.Cast<Pawn>())
            {
                if (idx >= units)
                    break;

                hasRole = false;
                foreach (CustomRole customRole in customRoles)
                {
                    if (!customRole.CanSpawnByProbability())
                        continue;

                    player.SetRole(customRole);
                    hasRole = true;
                    break;
                }

                if (!hasRole)
                    player.SetRole(DefaultChaosInsurgencyRoles.Random());

                yield return player;
                idx++;
            }

            Respawn.SummonChaosInsurgencyVan();
        }

        /// <summary>
        /// Respawns a <see cref="SpawnableTeamType.ChaosInsurgency"/> wave.
        /// </summary>
        /// <param name="players">The players to be respawned.</param>
        /// <param name="units">The maximum amount of units spawned at once.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> containing all spawned players.</returns>
        public IEnumerable<Pawn> RespawnNineTaledFoxWave(IEnumerable<Pawn> players, uint units = 12)
        {
            List<CustomRole> customRoles = CustomRole.Registered
                .Where(customRole => customRole.RespawnTeam == SpawnableTeamType.NineTailedFox && !customRole.IsTeamUnit)
                .ToList();

            List<Pawn> toSpawn = players.ToList();
            Pawn captain = toSpawn.Random();
            captain.SetRole(RoleType.NtfCaptain);
            toSpawn.Remove(captain);
            --units;

            int idx = 0;
            bool hasRole;
            foreach (Pawn player in toSpawn)
            {
                if (idx >= units)
                    break;

                hasRole = false;
                foreach (CustomRole customRole in customRoles)
                {
                    if (!customRole.CanSpawnByProbability())
                        continue;

                    player.SetRole(customRole);
                    hasRole = true;
                    break;
                }

                if (!hasRole)
                    player.SetRole(DefaultNtfRoles.Random());

                yield return player;
                idx++;
            }

            Respawn.SummonNtfChopper();
        }

        /// <summary>
        /// Forces the respawn given a <see cref="Team"/>.
        /// </summary>
        /// <param name="team">The team to be forced to respawn.</param>
        public void ForceRespawn(SpawnableTeamType team)
        {
            if (team is not SpawnableTeamType.NineTailedFox and not SpawnableTeamType.ChaosInsurgency)
                return;

            IEnumerable<CustomTeam> customTeams = CustomTeam.Registered.Where(t =>
            t.RespawnTeam == (team is not SpawnableTeamType.NineTailedFox ? SpawnableTeamType.NineTailedFox : SpawnableTeamType.ChaosInsurgency));

            CustomTeam customTeam = null;

            foreach (CustomTeam toSpawn in customTeams)
            {
                if ((!toSpawn.CanSpawnWithoutScps && !IsAlive(Team.SCPs)) ||
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

        /// <summary>
        /// Checks whether the specified <see cref="RoleType"/> is alive.
        /// </summary>
        /// <param name="roleType">The role to check.</param>
        /// <returns><see langword="true"/> if the role is alive; otherwise, <see langword="false"/>.</returns>
        public bool IsAlive(RoleType roleType) => Player.List.Any(x => x is Pawn p && !p.HasCustomRole && x.Role.Type == roleType);

        /// <summary>
        /// Checks whether the specified <see cref="CustomRole.Id"/> is alive.
        /// </summary>
        /// <param name="customRoleType">The role to check.</param>
        /// <returns><see langword="true"/> if the role is alive; otherwise, <see langword="false"/>.</returns>
        public bool IsAlive(uint customRoleType) => Player.List.Any(x => x is Pawn p && p.HasCustomRole && p.CustomRole.Id is uint value && value == customRoleType);

        /// <summary>
        /// Checks whether the specified <see cref="Team"/>[] are alive.
        /// </summary>
        /// <param name="teams">The teams to check.</param>
        /// <returns><see langword="true"/> if at least a team is alive; otherwise, <see langword="false"/>.</returns>
        public bool IsAlive(params Team[] teams)
        {
            foreach (Team team in teams)
            {
                foreach (Player player in Player.List)
                {
                    if (!CustomTeam.TryGet(player, out CustomTeam customTeam) ||
                        !customTeam.LeadingTeams.Contains(team))
                        continue;

                    return true;
                }

                if (Player.Get(team).Any())
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Spawning += OnSpawning;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Spawning -= OnSpawning;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
        }

        private void OnSpawning(SpawningEventArgs ev)
        {
            Timing.CallDelayed(0.25f, () =>
            {
                if (CustomRole.TryGet(ev.Player, out CustomRole customRole) && customRole.IsScp)
                {
                    if (!SpawnedCustomScps.Contains(customRole.Id))
                        SpawnedCustomScps.Add(customRole.Id);

                    if (!CustomScpsSpawnPositions.ContainsKey(customRole.Id))
                        CustomScpsSpawnPositions.Add(customRole.Id, ev.Position);
                }
                else if (ev.Player.Role.Team is Team.SCPs)
                {
                    if (!SpawnedScps.Contains(RoleType.Cast(ev.Player.Role.Type)))
                        SpawnedScps.Add(RoleType.Cast(ev.Player.Role.Type));

                    if (!ScpsSpawnPositions.ContainsKey(RoleType.Cast(ev.Player.Role.Type)))
                        ScpsSpawnPositions.Add(RoleType.Cast(ev.Player.Role.Type), ev.Position);
                }
            });
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (CustomRole.TryGet(ev.Player, out CustomRole customRole) && customRole.IsScp)
            {
                ContainedCustomScps.Add(customRole.Id);
                CustomScpsDeathPositions.Remove(customRole.Id);
                CustomScpsDeathPositions.Add(customRole.Id, ev.Player.Position);
            }
            else if (ev.Player.Role.Team is Team.SCPs)
            {
                ContainedScps.Add(RoleType.Cast(ev.Player.Role.Type));
                ScpsDeathPositions.Remove(RoleType.Cast(ev.Player.Role.Type));
                ScpsDeathPositions.Add(RoleType.Cast(ev.Player.Role.Type), ev.Player.Position);
            }
        }

        private void OnRoundStarted()
        {
            _respawnCoreHandle = Timing.RunCoroutine(RespawnHandler());
            _respawnHandle.Add(Timing.RunCoroutine(RespawnStateCheck()));
            _respawnHandle.Add(Timing.RunCoroutine(RespawnTimerHandler()));
        }

        private void OnRoundEnded(RoundEndedEventArgs _)
        {
            Timing.KillCoroutines(_respawnCoreHandle);
            Timing.KillCoroutines(_respawnHandle.ToArray());
            _respawnHandle.Clear();
            State = RespawnStateBase.Enabled;
        }

        private IEnumerator<float> RespawnHandler()
        {
            TimeUntilNextRespawn = DefaultRespawnCooldown;

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

        private IEnumerator<float> RespawnStateCheck()
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

        private IEnumerator<float> RespawnTimerHandler()
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
