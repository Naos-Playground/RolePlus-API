// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Configs
{
    /// <summary>
    /// Defines the contract for basic config features.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the config should be read.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be shown.
        /// </summary>
        public bool ShowDebugMessages { get; set; }
    }
}
