// -----------------------------------------------------------------------
// <copyright file="RoundManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Map;

    using global::RolePlus.ExternModule.API.Engine.Components;
    using global::RolePlus.ExternModule.API.Engine.Core;

    using MEC;

    using PlayerRoles;

    using UnityEngine;

    /// <summary>
    /// A class to easily manage round features.
    /// </summary>
    public sealed class RoundManager
    {
        private static CoroutineHandle _checkRoundHandle;
        private static Side _leadingTeam = Side.None;
        private static bool _roundLocked;
        private static bool _explosionDestroysEntities;

        /// <summary>
        /// Gets or sets a value indicating whether the round is locked.
        /// </summary>
        public static bool IsLocked
        {
            get => _roundLocked;
            set
            {
                _roundLocked = value;
                Round.IsLocked = _roundLocked;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether explosions can destroy entities.
        /// </summary>
        public static bool CanExplosionDestroyEntities
        {
            get => _explosionDestroysEntities;
            set
            {
                if (value == _explosionDestroysEntities)
                    return;

                _explosionDestroysEntities = value;
                if (_explosionDestroysEntities)
                    Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
                else
                    Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            }
        }

        /// <summary>
        /// Gets or sets the gamemode's name.
        /// </summary>
        public static string CurrentGamemode { get; set; }

        /// <summary>
        /// Gets the leading team.
        /// </summary>
        public static Side LeadingTeam
        {
            get
            {
                if (SpawnManager.IsAlive(Team.FoundationForces, Team.Scientists) && !SpawnManager.IsAlive(Team.ClassD, Team.ChaosInsurgency, Team.SCPs, Team.OtherAlive))
                    _leadingTeam = Side.Mtf;
                else if (SpawnManager.IsAlive(Team.ChaosInsurgency, Team.ClassD) && !SpawnManager.IsAlive(Team.FoundationForces, Team.Scientists, Team.SCPs, Team.OtherAlive))
                    _leadingTeam = Side.ChaosInsurgency;
                else if (SpawnManager.IsAlive(Team.SCPs) && !SpawnManager.IsAlive(Team.FoundationForces, Team.Scientists, Team.ChaosInsurgency, Team.ClassD, Team.OtherAlive))
                    _leadingTeam = Side.Scp;
                else if (SpawnManager.IsAlive(Team.OtherAlive) && !SpawnManager.IsAlive(Team.FoundationForces, Team.Scientists, Team.ChaosInsurgency, Team.ClassD, Team.SCPs))
                    _leadingTeam = Side.Tutorial;
                else
                    _leadingTeam = Side.None;

                return _leadingTeam;
            }
        }

        /// <summary>
        /// Starts the ending conditions check process.
        /// </summary>
        public static void Start() => _checkRoundHandle = Timing.RunCoroutine(EndingRoundConditionsValidator());

        /// <summary>
        /// Stops the ending conditions check process.
        /// </summary>
        public static void Stop() => Timing.KillCoroutines(_checkRoundHandle);

        /// <summary>
        /// Ends the round.
        /// </summary>
        public static void EndRound()
        {
            IsLocked = false;
            Timing.CallDelayed(0.2f, () => Round.EndRound(true));
        }

        private static void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!CanExplosionDestroyEntities)
                return;

            Vector3 explosionRadius = ev.Projectile.Base.Position;
            foreach (Ragdoll ragdoll in Ragdoll.List.Where(x => Vector3.Distance(x.Position, explosionRadius) <= 4))
                ragdoll.Destroy();

            foreach (Pickup item in Pickup.List.Where(x => Vector3.Distance(x.Position, explosionRadius) <= 4))
            {
                AInteractableFrameComponent[] frames = UObject.FindActiveObjectsOfType<AInteractableFrameComponent>();
                if (frames.Any(i => i.Pickups.Contains(item)))
                    continue;

                if (CustomItem.TryGet(item, out CustomItem customItem))
                    customItem.Destroy();

                item.Destroy();
            }
        }

        private static IEnumerator<float> EndingRoundConditionsValidator()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(5f);

                if (IsLocked || (Player.List.Count(x => x.IsAlive) > 1 && LeadingTeam is Side.None))
                    continue;

                EndRound();
                yield break;
            }
        }
    }
}