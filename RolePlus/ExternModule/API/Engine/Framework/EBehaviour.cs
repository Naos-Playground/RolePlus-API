// -----------------------------------------------------------------------
// <copyright file="EBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.Events.EventArgs.Player;
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;

    /// <summary>
    /// A component to be used with any type of playable character component.
    /// </summary>
    public abstract class EBehaviour : EActor, IAddittiveProperty
    {
        /// <summary>
        /// Gets the owner of the <see cref="EBehaviour"/>.
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

            BehaviourUpdate_Implementation();
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

            ExternModule.Events.EventManager.CreateFromTypeInstance(this);

            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            ExternModule.Events.EventManager.UnbindAllFromTypeInstance(this);

            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
        }

        /// <summary>
        /// Fired every tick.
        /// <para>Code affecting the <see cref="EBehaviour"/>'s base implementation should be placed here.</para>
        /// </summary>
        protected virtual void BehaviourUpdate()
        {
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

        /// <inheritdoc cref="BehaviourUpdate"/>
        private protected virtual void BehaviourUpdate_Implementation()
        {
            if (Owner is null)
                Destroy();

            BehaviourUpdate();
        }
    }
}
