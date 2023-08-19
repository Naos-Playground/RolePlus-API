// -----------------------------------------------------------------------
// <copyright file="ScreenLocation.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    /// <summary>
    /// Represents locations on a user's display.
    /// </summary>
    public enum ScreenLocation
    {
        /// <summary>
        /// Represents the top of the screen.
        /// </summary>
        Top,

        /// <summary>
        /// Represents a location between the <see cref="Middle"/> and the <see cref="Top"/>.
        /// </summary>
        MiddleTop,

        /// <summary>
        /// Represents the middle of the screen.
        /// </summary>
        Middle,

        /// <summary>
        /// Represents a location between the <see cref="Middle"/> and the <see cref="Bottom"/>.
        /// </summary>
        MiddleBottom,

        /// <summary>
        /// Represents the bottom of the screen.
        /// </summary>
        Bottom,
    }
}
