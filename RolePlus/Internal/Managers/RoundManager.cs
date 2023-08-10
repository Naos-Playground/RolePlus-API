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
    using Exiled.CustomItems.API.Features;

    using global::RolePlus.ExternModule.API.Engine.Components;

    using MEC;

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
                if (SpawnManager.IsAlive(Team.MTF, Team.RSC) && !SpawnManager.IsAlive(Team.CDP, Team.CHI, Team.SCP, Team.TUT))
                    _leadingTeam = Side.Mtf;
                else if (SpawnManager.IsAlive(Team.CHI, Team.CDP) && !SpawnManager.IsAlive(Team.MTF, Team.RSC, Team.SCP, Team.TUT))
                    _leadingTeam = Side.ChaosInsurgency;
                else if (SpawnManager.IsAlive(Team.SCP) && !SpawnManager.IsAlive(Team.MTF, Team.RSC, Team.CHI, Team.CDP, Team.TUT))
                    _leadingTeam = Side.Scp;
                else if (SpawnManager.IsAlive(Team.TUT) && !SpawnManager.IsAlive(Team.MTF, Team.RSC, Team.CHI, Team.CDP, Team.SCP))
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
            if (!CanExplosionDestroyEntities || ev.GrenadeType is not GrenadeType.FragGrenade)
                return;

            Vector3 explosionRadius = ev.Grenade.transform.position;
            foreach (Ragdoll ragdoll in Map.Ragdolls.Where(x => Vector3.Distance(x.Position, explosionRadius) <= 4))
                ragdoll.Delete();

            foreach (Pickup item in Pickup.List.Where(x => Vector3.Distance(x.Position, explosionRadius) <= 4))
            {
                AInteractableFrameComponent[] frames = EObject.FindActiveObjectsOfType<AInteractableFrameComponent>();
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