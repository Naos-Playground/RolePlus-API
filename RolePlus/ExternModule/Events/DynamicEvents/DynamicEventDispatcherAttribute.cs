// -----------------------------------------------------------------------
// <copyright file="DynamicEventDispatcherAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.DynamicEvents
{
    using System;

    /// <summary>
    /// An attribute to easily manage <see cref="DynamicEventDispatcher"/> initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DynamicEventDispatcherAttribute : Attribute
    {
    }
}