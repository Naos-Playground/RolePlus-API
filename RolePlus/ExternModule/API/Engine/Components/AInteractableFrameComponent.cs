// -----------------------------------------------------------------------
// <copyright file="AInteractableFrameComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Player;

    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;

    /// <summary>
    /// A class that handles in-game interactable objects by.
    /// </summary>
    public class AInteractableFrameComponent : AActorFrameComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AInteractableFrameComponent"/> class.
        /// </summary>
        public AInteractableFrameComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AInteractableFrameComponent"/> class.
        /// </summary>
        /// <param name="pickups"><inheritdoc cref="Pickups"/></param>
        /// <param name="component"><inheritdoc cref="RootComponent"/></param>
        public AInteractableFrameComponent(IEnumerable<Pickup> pickups, AActor component)
            : base(component) => Pickups = pickups.ToList();

        /// <summary>
        /// Gets or sets the root <see cref="AActor"/>.
        /// </summary>
        public override AActor RootComponent { get; protected set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="Pickup"/> containing all the managed pickups.
        /// </summary>
        public List<Pickup> Pickups { get; set; } = new();

        /// <summary>
        /// Checks whether the given <see cref="Pickup"/> object is managed by the current <see cref="AInteractableFrameComponent"/> instance.
        /// </summary>
        /// <param name="pickup">The pickup to check.</param>
        /// <returns><see langword="true"/> if the pickup is a <see cref="AInteractableFrameComponent"/>; otherwise, <see langword="false"/>.</returns>
        public bool Check(Pickup pickup) => Pickups.Contains(pickup);

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Exiled.Events.Handlers.Player.SearchingPickup += FrameInteraction;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Exiled.Events.Handlers.Player.SearchingPickup -= FrameInteraction;
        }

        private void FrameInteraction(SearchingPickupEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.IsAllowed = false;
            InteractingObjectEventArgs interactingEv = new(ev.Player, this);
            Framework.Events.Handlers.Player.OnInteractingObject(interactingEv);
        }
    }
}
