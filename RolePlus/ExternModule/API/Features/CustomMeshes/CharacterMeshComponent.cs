// -----------------------------------------------------------------------
// <copyright file="CharacterMeshComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomMeshes
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using MapEditorReborn.API.Extensions;
    using MapEditorReborn.API.Features;

    using MEC;

    using Mirror;

    using RolePlus.ExternModule.API.Engine.Components;

    using UnityEngine;

    /// <inheritdoc/>
    public class CharacterMeshComponent : ASchematicMeshComponent
    {
        private bool _isVisible = true;
        private bool _isVisibleToOwner;
        private bool _isVisibleToOwnerOnly;
        private bool _hideLights;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMeshComponent"/> class.
        /// </summary>
        /// <param name="gameObject">The owner of the mesh.</param>
        /// <param name="meshName">The name of the mesh.</param>
        protected CharacterMeshComponent(GameObject gameObject, string meshName)
            : base(gameObject, meshName)
        {
        }

        /// <inheritdoc/>
        public override bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;

                foreach (Player player in Player.Get(p => p != Owner))
                    ChangeVisibility(player, _isVisible);
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ASchematicMeshComponent.RootSchematic"/> is visible to the owner.
        /// </summary>
        public bool IsVisibleToOwner
        {
            get => _isVisibleToOwner || !HiddenFromPlayers.Contains(Owner);
            set
            {
                if (value == _isVisibleToOwner)
                    return;

                ChangeVisibility(Owner, _isVisibleToOwner = value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ASchematicMeshComponent.RootSchematic"/> is visible to the owner only.
        /// </summary>
        public bool IsVisibleToOwnerOnly
        {
            get => _isVisibleToOwnerOnly;
            set
            {
                ChangeVisibility(Owner, _isVisibleToOwnerOnly = value);

                IsVisible = !_isVisibleToOwnerOnly;
                IsVisibleToOwner = _isVisibleToOwnerOnly;
            }
        }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="Player"/> containing all the players unable to see the mesh.
        /// </summary>
        public HashSet<Player> HiddenFromPlayers { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the owner is visible.
        /// </summary>
        public bool IsOwnerVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mesh should be destroyed on player's death.
        /// </summary>
        public bool DestroyOnDeath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mesh lights should be hidden.
        /// </summary>
        public bool HideLights
        {
            get => _hideLights;
            set
            {
                _hideLights = value;

                if (_hideLights)
                {
                    foreach (NetworkIdentity networkIdentity in RootSchematic.NetworkIdentities)
                    {
                        if (networkIdentity.gameObject.name.ToLower().Contains("light"))
                            Owner.DestroyNetworkIdentity(networkIdentity);
                    }
                }
                else
                {
                    foreach (NetworkIdentity networkIdentity in RootSchematic.NetworkIdentities)
                    {
                        if (networkIdentity.gameObject.name.ToLower().Contains("light"))
                            Owner.SpawnNetworkIdentity(networkIdentity);
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Owner = Player.Get(Base);
            RootSchematic = ObjectSpawner.SpawnSchematic(MeshName, Owner.Position);

            if (!HiddenFromPlayers.IsEmpty())
            {
                Timing.CallDelayed(1f, () =>
                {
                    foreach (Player player in HiddenFromPlayers)
                        ChangeVisibility(player, false);
                });
            }

            CharacterMeshComponent[] meshes = FindActiveObjectsOfType<CharacterMeshComponent>(meshComp => meshComp.MeshName == MeshName && meshComp.Owner == Owner);
            if (meshes.Length > 1)
                meshes[1].Destroy();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;

            RootSchematic.Destroy();
            HiddenFromPlayers.Clear();

            base.OnEndPlay();
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (Owner is null || (DestroyOnDeath && Owner.IsDead))
            {
                RootSchematic.Destroy();
                return;
            }

            if (!IsOwnerVisible && !Owner.GetEffect(EffectType.Invisible).IsEnabled)
                Owner.EnableEffect(EffectType.Invisible);
            else if (IsOwnerVisible)
                Owner.DisableEffect(EffectType.Invisible);

            RootSchematic.transform.position = Owner.Position + Socket.Translation;
            RootSchematic.transform.rotation = Quaternion.Euler(Vector3.up * (Owner.CameraTransform.eulerAngles.y + Socket.Rotation.eulerAngles.y));
            RootSchematic.transform.localScale = Socket.Scale;
            NetworkServer.UnSpawn(RootSchematic.gameObject);
            NetworkServer.Spawn(RootSchematic.gameObject);
        }

        /// <summary>
        /// Changes the mesh visibility for the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="isVisible">The new visibility state.</param>
        public void ChangeVisibility(Player target, bool isVisible)
        {
            if (target is null)
                return;

            if (isVisible)
            {
                HiddenFromPlayers.Remove(target);
                target.SpawnSchematic(RootSchematic);
            }
            else
            {
                if (HiddenFromPlayers.Contains(target))
                    return;

                HiddenFromPlayers.Add(target);
                target.DestroySchematic(RootSchematic);
            }
        }

        /// <summary>
        /// Changes the mesh visibility for the specified targets.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <param name="isVisible">The new visibility state.</param>
        public void ChangeVisibility(Func<Player, bool> predicate, bool isVisible)
        {
            foreach (Player target in Player.Get(predicate))
                ChangeVisibility(target, isVisible);
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            if (IsVisible)
                return;

            ChangeVisibility(ev.Player, false);
        }
    }
}
