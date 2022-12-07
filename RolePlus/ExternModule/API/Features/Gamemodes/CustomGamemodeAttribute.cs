// -----------------------------------------------------------------------
// <copyright file="CustomGamemodeAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomGamemode"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomGamemodeAttribute : Attribute
    {
    }
}
