// -----------------------------------------------------------------------
// <copyright file="DynamicDelegate.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.DynamicEvents
{
    using System;
    using Exiled.API.Features.Core;

    /// <summary>
    /// The <see cref="DynamicDelegate"/> allows user-defined delegate routes bound to an <see cref="object"/> reference.
    /// </summary>
    public class DynamicDelegate : TypeCastObject<DynamicEventDispatcher>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDelegate"/> class.
        /// </summary>
        public DynamicDelegate()
        {
        }

        /// <summary>
        /// Gets the <see cref="DynamicDelegate"/>'s target.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Gets the <see cref="DynamicDelegate"/>'s delegate.
        /// </summary>
        public Action Delegate { get; }
    }
}