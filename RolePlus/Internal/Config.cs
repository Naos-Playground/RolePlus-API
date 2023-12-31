﻿// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using System.IO;

    using Exiled.API.Features;

    internal sealed partial class Config : Exiled.API.Interfaces.IConfig
    {
        public Config() => DataDirectory = Path.Combine(Paths.Plugins, "RPIData");

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        public bool ShowDebugMessages { get; set; } = false;

        public string DataDirectory { get; private set; } = string.Empty;
    }
}
