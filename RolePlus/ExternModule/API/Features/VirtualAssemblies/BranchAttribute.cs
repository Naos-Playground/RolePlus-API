// -----------------------------------------------------------------------
// <copyright file="BranchAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.VirtualAssemblies
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="Branch"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BranchAttribute : Attribute
    {
#pragma warning disable SA1401 // Fields should be private
        internal readonly string Master;
        internal readonly string Name;
        internal readonly string Prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAttribute"/> class.
        /// </summary>
        /// <param name="master">The master project.</param>
        /// <param name="name">The branch name.</param>
        /// <param name="prefix">The branch prefix.</param>
        public BranchAttribute(string master, string name, string prefix)
        {
            Master = master;
            Name = name;
            Prefix = prefix;
        }
    }
}
