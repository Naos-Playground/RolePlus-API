// -----------------------------------------------------------------------
// <copyright file="AbilityBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using MEC;
    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;

    /// <summary>
    /// CustomAbility is the base class used to create user-defined types treated as abilities applicable to a <see cref="Player"/>.
    /// </summary>
    public abstract class AbilityBehaviour : EBehaviour, IAddittiveSettings<AbilitySettings>
    {
#pragma warning disable SA1401 // Fields should be private
        private protected CoroutineHandle _abilityCooldownHandle;
        private protected byte _level;
        private protected bool _isActive;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Gets or sets the <see cref="AbilitySettings"/>.
        /// </summary>
        public AbilitySettings Settings { get; set; }

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

        /// <inheritdoc/>
        public abstract void AdjustAddittiveProperty();

        /// <summary>
        /// Activates the ability.
        /// </summary>
        /// <param name="isForced">A value indicating whether the activation should be forced.</param>
        /// <returns><see langword="true"/> if the ability was activated; otherwise, <see langword="false"/>.</returns>
        public virtual bool Activate(bool isForced = false)
        {
            if (!(IsReady = isForced || IsReady))
                return false;

            OnActivated();
            return true;
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            _abilityCooldownHandle = Timing.RunCoroutine(AbilityCooldown());
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Timing.KillCoroutines(_abilityCooldownHandle);
        }

        /// <inheritdoc/>
        protected override void BehaviourUpdate()
        {
            base.BehaviourUpdate();

            if (OnAddingLevel(Level))
                Level += 1;

            if (IsActive)
                OnUsing();
        }

        /// <summary>
        /// Fired before the ability is activated.
        /// </summary>
        protected virtual void OnActivating()
        {
            if (!IsReady)
            {
                Settings.OnCooldownHint.Show(Owner);
                return;
            }

            if (Settings.Duration == 0f)
            {
                IsReady = false;
                OnActivated();
                return;
            }

            OnActivated();
        }

        /// <summary>
        /// Fired after the ability is activated.
        /// </summary>
        protected virtual void OnActivated()
        {
            IsActive = true;

            Timing.CallDelayed(Settings.Duration, () =>
            {
                IsReady = false;
                IsActive = false;
                OnExpired();
            });
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
            Settings.NextLevelHint.Show(Owner, "%level", Level.ToString());
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

                if (IsReady)
                    continue;

                IsReady = false;
                yield return Timing.WaitForSeconds(Settings.Cooldown);
                IsReady = true;
            }
        }
    }
}