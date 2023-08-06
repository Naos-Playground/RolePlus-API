// -----------------------------------------------------------------------
// <copyright file="ItemMeshComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


namespace RolePlus.ExternModule.API.Features.CustomSkins
{
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MapEditorReborn.API.Features.Objects;

    using MEC;

    using RolePlus.ExternModule.API.Engine.Components;
    using RolePlus.ExternModule.API.Engine.Framework;

    using UnityEngine;

    /// <summary>
    /// A class to implement custom meshes for <see cref="Item"/>s.
    /// </summary>
    public class ItemMeshComponent : ASchematicMeshComponent
    {
        private bool _isPickup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMeshComponent"/> class.
        /// </summary>
        /// <param name="mesh"><inheritdoc cref="ASchematicMeshComponent.RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="AActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="AActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="AActor.Rotation"/></param>
        protected ItemMeshComponent(
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(mesh, scale, position, rotation)
        {
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        public Player Owner { get; private set; }

        /// <summary>
        /// Gets the <see cref="Item"/> mesh.
        /// </summary>
        public Item ItemMesh { get; private set; }

        /// <summary>
        /// Gets the <see cref="Pickup"/> mesh.
        /// </summary>
        public Pickup PickupMesh { get; private set; }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;

            if (Base.TryGetComponent(out ItemBase itemComp))
                ItemMesh = Item.Get(itemComp);
            else if (Base.TryGetComponent(out ItemPickupBase ipb))
                PickupMesh = Pickup.Get(ipb);

            if (ItemMesh is not null || PickupMesh is not null)
                ShowBones();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            if (!IsVisible)
                return;

            if (_isPickup)
            {
                RootSchematic.Position = PickupMesh.Base.transform.position + Socket.Translation;
                RootSchematic.Rotation = Quaternion.Euler(
                    (Vector3.up * PickupMesh.Base.transform.rotation.eulerAngles.y) +
                    Socket.Rotation.eulerAngles);
                RootSchematic.Scale = Socket.Scale;
                return;
            }

            RootSchematic.Position = ItemMesh.Base.transform.position + Socket.Translation;
            RootSchematic.Rotation = Quaternion.Euler(
                (Vector3.up * ItemMesh.Base.transform.rotation.eulerAngles.y) +
                Socket.Rotation.eulerAngles);
            RootSchematic.Scale = Socket.Scale;
        }

        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player != Owner || ev.Item != ItemMesh ||
                CustomItem.TryGet(ev.Item, out _))
                return;

            ev.IsAllowed = false;
            PickupMesh = ev.Item.CreatePickup(ev.Player.Position);
            ev.Player.RemoveItem(ev.Item, true);

            _isPickup = true;
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Player != Owner || ev.Pickup != PickupMesh ||
                CustomItem.TryGet(ev.Pickup, out _))
                return;

            _isPickup = false;
            Timing.CallDelayed(0.1f, () => ItemMesh = ev.Player.Items.Last());
        }
    }
}
