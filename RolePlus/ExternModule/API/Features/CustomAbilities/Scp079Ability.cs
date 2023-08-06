// -----------------------------------------------------------------------
// <copyright file="Scp079Ability.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Scp079;

    /// <summary>
    /// The <see cref="Scp079Ability"/> base class.
    /// </summary>
    public abstract class Scp079Ability : CustomAbility
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Ability"/> class.
        /// </summary>
        /// <param name="cooldown"><inheritdoc cref="CustomAbility.Cooldown"/></param>
        protected Scp079Ability(float cooldown = 0f)
            : base(cooldown)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Ability"/> class.
        /// </summary>
        /// <param name="tier"><inheritdoc cref="Tier"/></param>
        /// <param name="gainedExp"><inheritdoc cref="GainedExperience"/></param>
        /// <param name="requiredEnergy"><inheritdoc cref="RequiredEnergy"/></param>
        /// <param name="cooldown"><inheritdoc cref="CustomAbility.Cooldown"/></param>
        protected Scp079Ability(
            byte tier,
            int gainedExp,
            float requiredEnergy,
            float cooldown = 0f)
            : this(cooldown)
        {
            Tier = tier;
            GainedExperience = gainedExp;
            RequiredEnergy = requiredEnergy;
            Cooldown = cooldown;
        }

        /// <summary>
        /// All available responses used by <see cref="Invoke(object[])"/>.
        /// </summary>
        public enum InvokeResult
        {
            /// <summary>
            /// Invoked.
            /// </summary>
            Valid,

            /// <summary>
            /// Not enough energy.
            /// </summary>
            NotEnoughEnergy,

            /// <summary>
            /// Locked.
            /// </summary>
            Locked,

            /// <summary>
            /// On cooldown.
            /// </summary>
            OnCooldown,

            /// <summary>
            /// Invalid data.
            /// </summary>
            InvalidData,
        }

        /// <summary>
        /// Gets or sets the required tier to unlock the ability.
        /// </summary>
        public abstract byte Tier { get; protected set; }

        /// <summary>
        /// Gets or sets the gained experience after using the ability.
        /// </summary>
        public abstract int GainedExperience { get; protected set; }

        /// <summary>
        /// Gets or sets the required energy to use the ability.
        /// </summary>
        public abstract float RequiredEnergy { get; protected set; }

        /// <summary>
        /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="ZoneType"/> containing all the zones in which SCP-079 can use the ability.
        /// </summary>
        public virtual IEnumerable<ZoneType> AllowedZones { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the ability is unlockable.
        /// </summary>
        public bool IsUnlockable => Owner.Role.As<Scp079Role>().Level >= Tier;

        /// <summary>
        /// Gets or sets a value indicating whether the ability is unlocked.
        /// </summary>
        public bool IsUnlocked { get; set; }

        /// <inheritdoc/>
        protected override bool CanEnterCooldown => !IsUnlocked || base.CanEnterCooldown;

        /// <summary>
        /// Gets a value indicating whether the ability is unlocked.
        /// </summary>
        /// <param name="scp079">The player who owns the ability..</param>
        /// <param name="ability">The ability to check.</param>
        /// <returns><see langword="true"/> if the ability is unlocked; otherwise, <see langword="false"/>.</returns>
        public static bool IsAbilityUnlocked(Scp079Role scp079, Scp079Ability ability) => scp079.Level >= ability.Tier;

        /// <summary>
        /// Invokes the ability.
        /// </summary>
        /// <param name="args">The arguments to invoke the ability.</param>
        /// <returns>The corresponding <see cref="InvokeResult"/>.</returns>
        public virtual InvokeResult Invoke(params object[] args)
        {
            OnAbilityUsed();
            return InvokeResult.Valid;
        }

        /// <inheritdoc/>
        protected override void OnAbilityUsed()
        {
            base.OnAbilityUsed();

            Owner.Role.As<Scp079Role>().Energy -= RequiredEnergy;
            Owner.Role.As<Scp079Role>().Experience += GainedExperience;
        }

        /// <summary>
        /// Checks if the ability is invokable.
        /// </summary>
        /// <param name="invokeResponse">The response.</param>
        /// <returns><see langword="true"/> if the ability is invokable; otherwise, <see langword="false"/>.</returns>
        public bool IsInvokable(out InvokeResult invokeResponse)
        {
            if (!IsUnlocked)
            {
                invokeResponse = InvokeResult.Locked;
                return false;
            }
            else if (Owner.Role.As<Scp079Role>().Energy < RequiredEnergy)
            {
                invokeResponse = InvokeResult.NotEnoughEnergy;
                return false;
            }
            else if (!IsReady)
            {
                invokeResponse = InvokeResult.OnCooldown;
                return false;
            }
            else
            {
                invokeResponse = InvokeResult.Valid;
                return true;
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Scp079.GainingLevel += OnGainingLevel;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Scp079.GainingLevel -= OnGainingLevel;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp079.OnGainingLevel(GainingLevelEventArgs)"/>
        protected virtual void OnGainingLevel(GainingLevelEventArgs ev)
        {
            if (!Check(ev.Player) || ev.NewLevel < Tier)
                return;

            IsUnlocked = true;
            IsReady = true;
            Owner.ShowHint(UnlockedMessage, 5);
        }
    }
}