// -----------------------------------------------------------------------
// <copyright file="CustomAbilityAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System;

    using RolePlus.ExternModule.API.Features.CustomAbilities;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomAbility"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAbilityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAbilityAttribute"/> class.
        /// </summary>
        public CustomAbilityAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAbilityAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public CustomAbilityAttribute(uint id) => Id = id;

        /// <summary>
        /// Gets the custom ability's id.
        /// </summary>
        internal uint Id { get; }
    }
}