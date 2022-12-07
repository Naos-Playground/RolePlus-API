// -----------------------------------------------------------------------
// <copyright file="HudManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomHud
{
    using System.Collections.Generic;
    using System.Linq;

    using Hints;

    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.Controllers;

    using UnityEngine;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Manages hints on a player's display.
    /// </summary>
    public sealed class HudManager : PlayerScriptController
    {
        private const string HudTemplate = "<line-height=95%><voffset=8.5em><alpha=#ff>\n\n\n<align=center>{0}{1}{2}{3}{4}</align>";

        private object[] _toFormat;
        private float _globalTimer;
        private string _hint;

        /// <summary>
        /// Gets all instance of the <see cref="HudManager"/> component.
        /// </summary>
        public static Dictionary<Player, HudManager> Instances { get; } = new();

        /// <summary>
        /// Gets all attached <see cref="HudScreen"/> instances.
        /// </summary>
        public SortedList<ScreenLocation, HudScreen> Displays { get; } = new()
        {
            [ScreenLocation.Top] = new(),
            [ScreenLocation.MiddleTop] = new(),
            [ScreenLocation.Middle] = new(),
            [ScreenLocation.MiddleBottom] = new(),
            [ScreenLocation.Bottom] = new(),
        };

        /// <inheritdoc/>
        public override Player Owner { get; protected set; }

        /// <summary>
        /// Displays a hint to a player.
        /// </summary>
        /// <param name="player">The player to show the hint to.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">The duration of the hint.</param>
        /// <param name="overrideCurrent">Overrides the active hint.</param>
        /// <param name="displayLocation">The location to display the hint.</param>
        public static void ShowHint(Player player, string message, float duration = 3f, bool overrideCurrent = true, ScreenLocation displayLocation = ScreenLocation.MiddleBottom)
        {
            if (Instances.TryGetValue(player, out HudManager hudManager))
                hudManager.Displays[displayLocation].Enqueue(message, duration, overrideCurrent);
        }

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            Instances.Add(Owner, this);
        }

        /// <inheritdoc/>
        protected override void FixedUpdate()
        {
            _globalTimer += Time.deltaTime;
            if (_globalTimer > 1f)
            {
                UpdateHints();
                _globalTimer = 0;
            }
        }

        /// <inheritdoc/>
        protected override void PartiallyDestroy()
        {
            foreach (HudScreen display in Displays.Values)
                display.Kill();

            base.PartiallyDestroy();

            Instances.Remove(Owner);
        }

        private void UpdateHints()
        {
            _toFormat = Displays.Values.Select(display => FormatStringForHud(display.Content ?? string.Empty, 6)).ToArray<object>();
            _hint = string.Format(HudTemplate, _toFormat);
            HintParameter[] parameters =
            {
                new StringHintParameter(_hint),
            };

            Owner.HintDisplay.Show(new TextHint(_hint, parameters, durationScalar: 2));
        }

        private string FormatStringForHud(string text, int needNewLine)
        {
            int curNewLine = text.Count(x => x == '\n');
            for (int i = 0; i < needNewLine - curNewLine; i++)
                text += '\n';

            return text;
        }
    }
}
