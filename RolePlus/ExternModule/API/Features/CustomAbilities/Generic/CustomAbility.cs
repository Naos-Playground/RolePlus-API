// -----------------------------------------------------------------------
// <copyright file="CustomAbility.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities.Generic
{
    using System;

    /// <inheritdoc/>
    public abstract class CustomAbility<T> : CustomAbility
        where T : AbilityBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}