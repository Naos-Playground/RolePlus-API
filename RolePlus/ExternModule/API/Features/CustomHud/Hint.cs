// -----------------------------------------------------------------------
// <copyright file="Hint.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomHud
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    using Hints;

    using RolePlus.ExternModule.API.Enums;

    /// <summary>
    /// Represents a hint to be displayed to a player.
    /// </summary>
    public readonly struct Hint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> struct.
        /// </summary>
        /// <param name="content"><inheritdoc cref="Content"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        /// <param name="effects"><inheritdoc cref="Effects"/></param>
        /// <param name="parameters"><inheritdoc cref="Parameters"/></param>
        public Hint(string content, float duration, HintEffect[] effects = null, HintParameter[] parameters = null)
        {
            Content = content;
            Duration = duration;
            Effects = effects;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets the content of the hint.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the duration of the hint.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Gets the hint's effects.
        /// </summary>
        public HintEffect[] Effects { get; }

        /// <summary>
        /// Gets the hint's parameters.
        /// </summary>
        public HintParameter[] Parameters { get; }

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
        public static void Show(Player player,
            string content,
            float duration = 3f,
            bool overrideCurrent = true,
            DisplayLocation displayLocation = DisplayLocation.MiddleBottom,
            HintEffect[] effects = null,
            HintParameter[] parameters = null)
        {
            Hint hint = new(content, duration, effects, parameters);
            hint.Show(player, displayLocation, overrideCurrent);
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="player">The player to show the hint to.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(Player player, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (string.IsNullOrEmpty(Content))
                return;

            if (player.TryGetComponent(out HudBehaviour hud))
            {
                hud.Displays[displayLocation].Enqueue(Content, Duration, overrideQueue, Effects, Parameters);
                return;
            }

            player.HintDisplay.Show(new TextHint(Content, Parameters is null ? new HintParameter[1] { new StringHintParameter(Content) } : Parameters, Effects, Duration));
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="player">The player to show the hint to.</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(Player player, string replace, string newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (player.TryGetComponent(out HudBehaviour hud))
            {
                hud.Displays[displayLocation].Enqueue(Content.Replace(replace, newValue), Duration, overrideQueue, Effects, Parameters);
                return;
            }

            player.HintDisplay.Show(
                new TextHint(Content.Replace(replace, newValue), Parameters is null ? new HintParameter[1] { new StringHintParameter(Content) }
                : Parameters, Effects, Duration));
        }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="player">The player to show the hint to.</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(Player player, string[] replace, string[] newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (replace.Length < newValue.Length)
                throw new System.IndexOutOfRangeException("The values to be replaced are less than the new values.");

            string content = Content;
            for (int i = 0; i < replace.Length; i++)
                content = content.Replace(replace[i], newValue[i]);

            if (player.TryGetComponent(out HudBehaviour hud))
            {
                hud.Displays[displayLocation].Enqueue(content, Duration, overrideQueue, Effects, Parameters);
                return;
            }

            player.HintDisplay.Show(
                new TextHint(content, Parameters is null ? new HintParameter[1] { new StringHintParameter(Content) }
                : Parameters, Effects, Duration));
        }

        /// <summary>
        /// Shows a hint to the specified players.
        /// </summary>
        /// <param name="players">The players to show the hint to.</param>
        public void Show(IEnumerable<Player> players)
        {
            foreach (Player player in players)
                Show(player);
        }

        /// <summary>
        /// Shows a hint to the specified players.
        /// </summary>
        /// <param name="players">The players to show the hint to.</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(IEnumerable<Player> players, string replace, string newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            foreach (Player player in players)
                Show(player, replace, newValue, displayLocation, overrideQueue);
        }

        /// <summary>
        /// Shows a hint to the specified players.
        /// </summary>
        /// <param name="players">The players to show the hint to.</param>
        /// <param name="replace">The <see cref="string"/> to replace..</param>
        /// <param name="newValue">The <see cref="string"/> replacement.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        public void Show(IEnumerable<Player> players, string[] replace, string[] newValue, DisplayLocation displayLocation = DisplayLocation.MiddleBottom, bool overrideQueue = false)
        {
            if (replace.Length < newValue.Length)
                throw new System.IndexOutOfRangeException("The values to be replaced are less than the new values.");

            foreach (Player player in players)
            {
                string content = Content;
                for (int i = 0; i < replace.Length; i++)
                    content = content.Replace(replace[i], newValue[i]);

                if (player.TryGetComponent(out HudBehaviour hud))
                {
                    hud.Displays[displayLocation].Enqueue(content, Duration, overrideQueue, Effects, Parameters);
                    return;
                }

                player.HintDisplay.Show(
                    new TextHint(content, Parameters is null ? new HintParameter[1] { new StringHintParameter(Content) }
                    : Parameters, Effects, Duration));
            }
        }
    }
}
