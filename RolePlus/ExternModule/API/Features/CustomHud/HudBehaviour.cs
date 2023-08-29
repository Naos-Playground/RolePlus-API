// -----------------------------------------------------------------------
// <copyright file="HudBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomHud
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using Hints;

    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Enums;

    /// <summary>
    /// Defines the hints behaviour on a player's display.
    /// </summary>
    public sealed class HudBehaviour : EBehaviour
    {
        private const string HUD_FORMAT = "<line-height=95%><voffset=8.5em><alpha=#ff>\n\n\n<align=center>{0}{1}{2}{3}{4}</align>";

        private object[] _toFormat;
        private string _hint;

        /// <summary>
        /// Gets all attached <see cref="Display"/> instances.
        /// </summary>
        public SortedList<DisplayLocation, Display> Displays { get; } = new()
        {
            [DisplayLocation.Top] = new(),
            [DisplayLocation.MiddleTop] = new(),
            [DisplayLocation.Middle] = new(),
            [DisplayLocation.MiddleBottom] = new(),
            [DisplayLocation.Bottom] = new(),
        };

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            FixedTickRate = 1f;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            UpdateHints();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            foreach (Display display in Displays.Values)
                display.Destroy();
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="player">The player to show the hint to.</param>
        /// <param name="content">The message to display.</param>
        /// <param name="duration">The duration of the hint.</param>
        /// <param name="overrideCurrent">A value indicating whether the new hint should override the active hint.</param>
        /// <param name="displayLocation">The location to display the hint.</param>
        /// <param name="effects">The hint effects to be applied to.</param>
        /// <param name="parameters">The hint parameters.</param>
        public static void Show(
            Player player,
            string content,
            float duration = 3f,
            bool overrideCurrent = true,
            DisplayLocation displayLocation = DisplayLocation.MiddleBottom,
            HintEffect[] effects = null,
            HintParameter[] parameters = null)
        {
            if (!player.TryGetComponent(out HudBehaviour hud))
                return;

            hud.Show(new(content, duration, effects, parameters), displayLocation, overrideCurrent);
        }

        /// <summary>
        /// Shows a hint to the specified players.
        /// </summary>
        /// <param name="hint">The hint to display..</param>
        /// <param name="players">The players to show the hint to.</param>
        public static void Show(Hint hint, IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.TryGetComponent(out HudBehaviour hud))
                    hud.Show(hint);
            }
        }

        /// <summary>
        /// Shows a hint to the specified players.
        /// </summary>
        /// <param name="hint">The hint to display..</param>
        /// <param name="players">The players to show the hint to.</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public static void Show(Hint hint, IEnumerable<Player> players, string replace, string newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            foreach (Player player in players)
            {
                if (player.TryGetComponent(out HudBehaviour hud))
                    hud.Show(hint, replace, newValue, displayLocation, overrideQueue);
            }
        }

        /// <summary>
        /// Shows a hint to the specified players.
        /// </summary>
        /// <param name="hint">The hint to display..</param>
        /// <param name="players">The players to show the hint to.</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public static void Show(Hint hint, IEnumerable<Player> players, string[] replace, string[] newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (replace.Length < newValue.Length)
                throw new System.IndexOutOfRangeException("The values to be replaced are less than the new values.");

            foreach (Player player in players)
            {
                string content = hint.Content;
                for (int i = 0; i < replace.Length; i++)
                    content = content.Replace(replace[i], newValue[i]);

                if (player.TryGetComponent(out HudBehaviour hud))
                {
                    hud.Displays[displayLocation].Enqueue(content, hint.Duration, overrideQueue, hint.Effects, hint.Parameters);
                    return;
                }

                player.HintDisplay.Show(
                    new TextHint(content, hint.Parameters is null ? new HintParameter[1] { new StringHintParameter(hint.Content) }
                    : hint.Parameters, hint.Effects, hint.Duration));
            }
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="hint">The hint to display..</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(Hint hint, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (string.IsNullOrEmpty(hint.Content))
                return;

            if (Owner.TryGetComponent(out HudBehaviour hud))
            {
                hud.Displays[displayLocation].Enqueue(hint.Content, hint.Duration, overrideQueue, hint.Effects, hint.Parameters);
                return;
            }

            Owner.HintDisplay.Show(new TextHint(hint.Content, hint.Parameters is null ?
                new HintParameter[1] { new StringHintParameter(hint.Content) } : hint.Parameters, hint.Effects, hint.Duration));
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="hint">The hint to display..</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(Hint hint, string replace, string newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (Owner.TryGetComponent(out HudBehaviour hud))
            {
                hud.Displays[displayLocation].Enqueue(hint.Content.Replace(replace, newValue), hint.Duration, overrideQueue, hint.Effects, hint.Parameters);
                return;
            }

            Owner.HintDisplay.Show(
                new TextHint(hint.Content.Replace(replace, newValue), hint.Parameters is null ?
                new HintParameter[1] { new StringHintParameter(hint.Content) } : hint.Parameters, hint.Effects, hint.Duration));
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="hint">The hint to display..</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(Hint hint, string[] replace, string[] newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (replace.Length < newValue.Length)
                throw new System.IndexOutOfRangeException("The values to be replaced are less than the new values.");

            string content = hint.Content;
            for (int i = 0; i < replace.Length; i++)
                content = content.Replace(replace[i], newValue[i]);

            if (Owner.TryGetComponent(out HudBehaviour hud))
            {
                hud.Displays[displayLocation].Enqueue(content, hint.Duration, overrideQueue, hint.Effects, hint.Parameters);
                return;
            }

            Owner.HintDisplay.Show(
                new TextHint(content, hint.Parameters is null ? new HintParameter[1] { new StringHintParameter(hint.Content) }
                : hint.Parameters, hint.Effects, hint.Duration));
        }

        private void UpdateHints()
        {
            _toFormat = Displays.Values.Select(display => FormatStringForHud(display.Content ?? string.Empty, 6)).ToArray<object>();
            _hint = string.Format(HUD_FORMAT, _toFormat);
            Owner.HintDisplay.Show(new TextHint(_hint, new HintParameter[] { new StringHintParameter(_hint) }, durationScalar: 2));
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
