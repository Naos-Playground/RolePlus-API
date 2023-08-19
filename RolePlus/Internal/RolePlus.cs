// -----------------------------------------------------------------------
// <copyright file="RolePlus.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.Internal
{
    using System;

    using Exiled.API.Features;
    using global::RolePlus.ExternModule.API.Engine.Framework.Bootstrap;
    using global::RolePlus.ExternModule.API.Features;
    using global::RolePlus.ExternModule.API.Features.VirtualAssemblies;

    using HarmonyLib;

    using ConfigAPI = ExternModule.API.Features.Configs.Config;
    using ServerEvents = Exiled.Events.Handlers.Server;

    internal class RolePlus : Plugin<Config>
    {
        internal static RolePlus Singleton { get; set; }

        /// <summary>
        /// Gets the <see cref="global::RolePlus.Internal.ServerHandler"/> instance.
        /// </summary>
        internal ServerHandler ServerHandler { get; private set; }

        /// <summary>
        /// Gets the <see cref="Harmony"/> instance.
        /// </summary>
        internal Harmony HarmonyInstance { get; private set; }

        /// <inheritdoc/>
        public override string Author => "Nao";

        /// <inheritdoc/>
        public override string Name => "RolePlus";

        /// <inheritdoc/>
        public override string Prefix => "RolePlus";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new(5, 2, 1);

        /// <inheritdoc/>
        public override Version Version => new(6, 0, 0, 18);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Singleton ??= this;

            ConfigAPI.LoadAll();
            RegisterEvents();
            Branch.RegisterBranches();

            HarmonyInstance ??= GlobalPatchProcessor.PatchAll("roleplus", nameof(RolePlus));

            ServerConsole.AddLog(
                @$"You're running
██████╗  ██████╗ ██╗     ███████╗██████╗ ██╗     ██╗   ██╗███████╗
██╔══██╗██╔═══██╗██║     ██╔════╝██╔══██╗██║     ██║   ██║██╔════╝
██████╔╝██║   ██║██║     █████╗  ██████╔╝██║     ██║   ██║███████╗
██╔══██╗██║   ██║██║     ██╔══╝  ██╔═══╝ ██║     ██║   ██║╚════██║
██║  ██║╚██████╔╝███████╗███████╗██║     ███████╗╚██████╔╝███████║
╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚══════╝╚═╝     ╚══════╝ ╚═════╝ ╚══════╝ v{Version}", ConsoleColor.Red);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnReloaded()
        {
            ConfigAPI.LoadAll();
            Branch.ReloadBranches();

            base.OnReloaded();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            GlobalPatchProcessor.UnpatchAll("roleplus", nameof(RolePlus));

            Branch.UnregisterBranches();
            UnregisterEvents();

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            ServerHandler = new();

            RespawnManager.Start();
            SpawnManager.Start();
            CommonPatchProcessor.BeginEventHandlersDelivery();

            ServerEvents.ReloadedConfigs += () => ConfigAPI.LoadAll();
            ServerEvents.RoundStarted += ServerHandler.OnRoundStart;
            ServerEvents.EndingRound += ServerHandler.OnRoundEnding;
            ServerEvents.RoundEnded += ServerHandler.OnRoundEnded;
            ServerEvents.RestartingRound += ServerHandler.OnRestartingRound;
            ServerEvents.WaitingForPlayers += ServerHandler.OnWaitingForPlayers;
        }

        private void UnregisterEvents()
        {
            RespawnManager.Stop();
            SpawnManager.Stop();
            CommonPatchProcessor.StopEventHandlersDelivery();

            ServerEvents.ReloadedConfigs -= () => ConfigAPI.LoadAll();
            ServerEvents.RoundStarted -= ServerHandler.OnRoundStart;
            ServerEvents.EndingRound -= ServerHandler.OnRoundEnding;
            ServerEvents.RoundEnded -= ServerHandler.OnRoundEnded;
            ServerEvents.RestartingRound -= ServerHandler.OnRestartingRound;
            ServerEvents.WaitingForPlayers -= ServerHandler.OnWaitingForPlayers;

            ServerHandler = null;
            Singleton = null;
        }
    }
}