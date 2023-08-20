// -----------------------------------------------------------------------
// <copyright file="BranchTypeBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using RolePlus.ExternModule.API.Engine.Framework;

    /// <summary>
    /// All available branch environments.
    /// </summary>
    public class BranchTypeBase : UnmanagedEnumClass<sbyte, BranchTypeBase>
    {
        /// <summary>
        /// The production branch.
        /// </summary>
        public static readonly BranchTypeBase Release = new(0);

        /// <summary>
        /// The debug branch.
        /// </summary>
        public static readonly BranchTypeBase Debug = new (0);

        /// <summary>
        /// The development branch.
        /// </summary>
        public static readonly BranchTypeBase Dev = new (0);

        /// <summary>
        /// The beta branch.
        /// </summary>
        public static readonly BranchTypeBase Beta = new (0);

        /// <summary>
        /// The alpha branch.
        /// </summary>
        public static readonly BranchTypeBase Alpha = new (0);

        /// <summary>
        /// The prealpha branch.
        /// </summary>
        public static readonly BranchTypeBase Prealpha = new (0);

        /// <summary>
        /// The unstable branch.
        /// </summary>
        public static readonly BranchTypeBase Unstable = new (0);

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value.</param>
        protected BranchTypeBase(sbyte value) : base(value)
        {
        }
    }
}
