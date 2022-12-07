// -----------------------------------------------------------------------
// <copyright file="TMultiPlexCollection.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Core.Generic.Collections
{
    /// <summary>
    /// An object which handles three generic types.
    /// </summary>
    /// <typeparam name="T1">The first generic type.</typeparam>
    /// <typeparam name="T2">The second generic type.</typeparam>
    /// <typeparam name="T3">The third generic type.</typeparam>
    /// <typeparam name="T4">The box type.</typeparam>
    public class TMultiPlexCollection<T1, T2, T3, T4> : TPlexCollection<T1, T2, T4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiPlexCollection{T1, T2, T3, T4}"/> class.
        /// </summary>
        public TMultiPlexCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiPlexCollection{T1, T2, T3, T4}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="baseVal">The base value.</param>
        public TMultiPlexCollection(T1 key, T2 value, T4 baseVal)
            : base(key, value, baseVal)
        {
        }

        /// <summary>
        /// Gets or sets the plex.
        /// </summary>
        public T3 Plex { get; set; }
    }
}
