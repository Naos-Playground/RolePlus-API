// -----------------------------------------------------------------------
// <copyright file="AbilitySettings.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;
    using RolePlus.ExternModule.API.Features.CustomHud;

    /// <summary>
    /// A tool to easily setup abilities.
    /// </summary>
    public class AbilitySettings : IAddittiveProperty
    {
        /// <summary>
        /// Gets or sets the required cooldown before using the ability again.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before the ability is activated.
        /// </summary>
        public float WindupTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is used.
        /// </summary>
        public Hint ActivatedHint { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation is denied regardless any conditions.
        /// </summary>
        public Hint CannotBeUsed { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation in on cooldown.
        /// </summary>
        public Hint OnCooldownHint { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is expired.
        /// </summary>
        public Hint ExpiredHint { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is unlocked.
        /// </summary>
        public Hint UnlockedHint { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reaches a new level.
        /// </summary>
        public Hint NextLevelHint { get; set; }
    }
}