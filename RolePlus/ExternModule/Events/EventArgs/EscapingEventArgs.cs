// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.EventArgs
{
    using System;
    using Exiled.API.Features;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomHud;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using Hint = API.Features.CustomHud.Hint;

    /// <summary>
    /// Contains all informations before invoking an event.
    /// </summary>
    public sealed class EscapingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newRole"><inheritdoc cref="NewRole"/></param>
        /// <param name="newCustomRole"><inheritdoc cref="NewCustomRole"/></param>
        /// <param name="scenarioType"><inheritdoc cref="ScenarioType"/></param>
        /// <param name="hint"><inheritdoc cref="Hint"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EscapingEventArgs(
            Player player,
            RoleType newRole,
            CustomRole newCustomRole,
            EscapeScenarioTypeBase scenarioType,
            Hint hint,
            bool isAllowed = true)
        {
            Player = player;
            NewRole = newRole;
            NewCustomRole = newCustomRole;
            ScenarioType = scenarioType;
            Hint = hint;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the new <see cref="RoleType"/>.
        /// </summary>
        public RoleType NewRole { get; set; }

        /// <summary>
        /// Gets or sets the new <see cref="CustomRole"/>.
        /// </summary>
        public CustomRole NewCustomRole { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EscapeScenarioTypeBase"/>.
        /// </summary>
        public EscapeScenarioTypeBase ScenarioType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="API.Features.CustomHud.Hint"/> to be displayed.
        /// </summary>
        public Hint Hint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be invoked.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
