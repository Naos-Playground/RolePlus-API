// -----------------------------------------------------------------------
// <copyright file="UNetManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Exiled.API.Features;

    /// <summary>
    /// An object which manages server data across the network.
    /// </summary>
    public class UNetManager
    {
        /// <summary>
        /// Gets the static instance.
        /// </summary>
        public static UNetManager Singleton { get; internal set; }

        /// <summary>
        /// Gets all the entities data.
        /// </summary>
        public Dictionary<string, UNetEntity> EntityData { get; private set; } = new();

        /// <summary>
        /// Gets all the servers.
        /// </summary>
        public List<UNetServer> Servers { get; private set; } = new();

        /// <summary>
        /// Gets the data path.
        /// </summary>
        public string DataPath { get; private set; }

        /// <summary>
        /// Loads server data.
        /// </summary>
        /// <returns>A <see cref="UNetServer"/> containing all the server data.</returns>
        public UNetServer LoadServerData()
        {
            if (!Directory.Exists(Internal.RolePlus.Singleton.Config.DataDirectory))
                Directory.CreateDirectory(Internal.RolePlus.Singleton.Config.DataDirectory);

            string server = Path.Combine(Internal.RolePlus.Singleton.Config.DataDirectory, $"serverport.txt");
            File.WriteAllText(server, Server.Port.ToString());
            return new UNetServer(Server.Port);
        }

        /// <summary>
        /// Loads entity data.
        /// </summary>
        /// <param name="userId">The user id of the entity.</param>
        /// <returns>The loaded entity.</returns>
        public UNetEntity LoadEntityData(string userId)
        {
            string targetUserIdPath = Path.Combine(Path.Combine(Internal.RolePlus.Singleton.Config.DataDirectory, DataPath), $"{userId}.txt");

            if (!Directory.Exists(Internal.RolePlus.Singleton.Config.DataDirectory))
                Directory.CreateDirectory(Internal.RolePlus.Singleton.Config.DataDirectory);

            return !File.Exists(targetUserIdPath) ?
                new UNetEntity(userId, true, true, DateTime.Now) :
                ParseEntityData(targetUserIdPath);
        }

        /// <summary>
        /// Saves entity data.
        /// </summary>
        /// <param name="data">The entity to save.</param>
        public void SaveEntityData(UNetEntity data)
        {
            string targetUserIdPath = Path.Combine(Path.Combine(Internal.RolePlus.Singleton.Config.DataDirectory, DataPath), $"{data.UserId}.txt");

            if (!Directory.Exists(Internal.RolePlus.Singleton.Config.DataDirectory))
                Directory.CreateDirectory(Internal.RolePlus.Singleton.Config.DataDirectory);

            string[] textdata = new string[]
            {
                data.LastUpdate.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
                data.UserId,
                data.IsSteamLimited.ToString(),
                data.IsSteamVACBanned.ToString(),
            };

            File.WriteAllLines(targetUserIdPath, textdata);
        }

        private UNetEntity ParseEntityData(string path)
        {
            string[] text = File.ReadAllLines(path);
            return new UNetEntity(text[0], bool.Parse(text[1]), bool.Parse(text[2]), DateTime.Parse(text[3]));
        }
    }
}
