// -----------------------------------------------------------------------
// <copyright file="EscapeSettings.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace RolePlus.ExternModule.API.Features.CustomEscapes
{
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using UnityEngine;

    /// <summary>
    /// A tool to easily setup escapes.
    /// </summary>
    public struct EscapeSettings : IAddittiveProperty
    {
        /// <summary>
        /// The default distance tolerance value.
        /// </summary>
        public const float DefaultMaxDistanceTolerance = 3f;

        /// <summary>
        /// Gets the default escape position.
        /// </summary>
        public static readonly Vector3 DefaultPosition = Escape.WorldPos;

        /// <summary>
        /// Gets the default <see cref="EscapeSettings"/> values.
        /// </summary>
        public static readonly EscapeSettings Default = new(true);

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> struct.
        /// </summary>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="maxDistanceTolerance"><inheritdoc cref="MaxDistanceTolerance"/></param>
        public EscapeSettings(RoleType role, Vector3 position = default, float maxDistanceTolerance = DefaultMaxDistanceTolerance)
            : this(true, role: role, position: position, maxDistanceTolerance: maxDistanceTolerance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> struct.
        /// </summary>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="maxDistanceTolerance"><inheritdoc cref="MaxDistanceTolerance"/></param>
        public EscapeSettings(uint customRole, Vector3 position = default, float maxDistanceTolerance = DefaultMaxDistanceTolerance)
            : this(true, customRole: customRole, position: position, maxDistanceTolerance: maxDistanceTolerance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> struct.
        /// </summary>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="maxDistanceTolerance"><inheritdoc cref="MaxDistanceTolerance"/></param>
        public EscapeSettings(bool isAllowed = false, RoleType role = null, object customRole = null,
            Vector3 position = default, float maxDistanceTolerance = DefaultMaxDistanceTolerance)
        {
            IsAllowed = isAllowed;
            Role = role;
            CustomRole = CustomRole.Get(customRole);
            Position = position == default ? DefaultPosition : position;
            MaxDistanceTolerance = maxDistanceTolerance == DefaultMaxDistanceTolerance ? DefaultMaxDistanceTolerance : maxDistanceTolerance;
        }

        /// <summary>
        /// Gets or sets a value indicating whether escaping is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the role to be given when escaping.
        /// </summary>
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the custom role to be given when escaping.
        /// </summary>
        public CustomRole CustomRole { get; set; }

        /// <summary>
        /// Gets or sets the escape position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the distance tolerance which defines the maximum distance at which the settings will be applied.
        /// </summary>
        public float MaxDistanceTolerance { get; set; }
    }
}
