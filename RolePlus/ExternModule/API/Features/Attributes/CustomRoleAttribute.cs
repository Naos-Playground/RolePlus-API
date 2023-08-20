// -----------------------------------------------------------------------
// <copyright file="CustomRoleAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Attributes
{
    using System;

    using RolePlus.ExternModule.API.Features.CustomRoles;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomRole"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomRoleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoleAttribute"/> class.
        /// </summary>
        public CustomRoleAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoleAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public CustomRoleAttribute(uint id) => Id = id;

        /// <summary>
        /// Gets the custom role's id.
        /// </summary>
        internal uint Id { get; }
    }
}
