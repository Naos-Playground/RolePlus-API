// -----------------------------------------------------------------------
// <copyright file="PlayerBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Controllers
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// A controller to be used with any type of playable character component.
    /// </summary>
    public abstract class PlayerBehaviour : EActor
    {
        /// <summary>
        /// Gets or sets the owner of the <see cref="PlayerBehaviour"/>.
        /// </summary>
        protected virtual Player Owner { get; private set; }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            Owner = Player.Get(Base);
            if (Owner is null)
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (Owner is null)
                Destroy();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            if (Owner is null)
                return;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        protected virtual void OnDestroying(DestroyingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            Destroy();
        }

        /// <summary>
        /// Checks whether the given <see cref="Player"/> is the <see cref="Owner"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the <see cref="Player"/> is the <see cref="Owner"/>; otherwise, <see langword="false"/>.</returns>
        protected virtual bool Check(Player player) => player is not null && Owner == player;
    }
}
