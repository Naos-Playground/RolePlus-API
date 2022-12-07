﻿// -----------------------------------------------------------------------
// <copyright file="ConfigAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Configs
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="Config"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigAttribute"/> class.
        /// </summary>
        public ConfigAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigAttribute"/> class.
        /// </summary>
        /// <param name="folder"><inheritdoc cref="Folder"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="isParent"><inheritdoc cref="IsParent"/></param>
        public ConfigAttribute(string folder, string name, bool isParent = false)
        {
            Folder = folder;
            Name = name;
            IsParent = isParent;
        }

        /// <summary>
        /// Gets the folder's name.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets the file's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the class on which this
        /// attribute is being applied toshould be treated as parent.
        /// </summary>
        public bool IsParent { get; }
    }
}
