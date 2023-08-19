// -----------------------------------------------------------------------
// <copyright file="CustomAbilityTypeBase.cs" company="NaoUnderscore">
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
    public class CustomAbilityTypeBase : UnmanagedEnumClass<uint, CustomRoleTypeBase>
    {
        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static readonly CustomAbilityTypeBase None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAbilityTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        protected CustomAbilityTypeBase(uint value) : base(value)
        {
        }
    }
}
