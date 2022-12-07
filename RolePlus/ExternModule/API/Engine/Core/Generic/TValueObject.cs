// -----------------------------------------------------------------------
// <copyright file="TValueObject.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Core.Generic
{
    using System;

    /// <summary>
    /// An object which handles a value.
    /// </summary>
    /// <typeparam name="TObject">The type of the value.</typeparam>
    public abstract class TValueObject<TObject>
    {
        private readonly Type _boxed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TValueObject{TObject}"/> class.
        /// </summary>
        protected TValueObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TValueObject{TObject}"/> class.
        /// </summary>
        /// <param name="value">The value to store.</param>
        protected TValueObject(TObject value) => Base = value;

        /// <summary>
        /// Initializes a new instance of the <see cref="TValueObject{TObject}"/> class.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="type">The <see cref="Type"/> to box.</param>
        protected TValueObject(TObject value, Type type)
            : this(value) => _boxed = type;

        /// <summary>
        /// Gets or sets the stored value.
        /// </summary>
        public virtual TObject Base { get; set; }

        /// <summary>
        /// Unsafely casts <see cref="Base"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">The type cast.</typeparam>
        /// <returns>The casted <see cref="Base"/>.</returns>
        public T Cast<T>()
            where T : class => Base as T;

        /// <summary>
        /// Unsafely casts <see cref="Base"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">The type cast.</typeparam>
        /// <param name="_">The type param.</param>
        /// <returns>The casted <see cref="Base"/>.</returns>
        public T Cast<T>(T _)
            where T : class => Cast<T>();

        /// <summary>
        /// Safely casts <see cref="Base"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">The type cast.</typeparam>
        /// <param name="param">The type param.</param>
        /// <returns><see langword="true"/> if <see cref="Base"/> can be casted to the specified type; otherwise, <see langword="false"/>.</returns>
        public bool Cast<T>(out T param)
             where T : class
        {
            param = default;

            if (param.GetType() != _boxed)
                return false;

            param = Cast<T>();

            return true;
        }
    }
}
