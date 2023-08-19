// -----------------------------------------------------------------------
// <copyright file="FTransform.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
    using UnityEngine;

    /// <summary>
    /// Transform composed of Scale, Rotation (as a quaternion), and Translation.
    /// </summary>
    public struct FTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FTransform"/> struct.
        /// </summary>
        /// <param name="position"><inheritdoc cref="Translation"/></param>
        /// <param name="rotation"><inheritdoc cref="Rotation"/></param>
        /// <param name="scale"><inheritdoc cref="Scale"/></param>
        public FTransform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Translation = position;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FTransform"/> struct.
        /// </summary>
        /// <param name="target"><inheritdoc cref="FTransform"/></param>
        /// <param name="position"><inheritdoc cref="Translation"/></param>
        /// <param name="rotation"><inheritdoc cref="Rotation"/></param>
        /// <param name="scale"><inheritdoc cref="Scale"/></param>
        public FTransform(
            FTransform target,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
        {
            Translation = position != default ? position : target.Translation;
            Rotation = position != default ? rotation : target.Rotation;
            Scale = position != default ? scale : target.Scale;
        }

        /// <summary>
        /// Gets or sets the translation.
        /// </summary>
        public Vector3 Translation { get; set; }

        /// <summary>Gets or sets 2
        /// Gets or sets the rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public Vector3 Scale { get; set; }
    }
}
