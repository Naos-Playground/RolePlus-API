// -----------------------------------------------------------------------
// <copyright file="RootConfig.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Configs
{
    using System.ComponentModel;
    using global::RolePlus.ExternModule.API.Features.Configs;

    [Config(nameof(RolePlus), nameof(RootConfig), true)]
    internal sealed class RootConfig : IConfig
    {
        /// <inheritdoc/>
        [Description("A value indicating whether the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        [Description("A value indicating whether debug messages should be shown.")]
        public bool ShowDebugMessages { get; set; } = false;
    }
}
