// -----------------------------------------------------------------------
// <copyright file="Scp079AbilityBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features.Roles;

    /// <summary>
    /// The <see cref="Scp079AbilityBehaviour"/> base class.
    /// </summary>
    public abstract class Scp079AbilityBehaviour : UnlockableAbilityBehaviour
    {
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

        /// <inheritdoc/>
        protected override void OnActivating()
        {
            if (!AllowedZones.Contains(Owner.Zone) || Owner.Role.As<Scp079Role>().Energy < RequiredEnergy)
                return;

            base.OnActivating();
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            Owner.Role.As<Scp079Role>().Energy -= RequiredEnergy;
            Owner.Role.As<Scp079Role>().AddExperience(GainedExperience, PlayerRoles.PlayableScps.Scp079.Scp079HudTranslation.Experience);
        }

        /// <inheritdoc/>
        protected override bool ProcessConditions() => Owner.Role.As<Scp079Role>().Level >= Tier;
    }
}