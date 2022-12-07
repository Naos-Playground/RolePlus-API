// -----------------------------------------------------------------------
// <copyright file="UNetServer.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network.Models
{
    using Exiled.API.Features;

    /// <summary>
    /// An object used to communicate across the network.
    /// </summary>
    public class UNetServer : UNetObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UNetServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public UNetServer(ushort port)
        {
            NetworkAuthority |= ENetworkAuthority.Server;
            Port = port;
        }

        /// <inheritdoc cref="Server.Host"/>
        public static Player NetHost => Server.Host;

        /// <inheritdoc cref="Server.Name"/>
        public static string DisplayName
        {
            get => ServerConsole._serverName;
            set
            {
                ServerConsole._serverName = value;
                ServerConsole.singleton.RefreshServerName();
            }
        }

        /// <inheritdoc cref="Server.IpAddress"/>
        public static string IPAddress => ServerConsole.Ip;

        /// <inheritdoc cref="Server.FriendlyFire"/>
        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set => ServerConsole.FriendlyFire = value;
        }

        /// <inheritdoc cref="Server.PlayerCount"/>
        public static int PlayerCount => Player.Dictionary.Count;

        /// <inheritdoc cref="Server.MaxPlayerCount"/>
        public static int MaxPlayerCount
        {
            get => CustomNetworkManager.slots;
            set => CustomNetworkManager.slots = value;
        }

        /// <inheritdoc cref="Server.Port"/>
        public ushort Port { get; internal set; }

        /// <inheritdoc cref="Server.Restart"/>
        public static void Restart() => Server.Restart();

        /// <inheritdoc cref="Server.Shutdown"/>
        public static void Shutdown() => Server.Shutdown();

        /// <inheritdoc cref="Server.RestartRedirect"/>
        public static bool RestartRedirect(ushort redirectPort) => Server.RestartRedirect(redirectPort);

        /// <inheritdoc cref="Server.ShutdownRedirect"/>
        public static bool ShutdownRedirect(ushort redirectPort) => Server.ShutdownRedirect(redirectPort);

        /// <inheritdoc cref="Server.RunCommand"/>
        public static void RunCommand(string command, CommandSender sender = null) => Server.RunCommand(command, sender);
    }
}
