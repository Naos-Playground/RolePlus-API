// -----------------------------------------------------------------------
// <copyright file="CompletedAnimationStateEventArgs.cs" company="NaoUnderscore">
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
    /// Contains all the information after a <see cref="ASchematicMeshComponent"/> completes an animation.
    /// </summary>
    public class CompletedAnimationStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompletedAnimationStateEventArgs"/> class.
        /// </summary>
        /// <param name="component"><inheritdoc cref="MeshComponent"/></param>
        /// <param name="animator"><inheritdoc cref="Animator"/></param>
        /// <param name="info"><inheritdoc cref="Info"/></param>
        public CompletedAnimationStateEventArgs(ASchematicMeshComponent component, Animator animator, AnimatorStateInfo info)
        {
            MeshComponent = component;
            Animator = animator;
            Info = info;
        }

        /// <summary>
        /// Gets the <see cref="ASchematicMeshComponent"/> which completed the animation.
        /// </summary>
        public ASchematicMeshComponent MeshComponent { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Animator"/>.
        /// </summary>
        public Animator Animator { get; }

        /// <summary>
        /// Gets the <see cref="AnimatorStateInfo"/> of the completed animation.
        /// </summary>
        public AnimatorStateInfo Info { get; }
    }
}
