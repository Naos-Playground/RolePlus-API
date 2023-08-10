// -----------------------------------------------------------------------
// <copyright file="CustomEscapeAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Attributes
{
    using System;

    using RolePlus.ExternModule.API.Features.CustomEscapes;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomEscape"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEscapeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEscapeAttribute"/> class.
        /// </summary>
        public CustomEscapeAttribute()
        {
        }
    }
}
