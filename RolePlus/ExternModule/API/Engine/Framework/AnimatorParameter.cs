// -----------------------------------------------------------------------
// <copyright file="AnimatorParameter.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A class to easily handle <see cref="AnimatorControllerParameter"/> objects.
    /// </summary>
    public class AnimatorParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatorParameter"/> class.
        /// </summary>
        /// <param name="animator"><inheritdoc cref="Animator"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        public AnimatorParameter(Animator animator, string name)
        {
            Animator = animator;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatorParameter"/> class.
        /// </summary>
        /// <param name="animator"><inheritdoc cref="Animator"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="state"><inheritdoc cref="State"/></param>
        public AnimatorParameter(Animator animator, string name, bool state)
        {
            Animator = animator;
            Name = name;
            State = state;
        }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Animator"/>.
        /// </summary>
        public Animator Animator { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the state of the parameter.
        /// </summary>
        public bool State
        {
            get => Animator.GetBool(Name);
            set
            {
                if (Animator.GetBool(Name) == value)
                    return;

                Animator.SetBool(Name, value);
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="AnimatorParameter"/> and <see cref="AnimatorParameter"/>.
        /// </summary>
        /// <param name="left">The left <see cref="AnimatorParameter"/> to compare.</param>
        /// <param name="right">The right <see cref="AnimatorParameter"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(AnimatorParameter left, AnimatorParameter right) => left.Name == right.Name && left.Animator == right.Animator;

        /// <summary>
        /// Compares two operands: <see cref="AnimatorParameter"/> and <see cref="AnimatorParameter"/>.
        /// </summary>
        /// <param name="left">The left <see cref="AnimatorParameter"/> to compare.</param>
        /// <param name="right">The right <see cref="AnimatorParameter"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(AnimatorParameter left, AnimatorParameter right) => left.Name != right.Name && left.Animator != right.Animator;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is AnimatorParameter parameter &&
                   EqualityComparer<Animator>.Default.Equals(Animator, parameter.Animator) &&
                   Name == parameter.Name &&
                   State == parameter.State;
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode()
        {
            int hashCode = -1978165504;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Animator>.Default.GetHashCode(Animator);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + State.GetHashCode();
            return hashCode;
        }
    }
}
