// -----------------------------------------------------------------------
// <copyright file="Hint.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Hints;
using System.Collections;
using System.Collections.Generic;

namespace RolePlus.ExternModule.API.Features.CustomHud
{
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
        /// Gets or sets the content of the hint.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets or sets the duration of the hint.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Gets or sets the hint's effects.
        /// </summary>
        public HintEffect[] Effects { get; }

        /// <summary>
        /// Gets or sets the hint's parameters.
        /// </summary>
        public HintParameter[] Parameters { get; }

        /// <summary>
        /// Shows a hint to the specified player.
        /// </summary>
        /// <param name="player">The player to show the hint to.</param>
        public void Show(Player player)
        {
            player.HintDisplay.Show(new TextHint(Content, Parameters is null ? new HintParameter[1]
            {
                new StringHintParameter(Content)
            } : Parameters, Effects, Duration));
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
    }
}
