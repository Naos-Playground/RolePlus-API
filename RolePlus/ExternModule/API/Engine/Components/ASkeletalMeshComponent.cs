// -----------------------------------------------------------------------
// <copyright file="ASkeletalMeshComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using System.Collections.Generic;
    using Exiled.API.Features.Core;

    using UnityEngine;

    /// <summary>
    /// The base class for custom meshes.
    /// </summary>
    public abstract class ASkeletalMeshComponent : EActor
    {
        private static readonly HashSet<ASkeletalMeshComponent> _meshInstances = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ASkeletalMeshComponent"/> class.
        /// </summary>
        protected ASkeletalMeshComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASkeletalMeshComponent"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="EObject.Base"/></param>
        protected ASkeletalMeshComponent(GameObject gameObject = null)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASkeletalMeshComponent"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="EObject.Base"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASkeletalMeshComponent(GameObject gameObject, Vector3 scale, Vector3 position, Quaternion rotation)
            : base(gameObject)
        {
            Scale = scale;
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mesh can be seen by other players.
        /// </summary>
        public virtual bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mesh is collidable.
        /// </summary>
        public virtual bool IsCollidable { get; set; }

        /// <summary>
        /// Gets or sets the value which determines the size of one tick.
        /// </summary>
        public override float FixedTickRate { get; set; }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            _meshInstances.Add(this);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            _meshInstances.Remove(this);
        }

        /// <summary>
        /// Changes the visibility state for all the <see cref="ASkeletalMeshComponent"/> instances.
        /// </summary>
        /// <param name="state">The new state.</param>
        public static void ChangeVisibilityAll(bool state)
        {
            foreach (ASkeletalMeshComponent controller in _meshInstances)
                controller.IsVisible = state;
        }
    }
}
