// -----------------------------------------------------------------------
// <copyright file="IAddittiveBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Interfaces
{
    using System;

    /// <summary>
    /// Defines a <see cref="EBehaviour"/> which is being set up through user-defined type component.
    /// </summary>
    public interface IAddittiveBehaviour : IAddittiveIdentifier
    {
        /// <summary>
        /// Gets the behaviour component.
        /// </summary>
        public Type BehaviourComponent { get; }
    }
}
