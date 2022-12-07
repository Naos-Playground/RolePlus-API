// -----------------------------------------------------------------------
// <copyright file="TMultiDirectionalCollection.cs" company="NaoUnderscore">
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
    /// <typeparam name="T3">The box type.</typeparam>
    public class TMultiDirectionalCollection<T1, T2, T3> : TPlainCollection<T1, T1, T3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiDirectionalCollection{T1, T2, T3}"/> class.
        /// </summary>
        public TMultiDirectionalCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiDirectionalCollection{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The destination.</param>
        /// <param name="value">The value to box.</param>
        public TMultiDirectionalCollection(T2 src, T2 dst, T3 value)
            : this(dst, value) => CustomSource = src;

        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiDirectionalCollection{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The destination.</param>
        /// <param name="value">The value to box.</param>
        public TMultiDirectionalCollection(T1 src, T2 dst, T3 value)
            : this(dst, value) => Source = src;

        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiDirectionalCollection{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The destination.</param>
        /// <param name="value">The value to box.</param>
        public TMultiDirectionalCollection(T2 src, T1 dst, T3 value)
            : base(dst, value) => CustomSource = src;

        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiDirectionalCollection{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="value">The value to box.</param>
        protected TMultiDirectionalCollection(T3 value) => Base = value;

        /// <summary>
        /// Initializes a new instance of the <see cref="TMultiDirectionalCollection{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="dst">The destination.</param>
        /// <param name="value">The value to box.</param>
        protected TMultiDirectionalCollection(T2 dst, T3 value)
            : this(value) => CustomDestination = dst;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public T2 CustomSource { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public T2 CustomDestination { get; set; }
    }
}
