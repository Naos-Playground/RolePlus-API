// -----------------------------------------------------------------------
// <copyright file="RootConfig.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal.Configs
{
    using global::RolePlus.ExternModule.API.Features.Configs;

    [Config(nameof(RolePlus), nameof(RootConfig), true)]
    internal sealed class RootConfig : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool ShowDebugMessages { get; set; } = false;
    }
}
