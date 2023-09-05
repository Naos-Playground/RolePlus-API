// -----------------------------------------------------------------------
// <copyright file="RespawnStateBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
<<<<<<< HEAD
<<<<<<< HEAD
    using Exiled.API.Features.Core.Generic;
=======
    using RolePlus.ExternModule.API.Engine.Framework.Generic;
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75
=======
    using Exiled.API.Features.Core.Generic;
>>>>>>> d56c47334965da96730d299937b03a57f2fbe373

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
