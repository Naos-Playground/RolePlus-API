// -----------------------------------------------------------------------
// <copyright file="CustomEscapeTypeBase.cs" company="NaoUnderscore">
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
    /// All available custom escapes.
    /// </summary>
    public class CustomEscapeTypeBase : UnmanagedEnumClass<uint, CustomRoleTypeBase>
    {
        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static readonly CustomEscapeTypeBase None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEscapeTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        protected CustomEscapeTypeBase(uint value) : base(value)
        {
        }
    }
}
