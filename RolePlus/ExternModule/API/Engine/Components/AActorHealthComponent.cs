// -----------------------------------------------------------------------
// <copyright file="AActorHealthComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using RolePlus.ExternModule.API.Engine.Framework;

    /// <summary>
    /// A class that handles in-game interactable objects by.
    /// </summary>
    public abstract class AActorHealthComponent : AActorFrameComponent
    {
        private float _maxHealth;
        private float _curHealth;
        private bool _isDamageable;

        /// <summary>
        /// Initializes a new instance of the <see cref="AActorHealthComponent"/> class.
        /// </summary>
        public AActorHealthComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AActorHealthComponent"/> class.
        /// </summary>
        /// <param name="component"><inheritdoc cref="AActorFrameComponent.RootComponent"/></param>
        /// <param name="maxHealth"><inheritdoc cref="MaxHealth"/></param>
        /// <param name="curHealth"><inheritdoc cref="CurHealth"/></param>
        protected AActorHealthComponent(AActor component, float maxHealth, float curHealth)
            : base(component)
        {
            _maxHealth = maxHealth;
            _curHealth = curHealth;
        }

        /// <inheritdoc/>
        public override AActor RootComponent { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RootComponent"/> is damageable.
        /// </summary>
        public virtual bool IsDamageable
        {
            get => _isDamageable;
            set => _isDamageable = value;
        }

        /// <summary>
        /// Gets or sets the max health.
        /// </summary>
        public virtual float MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        /// <summary>
        /// Gets or sets the current health.
        /// </summary>
        public virtual float CurHealth
        {
            get => _curHealth;
            set => _curHealth = value;
        }

        /// <summary>
        /// Damages the <see cref="RootComponent"/>.
        /// </summary>
        /// <param name="amount">The damage amount.</param>
        /// <returns><see langword="true"/> if the <see cref="RootComponent"/> was successfully damaged; otherwise, <see langword="false"/>.</returns>
        public abstract bool Damage(float amount);

        /// <summary>
        /// Fired when the <see cref="RootComponent"/> takes damage.
        /// </summary>
        /// <param name="amount">The damage amount.</param>
        /// <returns><see langword="true"/> if the <see cref="RootComponent"/> was successfully damaged; otherwise, <see langword="false"/>.</returns>
        protected abstract bool OnDamagingActor(float amount);
    }
}
