// -----------------------------------------------------------------------
// <copyright file="RuleAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="TRule{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RuleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleAttribute"/> class.
        /// </summary>
        /// <param name="group"><inheritdoc cref="Group"/></param>
        public RuleAttribute(string group) => Group = group;

        /// <summary>
        /// Gets the rule's group.
        /// </summary>
        public string Group { get; }
    }
}
