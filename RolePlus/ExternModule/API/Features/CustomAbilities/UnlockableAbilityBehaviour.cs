// -----------------------------------------------------------------------
// <copyright file="UnlockableAbilityBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using Exiled.API.Features;

    /// <summary>
    /// CustomAbility is the base class used to create user-defined types treated as abilities applicable to a <see cref="Player"/>.
    /// </summary>
    public abstract class UnlockableAbilityBehaviour : AbilityBehaviour
    {
        /// <summary>
        /// Gets or sets a value indicating whether the ability can be used regardless any conditions.
        /// </summary>
        protected virtual bool IsUnlocked { get; set; }

        /// <summary>
        /// Defines the conditions to be met in order to unlock the ability.
        /// </summary>
        /// <returns><see langword="true"/> if the ability can be unlocked; otherwise, <see langword="false"/>.</returns>
        protected abstract bool ProcessConditions();

        /// <inheritdoc/>
        public override bool Activate(bool isForced = false) => (IsUnlocked = isForced || IsUnlocked) && base.Activate(isForced);

        /// <inheritdoc/>
        protected override void BehaviourUpdate()
        {
            if (!IsUnlocked)
            {
                OnUnlocking(ProcessConditions());
                return;
            }

            base.BehaviourUpdate();
        }

        /// <summary>
        /// Fired before the ability is unlocked.
        /// </summary>
        /// <param name="isUnlockable">A value indicating whether the ability should be unlocked.</param>
        protected virtual void OnUnlocking(bool isUnlockable)
        {
            if (IsUnlocked = isUnlockable)
                OnUnlocked();
        }

        /// <summary>
        /// Fired after the ability is unlocked.
        /// </summary>
        protected virtual void OnUnlocked() => Owner.ShowHint(Settings.UnlockedHint);

        /// <summary>
        /// Fired before the ability is activated.
        /// </summary>
        protected override void OnActivating()
        {
            if (!IsUnlocked)
            {
                Owner.ShowHint(Settings.CannotBeUsed);
                return;
            }

            base.OnActivating();
        }
    }
}