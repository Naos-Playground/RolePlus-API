// -----------------------------------------------------------------------
// <copyright file="EscapeBuilder.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomEscapes
{
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.Controllers;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.Events.EventArgs;

    using System.Collections.Generic;

    using UnityEngine;

    using EscapeScenarioTypeBase = Enums.EscapeScenarioTypeBase;

    /// <summary>
    /// A class to easily manage escaping behavior.
    /// </summary>
    public abstract class EscapeBuilder : PlayerBehaviour
    {
        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape settings.
        /// </summary>
        public List<EscapeSettings> Settings { get; set; }

        /// <summary>
        /// Fired on <see cref="PostInitialize"/>.
        /// <para>Settings should be adjusted here.</para>
        /// </summary>
        protected virtual void AdjustSettings()
        {
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            AdjustSettings();

            FixedTickRate = 0.5f;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            foreach (EscapeSettings settings in Settings)
            {
                if (!settings.IsAllowed || Vector3.Distance(Owner.Position, settings.Position) > settings.MaxDistanceTolerance)
                    continue;

                ProcessEscapeAction(settings);
                CanEverTick = false;
                break;
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Events.Handlers.Player.Escaping += OnEscaping;
            Events.Handlers.Player.Escaped += OnEscaped;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Events.Handlers.Player.Escaping -= OnEscaping;
            Events.Handlers.Player.Escaped -= OnEscaped;
        }

        /// <summary>
        /// Gets the current escape scenario.
        /// </summary>
        protected virtual EscapeScenarioTypeBase EscapeScenario =>
            Owner.TryGetCustomRole(out CustomRole customRole) && customRole.UseCustomEscape ?
            CalculateEscapeScenario() : EscapeScenarioTypeBase.None;

        /// <summary>
        /// Calculates the escape scenario.
        /// <para>An <see langword="override"/> is required in order to make it work.</para>
        /// </summary>
        /// <returns>The corresponding <see cref="EscapeScenarioTypeBase"/>.</returns>
        protected virtual EscapeScenarioTypeBase CalculateEscapeScenario() => EscapeScenarioTypeBase.None;

        /// <summary>
        /// Fired before the player escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        protected virtual void OnEscaping(EscapingEventArgs ev)
        {
        }

        /// <summary>
        /// Fired after the player escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapedEventArgs"/> instance.</param>
        protected virtual void OnEscaped(EscapedEventArgs ev)
        {
        }

        private void ProcessEscapeAction(EscapeSettings settings)
        {
            EscapingEventArgs escaping = new(Owner, settings.Role, settings.CustomRole, EscapeScenario, CustomEscape.AllScenarios[EscapeScenario]);
            Events.Handlers.Player.OnEscaping(escaping);

            if (!escaping.IsAllowed)
                return;

            escaping.Player.SetRole(escaping.NewRole != RoleType.None ? escaping.NewRole : escaping.NewCustomRole);
            escaping.Hint.Show(escaping.Player);

            EscapedEventArgs escaped = new(escaping.Player);
            Events.Handlers.Player.OnEscaped(escaped);
            Destroy();
        }
    }
}
