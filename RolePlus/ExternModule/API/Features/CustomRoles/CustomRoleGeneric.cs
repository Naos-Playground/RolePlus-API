// -----------------------------------------------------------------------
// <copyright file="CustomRoleGeneric.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System;

    /// <inheritdoc/>
    public abstract class CustomRole<T> : CustomRole
        where T : RoleBuilder
    {
        /// <inheritdoc/>
        public override Type RoleBuilderComponent => typeof(T);
    }
}
