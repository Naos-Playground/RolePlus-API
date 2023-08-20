// -----------------------------------------------------------------------
// <copyright file="CustomEscape.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomEscapes.Generic
{
    using System;

    /// <summary>
    /// A class to easily manage escaping behavior.
    /// </summary>
    /// <typeparam name="T">The <see cref="EscapeBehaviour"/> type.</typeparam>
    public abstract class CustomEscape<T> : CustomEscape
        where T : EscapeBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}
