// -----------------------------------------------------------------------
// <copyright file="SpawnManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using global::RolePlus.ExternModule;
    using global::RolePlus.ExternModule.API.Enums;
    using global::RolePlus.ExternModule.API.Features.CustomRoles;
    using global::RolePlus.ExternModule.API.Features.CustomTeams;

    using MEC;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// A class which keeps track of every action which involves the players' current state.
    /// </summary>
    public sealed class SpawnManager
    {
        /// <summary>
        /// Gets all the SCPs' death position.
        /// </summary>
        public static Dictionary<RoleType, Vector3> ScpsDeathPositions { get; } = new();

        /// <summary>
        /// Gets all the SCPs' spawn position.
        /// </summary>
        public static Dictionary<RoleType, Vector3> ScpsSpawnPositions { get; } = new();

        /// <summary>
        /// Gets all the custom SCPs' death position.
        /// </summary>
        public static Dictionary<object, Vector3> CustomScpsDeathPositions { get; } = new();

        /// <summary>
        /// Gets all the custom SCPs' spawn position.
        /// </summary>
        public static Dictionary<object, Vector3> CustomScpsSpawnPositions { get; } = new();

        /// <summary>
        /// Gets all the contained SCPs.
        /// </summary>
        public static List<RoleType> ContainedScps { get; } = new();

        /// <summary>
        /// Gets all the contained custom SCPs.
        /// </summary>
        public static List<object> ContainedCustomScps { get; } = new();

        /// <summary>
        /// Gets all the spawned SCPs.
        /// </summary>
        public static List<RoleType> SpawnedScps { get; } = new();

        /// <summary>
        /// Gets all the spawned custom SCPs.
        /// </summary>
        public static List<object> SpawnedCustomScps { get; } = new();

        /// <summary>
        /// Checks whether the specified <see cref="RoleType"/> is alive.
        /// </summary>
        /// <param name="roleType">The role to check.</param>
        /// <returns><see langword="true"/> if the role is alive; otherwise, <see langword="false"/>.</returns>
        public static bool IsAlive(RoleType roleType) => Player.List.Any(x => !x.HasCustomRole() && x.Role.Type == roleType);

        /// <summary>
        /// Checks whether the specified <see cref="CustomRole.Id"/> is alive.
        /// </summary>
        /// <param name="customRoleType">The role to check.</param>
        /// <returns><see langword="true"/> if the role is alive; otherwise, <see langword="false"/>.</returns>
        public static bool IsAlive(uint customRoleType) => Player.List.Any(x => x.HasCustomRole() && x.GetCustomRoleType() is uint value && value == customRoleType);

        /// <summary>
        /// Checks whether the specified <see cref="Team"/>[] are alive.
        /// </summary>
        /// <param name="teams">The teams to check.</param>
        /// <returns><see langword="true"/> if at least a team is alive; otherwise, <see langword="false"/>.</returns>
        public static bool IsAlive(params Team[] teams)
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

        /// <summary>
        /// Starts the process.
        /// </summary>
        public static void Start()
        {
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Spawning += OnSpawning;
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public static void Stop()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Spawning -= OnSpawning;
        }

        private static void OnSpawning(SpawningEventArgs ev)
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

        private static void OnDied(DiedEventArgs ev)
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
    }
}
