// -----------------------------------------------------------------------
// <copyright file="TPlexCollection.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Core.Generic.Collections
{
    /// <summary>
    /// An object which handles two generic types.
    /// </summary>
    /// <typeparam name="T1">The first generic type.</typeparam>
    /// <typeparam name="T2">The second generic type.</typeparam>
    /// <typeparam name="T3">The box type.</typeparam>
    public class TPlexCollection<T1, T2, T3> : TValueObject<T3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TPlexCollection{T1, T2, T3}"/> class.
        /// </summary>
        public TPlexCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TPlexCollection{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="baseVal">The base value.</param>
        public TPlexCollection(T1 key, T2 value, T3 baseVal)
            : base(baseVal)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public T1 Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T2 Value { get; set; }
    }
}
