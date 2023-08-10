// -----------------------------------------------------------------------
// <copyright file="AEnvironmentMeshComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
  
    using InventorySystem.Items.Pickups;

    using MapEditorReborn.API.Features.Objects;
    using MapEditorReborn.API.Features.Serializable;

    using MEC;

    using Mirror;

    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;
    using RolePlus.ExternModule.API.Engine.Framework.Structs;

    using UnityEngine;

    /// <summary>
    /// A class that handles in-game objects destinated to be used as environmental objects.
    /// </summary>
    public abstract class AEnvironmentMeshComponent : ASchematicMeshComponent
    {
        private bool _canInteract = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AEnvironmentMeshComponent"/> class.
        /// </summary>
        protected AEnvironmentMeshComponent()
            : base()
        {
        }

        /// <inheritdoc/>
        protected AEnvironmentMeshComponent(GameObject gameObject = null)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AEnvironmentMeshComponent"/> class.
        /// </summary>
        /// <param name="mesh"><inheritdoc cref="ASchematicMeshComponent.RootSchematic"/></param>
        protected AEnvironmentMeshComponent(SchematicObject mesh)
            : base(mesh, mesh.Scale, mesh.Position, mesh.Rotation)
        {
        }

        /// <inheritdoc/>
        protected AEnvironmentMeshComponent(
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(mesh, scale, position, rotation)
        {
        }

        /// <inheritdoc/>
        protected AEnvironmentMeshComponent(
            FTransform socket,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(socket, mesh, scale, position, rotation)
        {
        }

        /// <inheritdoc/>
        protected AEnvironmentMeshComponent(
            Vector3 localPosition,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(localPosition, mesh, scale, position, rotation)
        {
        }

        /// <inheritdoc/>
        protected AEnvironmentMeshComponent(
            Vector3 localPosition,
            Quaternion localRotation,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(localPosition, localRotation, mesh, scale, position, rotation)
        {
        }

        /// <inheritdoc/>
        protected AEnvironmentMeshComponent(
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(localPosition, localRotation, localScale, mesh, scale, position, rotation)
        {
        }

        /// <summary>
        /// Gets the <see cref="SchematicObject"/> base object.
        /// </summary>
        public SchematicSerializable SerializedObject => RootSchematic.Base;

        /// <summary>
        /// Gets the <see cref="SchematicObjectDataList"/> base object.
        /// </summary>
        public SchematicObjectDataList Data => RootSchematic.SchematicData;

        /// <summary>
        /// Gets a value indicating whether the object is the root schematic.
        /// </summary>
        public bool IsRoot => RootSchematic.IsRootSchematic;

        /// <summary>
        /// Gets the relative position.
        /// </summary>
        public Vector3 RelativePosition => RootSchematic.RelativePosition;

        /// <summary>
        /// Gets the relative rotation.
        /// </summary>
        public Vector3 RelativeRotation => RootSchematic.RelativeRotation;

        /// <summary>
        /// Gets a <see cref="ObservableCollection{T}"/> of <see cref="GameObject"/> which contains all the parts of the <see cref="AEnvironmentMeshComponent"/>.
        /// </summary>
        public ObservableCollection<GameObject> Blocks => RootSchematic.AttachedBlocks;

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> of <see cref="NetworkIdentity"/> which contains all the parts' identity of the <see cref="AEnvironmentMeshComponent"/>.
        /// </summary>
        public ReadOnlyCollection<NetworkIdentity> NetworkIdentities => RootSchematic.NetworkIdentities;

        /// <summary>
        /// Gets the <see cref="MapEditorReborn.API.Features.AnimationController"/>.
        /// </summary>
        public MapEditorReborn.API.Features.AnimationController AnimationController => RootSchematic.AnimationController;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Enums.RoomType"/> of the <see cref="AEnvironmentMeshComponent"/>.
        /// </summary>
        public RoomType RoomType => RootSchematic.RoomType;

        /// <summary>
        /// Gets a value indicating whether the <see cref="AEnvironmentMeshComponent"/> is rotable.
        /// </summary>
        public bool IsRotatable => RootSchematic.IsRotatable;

        /// <summary>
        /// Gets a value indicating whether the <see cref="AEnvironmentMeshComponent"/> is scalable.
        /// </summary>
        public bool IsScalable => RootSchematic.IsScalable;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Room"/> in which the <see cref="AEnvironmentMeshComponent"/> is located.
        /// </summary>
        public Room Room => RootSchematic.CurrentRoom;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="AEnvironmentMeshComponent"/> is interactable.
        /// </summary>
        public virtual bool IsInteractable { get; set; }

        /// <summary>
        /// Gets or sets the interaction cooldown which determines whether an interaction can be computed.
        /// </summary>
        public virtual float InteractionCooldown { get; set; } = 2.5f;

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="Pickup"/> containing all the existing pickups related to this <see cref="AEnvironmentMeshComponent"/> instance.
        /// </summary>
        public HashSet<AInteractableFrameComponent> InteractableFrames { get; } = new();

        /// <summary>
        /// Updates all the properties of the current <see cref="AEnvironmentMeshComponent"/> instance.
        /// </summary>
        public void UpdateObject() => RootSchematic.UpdateObject();

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            List<Pickup> pickups = new();
            foreach (GameObject gameObject in RootSchematic.AttachedBlocks)
            {
                if (gameObject.TryGetComponent(out ItemPickupBase ipb))
                    pickups.Add(Pickup.Get(ipb));
            }

            InteractableFrames.Add(
                CreateDefaultSubobject<AInteractableFrameComponent>(
                    Base, $"{GetType().Name}-Frame#{InteractableFrames.Count}", pickups, this));

            Framework.Events.Handlers.Player.InteractingObject += CheckInteraction;
            Framework.Events.Handlers.EObject.ChangingAnimationState += OnChangingAnimationState;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            InteractableFrames.Clear();
            Framework.Events.Handlers.Player.InteractingObject -= CheckInteraction;
            Framework.Events.Handlers.EObject.ChangingAnimationState -= OnChangingAnimationState;
        }

        /// <inheritdoc cref="Framework.Events.Handlers.Player.OnInteractingObject(InteractingObjectEventArgs)"/>
        protected virtual void OnInteractingObject(InteractingObjectEventArgs ev)
        {
        }

        /// <inheritdoc cref="Framework.Events.Handlers.Player.OnInteractingObject(InteractingObjectEventArgs)"/>
        private void CheckInteraction(InteractingObjectEventArgs ev)
        {
            if (!InteractableFrames.Contains(ev.InteractableFrame) || !_canInteract || !IsInteractable)
                return;

            _canInteract = false;
            Timing.CallDelayed(InteractionCooldown, () => _canInteract = true);
            OnInteractingObject(ev);
        }

        /// <inheritdoc/>
        public void UpdateInteractableItems(bool shouldBeSpawned, params string[] animStates)
        {
            if (RootSchematic is null)
                return;

            try
            {
                foreach (string animState in animStates)
                {
                    if (!GetCurrentAnimation(RootSchematic.AnimationController.Animators[0]).IsName(animState))
                        continue;

                    return;
                }

                foreach (GameObject gameObject in RootSchematic.AttachedBlocks)
                {
                    if (gameObject is null || !gameObject.TryGetComponent(out ItemPickupBase _))
                        return;

                    if (shouldBeSpawned)
                        NetworkServer.Spawn(gameObject);
                    else
                        NetworkServer.UnSpawn(gameObject);
                }
            }
            catch
            {
            }
        }
    }
}
