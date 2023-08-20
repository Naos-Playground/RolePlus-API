// -----------------------------------------------------------------------
// <copyright file="ChangingAnimationStateEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs
{
    using System;

    using RolePlus.ExternModule.API.Engine.Components;

    using UnityEngine;

    /// <summary>
    /// Contains all the information before a <see cref="ASchematicMeshComponent"/> changes animation state.
    /// </summary>
    public class ChangingAnimationStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAnimationStateEventArgs"/> class.
        /// </summary>
        /// <param name="component"><inheritdoc cref="MeshComponent"/></param>
        /// <param name="animator"><inheritdoc cref="Animator"/></param>
        /// <param name="param"><inheritdoc cref="AnimationParameter"/></param>
        public ChangingAnimationStateEventArgs(ASchematicMeshComponent component, Animator animator, AnimatorParameter param)
        {
            MeshComponent = component;
            Animator = animator;
            AnimationParameter = param;
        }

        /// <summary>
        /// Gets the <see cref="ASchematicMeshComponent"/> changing animation state.
        /// </summary>
        public ASchematicMeshComponent MeshComponent { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Animator"/>.
        /// </summary>
        public Animator Animator { get; }

        /// <summary>
        /// Gets the <see cref="AnimatorParameter"/> which is being changed to.
        /// </summary>
        public AnimatorParameter AnimationParameter { get; }
    }
}
