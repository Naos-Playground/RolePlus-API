// -----------------------------------------------------------------------
// <copyright file="AEnvironmentActorHealthComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DamageHandlers;

    using MapEditorReborn.API.Features.Objects;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A class that handles in-game interactable objects by.
    /// </summary>
    public abstract class AEnvironmentActorHealthComponent : AActorHealthComponent, IDestructible
    {
        private bool _isDestroyed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AEnvironmentActorHealthComponent"/> class.
        /// </summary>
        public AEnvironmentActorHealthComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AEnvironmentActorHealthComponent"/> class.
        /// </summary>
        /// <param name="component"><inheritdoc cref="AActorFrameComponent.RootComponent"/></param>
        /// <param name="maxHealth"><inheritdoc cref="AActorHealthComponent.MaxHealth"/></param>
        /// <param name="curHealth"><inheritdoc cref="AActorHealthComponent.CurHealth"/></param>
        protected AEnvironmentActorHealthComponent(EActor component, float maxHealth, float curHealth)
            : this()
        {
            if (!component.Cast(out AEnvironmentMeshComponent envComponent))
            {
                Destroy();
                return;
            }

            RootComponent = envComponent;
            MaxHealth = maxHealth;
            CurHealth = curHealth;
            NetworkId = RootComponent.Base.GetComponent<NetworkIdentity>().netId;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AEnvironmentMeshComponent"/> has been damaged and destroyed.
        /// </summary>
        public bool IsDestroyed => _isDestroyed;

        /// <inheritdoc/>
        public uint NetworkId { get; }

        /// <inheritdoc/>
        public Vector3 CenterOfMass => Position;

        /// <summary>
        /// Fired when the actor is damaged.
        /// </summary>
        /// <param name="amount">The amount of damage.</param>
        /// <param name="damageHandler">The damage handler.</param>
        /// <param name="exactHitPos">The hit position.</param>
        /// <returns><see langword="true"/> if the actor was successfully damaged; otherwise, <see langword="false"/>.</returns>;
        protected abstract bool OnDamagedActor(float amount, DamageHandlerBase damageHandler, Vector3 exactHitPos);

        /// <inheritdoc/>
        public override bool Damage(float amount)
        {
            if (CurHealth == -1f)
                return false;

            return !Damage(amount, null, default);
        }

        /// <summary>
        /// Damages the <see cref="AActorHealthComponent.RootComponent"/>.
        /// </summary>
        /// <param name="amount">The damage amount.</param>
        /// <param name="damageHandler">The damage handler.</param>
        /// <param name="exactHitPos">The hit position.</param>
        /// <returns><see langword="true"/> if the <see cref="AActorHealthComponent.RootComponent"/> was successfully damaged; otherwise, <see langword="false"/>.</returns>
        public virtual bool Damage(float amount, DamageHandlerBase damageHandler, Vector3 exactHitPos)
        {
            if (CurHealth == -1f)
                return false;

            if (!OnDamagingActor(amount))
                return false;

            CurHealth -= amount;
            if (CurHealth <= 0f)
            {
                if (!_isDestroyed)
                {
                    foreach (GameObject gameObject in RootComponent.Cast<AEnvironmentMeshComponent>().Blocks)
                    {
                        if (!gameObject.TryGetComponent(out PrimitiveObject primitiveObject))
                            continue;

                        primitiveObject.Rigidbody.isKinematic = false;
                    }

                    DestroyEffects();
                    _isDestroyed = true;
                }
            }

            if (damageHandler is null ||
                !damageHandler.Is(out PlayerStatsSystem.AttackerDamageHandler handler) ||
                !OnDamagedActor(amount, damageHandler, exactHitPos))
                return false;

            foreach (GameObject gameObject in RootComponent.Cast<AEnvironmentMeshComponent>().Blocks)
            {
                if (!gameObject.TryGetComponent(out PrimitiveObject primitiveObject))
                    continue;

                primitiveObject.Rigidbody.AddForceAtPosition(
                    Player.Get(handler.Attacker.Hub).CameraTransform.forward * amount * 10f, exactHitPos);
            }

            return true;
        }

        /// <summary>
        /// Fired when the health is reduced to zero or lower.
        /// </summary>
        public abstract void DestroyEffects();

        /// <summary>
        /// Forces the destroy action.
        /// </summary>
        public void ForceDestroy() => Damage(CurHealth);

        /// <inheritdoc/>
        public bool Damage(float damage, PlayerStatsSystem.DamageHandlerBase handler, Vector3 exactHitPos) => Damage(damage, handler, exactHitPos);
    }
}
