// -----------------------------------------------------------------------
// <copyright file="AbilityBuilder.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using MEC;
    using RolePlus.ExternModule.API.Features.Controllers;

    /// <summary>
    /// CustomAbility is the base class used to create user-defined types treated as abilities applicable to a <see cref="Player"/>.
    /// </summary>
    public abstract class AbilityBuilder : PlayerBehaviour
    {
        private CoroutineHandle _abilityCooldownHandle;
        private byte _level;
        private bool _isActive;

        /// <summary>
        /// Gets or sets a value indicating whether the ability is ready.
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ability is active.
        /// <br>Value is changed only if the ability is duration-based.</br>
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                IsReady = _isActive;
            }
        }

        /// <summary>
        /// Gets or sets the level of the ability.
        /// </summary>
        public virtual byte Level
        {
            get => _level;
            set => OnLevelAdded(value);
        }

        /// <summary>
        /// Gets a value indicating whether the ability can enter the cooldown phase.
        /// </summary>
        protected virtual bool CanEnterCooldown => !IsReady;

        /// <summary>
        /// Gets a value indicating whether the ability can be used.
        /// </summary>
        protected virtual bool CanBeUsed => true;

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            _abilityCooldownHandle = Timing.RunCoroutine(AbilityCooldown());
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (OnAddingLevel(Level))
                Level += 1;

            if (IsActive)
                OnUsing();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Timing.KillCoroutines(_abilityCooldownHandle);
        }

        /// <summary>
        /// Fired when the ability is used.
        /// </summary>
        protected virtual void OnActivated()
        {
            if (!CanBeUsed)
            {
                if (!string.IsNullOrEmpty(DeniedActivationMessage))
                    Owner.ShowHint(DeniedActivationMessage, 4);

                return;
            }

            if (Duration == 0f)
            {
                IsReady = false;
                return;
            }

            IsActive = true;

            Timing.CallDelayed(Duration, () =>
            {
                IsReady = false;
                IsActive = false;
                OnExpired();
            });

            return;
        }

        /// <summary>
        /// Fired every tick when the ability is still active.
        /// </summary>
        protected virtual void OnUsing()
        {
        }

        /// <summary>
        /// Fired when the ability time's up.
        /// <para>Called using duration-based abilities.</para>
        /// </summary>
        protected virtual void OnExpired()
        {
        }

        /// <summary>
        /// Fired when the level is changed.
        /// </summary>
        /// <param name="level">The new level.</param>
        protected virtual void OnLevelAdded(byte level)
        {
            _level = level;
            Owner.ShowHint(NewLevelReachedMessage.Replace("%level", Level.ToString()), 5);
        }

        /// <summary>
        /// Fired before adding a new level.
        /// </summary>
        /// <param name="curLevel">The current level.</param>
        /// <returns><see langword="true"/> if the level can change; otherwise, <see langword="false"/>.</returns>
        protected virtual bool OnAddingLevel(byte curLevel) => false;

        private IEnumerator<float> AbilityCooldown()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;

                if (!CanEnterCooldown)
                    continue;

                IsReady = false;
                yield return Timing.WaitForSeconds(Cooldown);
                IsReady = true;
            }
        }
    }
}