// -----------------------------------------------------------------------
// <copyright file="PlayerScriptController.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Controllers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// A controller to be used with any type of playable character component.
    /// </summary>
    public abstract class PlayerScriptController : ObjectController
    {
        /// <summary>
        /// Gets or sets the owner of the <see cref="PlayerScriptController"/>.
        /// </summary>
        public abstract Player Owner { get; protected set; }

        /// <inheritdoc/>
        protected override void Awake()
        {
            SubscribeEvents();
            Owner = Player.Get(gameObject);
        }

        /// <inheritdoc/>
        protected override void Start()
        {
        }

        /// <inheritdoc/>
        protected override void FixedUpdate()
        {
            if (Owner is null)
                Destroy();
        }

        /// <inheritdoc/>
        protected override void PartiallyDestroy()
        {
            UnsubscribeEvents();

            if (Owner is null)
                return;
        }

        private void OnDestroy() => PartiallyDestroy();

        /// <inheritdoc/>
        protected override void SubscribeEvents() => Exiled.Events.Handlers.Player.Destroying += OnDestroying;

        /// <inheritdoc/>
        protected override void UnsubscribeEvents() => Exiled.Events.Handlers.Player.Destroying -= OnDestroying;

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
        public bool Check(Player player) => player is not null && Owner == player;
    }
}
