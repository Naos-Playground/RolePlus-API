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
    }
}
