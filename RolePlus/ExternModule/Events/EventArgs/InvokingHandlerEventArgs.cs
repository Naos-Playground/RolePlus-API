// -----------------------------------------------------------------------
// <copyright file="InvokingHandlerEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.EventArgs
{
    using System;

    using static Exiled.Events.Events;

    /// <summary>
    /// Contains all informations before invoking an event.
    /// </summary>
    public sealed class InvokingHandlerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvokingHandlerEventArgs"/> class.
        /// </summary>
        /// <param name="ev">The event being invoked.</param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InvokingHandlerEventArgs(object ev, bool isAllowed = true)
        {
            if (ev is null)
                return;

            if (ev is CustomEventHandler<EventArgs> generic)
                GenericHandler = generic;

            if (ev is CustomEventHandler target)
                TargetHandler = target;

            Name = ev.GetType().Name;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the generic handler.
        /// </summary>
        public CustomEventHandler<EventArgs> GenericHandler { get; }

        /// <summary>
        /// Gets the target handler.
        /// </summary>
        public CustomEventHandler TargetHandler { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be invoked.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
