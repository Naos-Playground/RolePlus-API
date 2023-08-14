﻿// -----------------------------------------------------------------------
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
    }
}