// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Roles;
using RolePlus.ExternModule.API.Enums;
using System.Diagnostics.SymbolStore;

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    /// <summary>
    /// A struct to easily manage escaping behavior.
    /// </summary>
    public struct EscapeSettings
    {
        /// <summary>
        /// Gets the default <see cref="EscapeSettings"/> values.
        /// </summary>
        public static EscapeSettings Default { get; } = new(true);

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> struct.
        /// </summary>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        public EscapeSettings(RoleType role) : this(true, role: role)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> struct.
        /// </summary>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        public EscapeSettings(uint customRole) : this(true, customRole: customRole)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> struct.
        /// </summary>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        public EscapeSettings(bool isAllowed = false, RoleType role = null, object customRole = null)
        {
            IsAllowed = isAllowed;
            Role = role;
            CustomRole = CustomRole.Get(customRole);
        }

        /// <summary>
        /// Gets or sets a value indicating whether escaping is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the role to be given when escaping.
        /// </summary>
        public RoleType Role { get;set; }

        /// <summary>
        /// Gets or sets the custom role to be given when escaping.
        /// </summary>
        public CustomRole CustomRole { get; set; }
    }
}
