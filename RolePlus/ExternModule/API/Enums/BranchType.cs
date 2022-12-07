// -----------------------------------------------------------------------
// <copyright file="BranchType.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    /// <summary>
    /// All available environments.
    /// </summary>
    public enum BranchType
    {
        /// <summary>
        /// The production branch.
        /// </summary>
        Release,

        /// <summary>
        /// The debug branch.
        /// </summary>
        Debug,

        /// <summary>
        /// The development branch.
        /// </summary>
        Dev,

        /// <summary>
        /// The beta branch.
        /// </summary>
        Beta,

        /// <summary>
        /// The alpha branch.
        /// </summary>
        Alpha,

        /// <summary>
        /// The prealpha branch.
        /// </summary>
        Prealpha,

        /// <summary>
        /// The unstable branch.
        /// </summary>
        Unstable,
    }
}
