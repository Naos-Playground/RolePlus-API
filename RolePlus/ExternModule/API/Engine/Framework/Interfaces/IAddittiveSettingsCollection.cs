// -----------------------------------------------------------------------
// <copyright file="IAddittiveSettingsCollection.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a <see cref="EBehaviour"/> which is being set up through user-defined settings.
    /// </summary>
    /// <typeparam name="T">The <see cref="EBehaviour"/> type.</typeparam>
    public interface IAddittiveSettingsCollection<T> : IAddittivePipe
        where T : IAddittiveProperty
    {
        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> settings.
        /// </summary>
        public List<T> Settings { get; set; }
    }
}
