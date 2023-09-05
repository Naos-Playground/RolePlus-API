// -----------------------------------------------------------------------
// <copyright file="RoundManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Map;
    using global::RolePlus.ExternModule.API.Engine.Components;
<<<<<<< HEAD
<<<<<<< HEAD
=======
    using global::RolePlus.ExternModule.API.Engine.Framework.Generic;
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75
=======
>>>>>>> d56c47334965da96730d299937b03a57f2fbe373
    using MEC;
    using PlayerRoles;
    using Respawning;
    using RolePlus.ExternModule.API.Engine.Framework;
    using UnityEngine;

    /// <summary>
    /// A class to easily manage round features.
    /// </summary>
    public sealed class RoundManager : StaticActor
    {
        private static CoroutineHandle _checkRoundHandle;
        private static Side _leadingTeam = Side.None;
        private static bool _roundLocked;
        private static bool _explosionDestroysEntities;
        private static RespawnManager _respawnManager;

        /// <summary>
        /// Gets the <see cref="Features.RespawnManager"/>.
        /// </summary>
        public RespawnManager RespawnManager => _respawnManager ??= Get<RespawnManager>();

        /// <summary>
        /// Gets or sets a value indicating whether the round is locked.
        /// </summary>
        public bool IsLocked
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
        public bool CanExplosionDestroyEntities
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
        public string CurrentGamemode { get; set; }

        /// <summary>
        /// Gets the leading team.
        /// </summary>
        public Side LeadingTeam
        {
            get
            {
                if (RespawnManager.IsAlive(Team.FoundationForces, Team.Scientists) && !RespawnManager.IsAlive(Team.ClassD, Team.ChaosInsurgency, Team.SCPs, Team.OtherAlive))
                    _leadingTeam = Side.Mtf;
                else if (RespawnManager.IsAlive(Team.ChaosInsurgency, Team.ClassD) && !RespawnManager.IsAlive(Team.FoundationForces, Team.Scientists, Team.SCPs, Team.OtherAlive))
                    _leadingTeam = Side.ChaosInsurgency;
                else if (RespawnManager.IsAlive(Team.SCPs) && !RespawnManager.IsAlive(Team.FoundationForces, Team.Scientists, Team.ChaosInsurgency, Team.ClassD, Team.OtherAlive))
                    _leadingTeam = Side.Scp;
                else if (RespawnManager.IsAlive(Team.FoundationForces, Team.Scientists, Team.ChaosInsurgency, Team.ClassD, Team.SCPs))
                    _leadingTeam = Side.Tutorial;
                else
                    _leadingTeam = Side.None;

                return _leadingTeam;
            }
        }

        /// <summary>
        /// Starts the ending conditions check process.
        /// </summary>
        public void Start() => _checkRoundHandle = Timing.RunCoroutine(EndingRoundConditionsValidator());

        /// <summary>
        /// Stops the ending conditions check process.
        /// </summary>
        public void Stop() => Timing.KillCoroutines(_checkRoundHandle);

        /// <summary>
        /// Ends the round.
        /// </summary>
        public void EndRound()
        {
            IsLocked = false;
            Timing.CallDelayed(0.2f, () => Round.EndRound(true));
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!CanExplosionDestroyEntities || ev.Projectile.Type is not ItemType.GrenadeHE)
                return;

            Vector3 explosionRadius = ev.Projectile.Position;
            foreach (Ragdoll ragdoll in Ragdoll.List.Where(x => Vector3.Distance(x.Position, explosionRadius) <= 4))
                ragdoll.Destroy();

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

        private IEnumerator<float> EndingRoundConditionsValidator()
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