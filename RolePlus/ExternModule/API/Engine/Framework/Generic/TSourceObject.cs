// -----------------------------------------------------------------------
// <copyright file="TSourceObject.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Generic
{
    /// <summary>
    /// An object which handles a generic type.
    /// </summary>
    /// <typeparam name="T1">The generic type.</typeparam>
    /// <typeparam name="T2">The box type.</typeparam>
    public class TSourceObject<T1, T2> : TValueObject<T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TSourceObject{T1, T2}"/> class.
        /// </summary>
        public TSourceObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TSourceObject{T1, T2}"/> class.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="src">The generic type param.</param>
        public TSourceObject(T1 src, T2 value)
            : base(value) => Source = src;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public T1 Source { get; set; }
    }
}
