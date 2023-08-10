// -----------------------------------------------------------------------
// <copyright file="ScreenLocation.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using RolePlus.ExternModule.API.Engine.Core;

    /// <summary>
    /// All available custom roles.
    /// </summary>
    public class CustomTeamTypeBase : UnmanagedEnumClass<uint, CustomTeamTypeBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeamTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        protected CustomTeamTypeBase(uint value) : base(value)
        {
        }

        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static CustomTeamTypeBase None { get; } = new(0);
    }
}
