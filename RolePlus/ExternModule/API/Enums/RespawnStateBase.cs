// -----------------------------------------------------------------------
// <copyright file="RespawnStateBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// All available respawn states.
    /// </summary>
    public class RespawnStateBase : UnmanagedEnumClass<sbyte, RespawnStateBase>
    {
        /// <summary>
        /// Represents the disabled respawn state.
        /// </summary>
        public static readonly RespawnStateBase Disabled = new(0);

        /// <summary>
        /// Represents the disabled enabled state.
        /// </summary>
        public static readonly RespawnStateBase Enabled = new(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnStateBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value.</param>
        protected RespawnStateBase(sbyte value) : base(value)
        {
        }
    }
}
