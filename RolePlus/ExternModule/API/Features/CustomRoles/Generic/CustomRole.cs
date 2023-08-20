// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles.Generic
{
    using System;

    /// <inheritdoc/>
    public abstract class CustomRole<T> : CustomRole
        where T : RoleBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}
