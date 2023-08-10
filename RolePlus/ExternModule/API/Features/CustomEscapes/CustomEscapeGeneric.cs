// -----------------------------------------------------------------------
// <copyright file="CustomEscapeGeneric.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomEscapes
{
    using System;

    /// <summary>
    /// A class to easily manage escaping behavior.
    /// </summary>
    public abstract class CustomEscape<T> : CustomEscape
        where T : EscapeBuilder
    {
        /// <inheritdoc/>
        public override Type EscapeBuilderComponent => typeof(T);
    }
}
