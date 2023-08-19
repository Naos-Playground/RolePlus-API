// -----------------------------------------------------------------------
// <copyright file="EscapeBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomEscapes
{
    using System.Collections.Generic;
    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.Events.EventArgs;
    using UnityEngine;

    /// <summary>
    /// A class to easily manage escaping behavior.
    /// </summary>
    public abstract class EscapeBehaviour : EBehaviour, IAddittiveSettingsCollection<EscapeSettings>
    {
        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape settings.
        /// </summary>
        public virtual List<EscapeSettings> Settings { get; set; }

        /// <summary>
        /// Gets the current escape scenario.
        /// </summary>
        protected virtual EscapeScenarioTypeBase CurrentScenario => CalculateEscapeScenario();

        /// <inheritdoc/>
        public abstract void AdjustAddittiveProperty();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner.TryGetCustomRole(out CustomRole customRole) && !customRole.EscapeSettings.IsEmpty())
                Settings = customRole.EscapeSettings;

            AdjustAddittiveProperty();

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
        /// Calculates the escape scenario.
        /// <para>An <see langword="override"/> is required in order to make it work.</para>
        /// </summary>
        /// <returns>The corresponding <see cref="EscapeScenarioTypeBase"/>.</returns>
        protected abstract EscapeScenarioTypeBase CalculateEscapeScenario();

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
            EscapingEventArgs escaping = new(Owner, settings.Role, settings.CustomRole, CurrentScenario, CustomEscape.AllScenarios[CurrentScenario]);
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
