// -----------------------------------------------------------------------
// <copyright file="AnimatorParameter.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
    using UnityEngine;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

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
    }
}
