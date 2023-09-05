// -----------------------------------------------------------------------
// <copyright file="CustomTeamTypeBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
<<<<<<< HEAD
    using Exiled.API.Features.Core.Generic;
=======
    using RolePlus.ExternModule.API.Engine.Framework.Generic;
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75

    /// <summary>
    /// All available custom teams.
    /// </summary>
    public class CustomTeamTypeBase : UnmanagedEnumClass<uint, CustomTeamTypeBase>
    {
        /// <summary>
        /// Represents an invalid custom role.
        /// </summary>
        public static readonly CustomTeamTypeBase None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeamTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        protected CustomTeamTypeBase(uint value) : base(value)
        {
        }
    }
}
