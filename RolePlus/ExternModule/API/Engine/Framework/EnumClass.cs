// -----------------------------------------------------------------------
// <copyright file="EnumClass.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A class which allows <see cref="Enum"/> implicit conversions.
    /// <para>Can be used along with <see cref="Enum"/>, means it doesn't require another <see cref="EnumClass{TSource, TObject}"/> instance to be comparable or usable.</para>
    /// </summary>
    /// <typeparam name="TSource">The type of the source object to handle the instance of.</typeparam>
    /// <typeparam name="TObject">The type of the child object to handle the instance of.</typeparam>
    public abstract class EnumClass<TSource, TObject> : IComparable, IEquatable<TObject>, IComparable<TObject>, IComparer<TObject>
        where TSource : Enum
        where TObject : EnumClass<TSource, TObject>
    {
        private static SortedList<TSource, TObject> _values;
        private static bool _isDefined;

        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumClass{TSource, TObject}"/> class.
        /// </summary>
        /// <param name="value">The value of the enum item.</param>
        protected EnumClass(TSource value)
        {
            _values ??= new();

            Value = value;
            _values.Add(value, (TObject)this);
        }

        /// <summary>
        /// Gets all <typeparamref name="TObject"/> object instances.
        /// </summary>
        public static IEnumerable<TObject> Values => _values.Values;

        /// <summary>
        /// Gets the value of the enum item.
        /// </summary>
        public TSource Value { get; }

        /// <summary>
        /// Gets the name determined from reflection.
        /// </summary>
        public string Name
        {
            get
            {
                if (_isDefined)
                    return _name;

                IEnumerable<FieldInfo> fields = typeof(TObject)
                    .GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public)
                    .Where(t => t.FieldType == typeof(TObject));

                foreach (FieldInfo field in fields)
                {
                    TObject instance = (TObject)field.GetValue(null);
                    instance._name = field.Name;
                }

                _isDefined = true;
                return _name;
            }
        }

        /// <summary>
        /// Implicitly converts the <see cref="EnumClass{TSource, TObject}"/> to <typeparamref name="TSource"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator TSource(EnumClass<TSource, TObject> value) => value.Value;

        /// <summary>
        /// Implicitly converts the <typeparamref name="TSource"/> to <see cref="EnumClass{TSource, TObject}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator EnumClass<TSource, TObject>(TSource value) => _values[value];

        /// <summary>
        /// Implicitly converts the <see cref="EnumClass{TSource, TObject}"/> to <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator TObject(EnumClass<TSource, TObject> value) => value;

        /// <summary>
        /// Casts the specified <paramref name="value"/> to the corresponding type.
        /// </summary>
        /// <param name="value">The enum value to be cast.</param>
        /// <returns>The cast object.</returns>
        public static TObject Cast(TSource value) => _values[value];

        /// <summary>
        /// Safely casts the specified <paramref name="value"/> to the corresponding type.
        /// </summary>
        /// <param name="value">The enum value to be cast.</param>
        /// <param name="result">The cast <paramref name="value"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> was cast; otherwise, <see langword="false"/>.</returns>
        public static bool SafeCast(TSource value, out TObject result) => _values.TryGetValue(value, out result);

        /// <summary>
        /// Parses a <see cref="string"/> object.
        /// </summary>
        /// <param name="obj">The object to be parsed.</param>
        /// <returns>The corresponding <typeparamref name="TObject"/> object instance, or <see langword="null"/> if not found.</returns>
        public static TObject Parse(string obj)
        {
            foreach (TObject value in _values.Values.Where(value => 0 == string.Compare(value.Name, obj, true)))
                return value;

            return null;
        }

        /// <summary>
        /// Converts the <see cref="EnumClass{TSource, TObject}"/> instance to a human-readable <see cref="string"/> representation.
        /// </summary>
        /// <returns>A human-readable <see cref="string"/> representation of the <see cref="EnumClass{TSource, TObject}"/> instance.</returns>
        public override string ToString() => _name;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) =>
            obj != null && (obj is TSource value ? Value.Equals(value) : obj is TObject derived && Value.Equals(derived.Value));

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(TObject other) => Value.Equals(other.Value);

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(TObject other) => Value.CompareTo(other.Value);

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(object obj) =>
            obj == null ? -1 : obj is TSource value ? Value.CompareTo(value) : obj is TObject derived ? Value.CompareTo(derived.Value) : -1;

        /// <summary>
        /// Compares the specified object instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="x">An object to compare.</param>
        /// <param name="y">Another object to compare.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int Compare(TObject x, TObject y) => x == null ? -1 : y == null ? 1 : x.Value.CompareTo(y.Value);
    }
}
