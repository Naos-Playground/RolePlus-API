// -----------------------------------------------------------------------
// <copyright file="CustomEscapeTypeBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using RolePlus.ExternModule.API.Engine.Framework;

    /// <summary>
    /// All available custom escapes.
    /// </summary>
    public class CustomEscapeTypeBase : UnmanagedEnumClass<uint, CustomRoleTypeBase>
    {
        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static readonly CustomEscapeTypeBase None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEscapeTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        protected CustomEscapeTypeBase(uint value) : base(value)
        {
        }
    }
}
