// -----------------------------------------------------------------------
// <copyright file="CustomRoleTypeBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using RolePlus.ExternModule.API.Engine.Framework;

    /// <summary>
    /// All available custom roles.
    /// </summary>
    public class CustomRoleTypeBase : UnmanagedEnumClass<uint, CustomRoleTypeBase>
    {
        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static readonly CustomRoleTypeBase None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoleTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        protected CustomRoleTypeBase(uint value) : base(value)
        {
        }
    }
}
