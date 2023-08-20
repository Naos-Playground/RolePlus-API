// -----------------------------------------------------------------------
// <copyright file="StateAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.States
{
    using System;

    /// <summary>
    /// An attribute to easily initialize states.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StateAttribute : Attribute
    {
    }
}
