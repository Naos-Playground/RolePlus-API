// -----------------------------------------------------------------------
// <copyright file="CustomAbilityAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomAbility"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAbilityAttribute : Attribute
    {
    }
}