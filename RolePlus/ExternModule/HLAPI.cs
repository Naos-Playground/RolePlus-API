// -----------------------------------------------------------------------
// <copyright file="HLAPI.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    using MapEditorReborn.API.Features.Objects;

    using Mirror;
    using NorthwoodLib.Pools;
    using RemoteAdmin;
    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.API.Features.CustomTeams;
    using RolePlus.ExternModule.API.Features.VirtualAssemblies;
    using RolePlus.Internal;

    using UnityEngine;

    /// <summary>
    /// The fully exposed methods and properties library.
    /// </summary>
    public static class HLAPI
    {
        private static RespawnManager _respawnManager;
        private static RoundManager _roundManager;

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="Player"/> containing all the players that are currently ignored by functions which use NoClip event(s) as base-logic.
        /// </summary>
        public static HashSet<Player> IgnoredNoClipPlayers { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> of all available <see cref="CustomRole"/>s.
        /// </summary>
        public static IEnumerable<CustomRole> RegisteredRoles => CustomRole.Registered;

        /// <summary>
        /// Gets a <see cref="List{T}"/> of all available <see cref="CustomTeam"/>s.
        /// </summary>
        public static IEnumerable<CustomTeam> RegisteredTeams => CustomTeam.Registered;

        /// <summary>
        /// Gets a <see cref="List{T}"/> of all available <see cref="CustomGamemode"/>s.
        /// </summary>
        public static IEnumerable<CustomGamemode> RegisteredGamemodes => CustomGamemode.Registered;

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/>  of available <see cref="Branch"/>es.
        /// </summary>
        public static IEnumerable<Branch> RegisteredBranches => Branch.Registered;

        /// <summary>
        /// Gets the <see cref="API.Features.RespawnManager"/>.
        /// </summary>
        public static RespawnManager RespawnManager => _respawnManager ??= StaticActor.Get<RespawnManager>();

        /// <summary>
        /// Gets the <see cref="API.Features.RoundManager"/>.
        /// </summary>
        public static RoundManager RoundManager => _roundManager ??= StaticActor.Get<RoundManager>();

        /// <summary>
        /// Gets or sets a value indicating whether or not the round is locked.
        /// </summary>
        public static bool IsRoundLocked
        {
            get => RoundManager.IsLocked;
            set => RoundManager.IsLocked = value;
        }

        /// <inheritdoc cref="RespawnManager.TimeUntilNextRespawn"/>
        public static int TimeUntilNextRespawn
        {
            get => RespawnManager.TimeUntilNextRespawn;
            set => RespawnManager.TimeUntilNextRespawn = value;
        }

        /// <inheritdoc cref="RespawnManager.IsWaitingForPlayers"/>
        public static bool IsWaitingForPlayers => RespawnManager.IsWaitingForPlayers;

        /// <inheritdoc cref="RespawnManager.State"/>
        public static RespawnStateBase RespawnState
        {
            get => RespawnManager.State;
            set => RespawnManager.State = value;
        }

        /// <inheritdoc cref="RespawnManager.IsEnabled"/>
        public static bool IsRespawnEnabled
        {
            get => RespawnManager.IsEnabled;
            set => RespawnManager.IsEnabled = value;
        }

        /// <inheritdoc cref="RespawnManager.LastSpawnedPlayers"/>
        public static HashSet<Player> LastSpawnedPlayers
        {
            get => RespawnManager.LastSpawnedPlayers;
            set => RespawnManager.LastSpawnedPlayers = value;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> from the given <see cref="Pickup"/> instance.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> result.</param>
        /// <returns><see langword="true"/> if the given <paramref name="pickup"/> is a <see cref="CustomItem"/>; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetCustomItem(this Pickup pickup, out CustomItem customItem) => CustomItem.TryGet(pickup, out customItem);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> from the given <see cref="Pickup"/> instance.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns>The corresponding <see cref="CustomItem"/> or <see langword="null"/> if not found.</returns>
        public static CustomItem GetCustomItem(this Pickup pickup) => CustomItem.TryGet(pickup, out CustomItem customItem) ? customItem : null;

        /// <summary>
        /// Gets a value indicating whether the given <paramref name="position"/> is stuck.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="position"/> is stuck; otherwise, <see langword="false"/>.</returns>
        public static bool IsStuck(this Vector3 position)
        {
            bool result = false;
            foreach (Collider collider in Physics.OverlapBox(position, new Vector3(0.4f, 1f, 0.4f), new Quaternion(0f, 0f, 0f, 0f)))
            {
                bool flag = collider.name.Contains("Hitbox") || collider.name.Contains("mixamorig") ||
                    collider.name.Equals("Player") || collider.name.Equals("PlyCenter") || collider.name.Equals("Antijumper");

                if (!flag)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Moves a <see cref="NetworkIdentity"/> to a new position.
        /// </summary>
        /// <param name="identity">The object to move.</param>
        /// <param name="pos">The new position.</param>
        public static void MoveNetworkIdentityObject(this NetworkIdentity identity, Vector3 pos)
        {
            identity.gameObject.transform.position = pos;
            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            foreach (Player ply in Player.List)
            {
                ply.Connection.Send(objectDestroyMessage, 0);
                typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { identity, ply.Connection });
            }
        }

        /// <summary>
        /// Gets a <see cref="NetworkIdentity"/> given its name.
        /// </summary>
        /// <param name="name">The name of the <see cref="NetworkIdentity"/> to look for.</param>
        /// <returns>The corresponding <see cref="NetworkIdentity"/> or <see langword="null"/> if not found.</returns>
        public static NetworkIdentity GetNetworkIdentity(string name)
        {
            foreach (NetworkIdentity identity in
                from identity in UnityEngine.Object.FindObjectsOfType<NetworkIdentity>()
                where identity.name == name
                select identity)
                return identity;

            return null;
        }

        /// <summary>
        /// Spawns an active grenade.
        /// </summary>
        /// <param name="position">The position on which spawn the grenade.</param>
        /// <param name="isFlash">A value indicating whether the grenade is <see cref="FlashGrenade"/>.</param>
        /// <param name="fuseDuration">The fuse duration.</param>
        /// <param name="owner">The owner of the grenade.</param>
        public static void SpawnActiveGrenade(this Vector3 position, bool isFlash = false, float fuseDuration = 0.1f, Player owner = null)
        {
            if (isFlash)
            {
                FlashGrenade grenade = Item.Create(ItemType.GrenadeFlash) as Throwable as FlashGrenade;
                grenade.FuseTime = fuseDuration;
                grenade.SpawnActive(position, owner);
            }
            else
            {
                ExplosiveGrenade grenade = Item.Create(ItemType.GrenadeHE) as ExplosiveGrenade;
                grenade.FuseTime = fuseDuration;
                grenade.SpawnActive(position, owner);
            }
        }

        /// <summary>
        /// Spawns an active grenade.
        /// </summary>
        /// <param name="position">The position on which spawn the grenade.</param>
        /// <param name="magnitude">The explosion radius.</param>
        /// <param name="fuseDuration">The fuse duration.</param>
        /// <param name="owner">The owner of the grenade.</param>
        public static void SpawnActiveGrenade(this Vector3 position, float magnitude, float fuseDuration = 0.1f, Player owner = null)
        {
            ExplosiveGrenade grenade = Item.Create(ItemType.GrenadeHE) as ExplosiveGrenade;
            grenade.FuseTime = fuseDuration;
            grenade.MaxRadius = magnitude;
            grenade.SpawnActive(position, owner);
        }

        /// <summary>
        /// Validates the usage of a <see cref="ParentCommand"/> returning all the available commands if the check fails.
        /// </summary>
        /// <param name="parentCommand">The command to check.</param>
        /// <param name="response">The response.</param>
        /// <returns>Always <see langword="false"/>.</returns>
        public static bool InvalidateCommandUsage(ParentCommand parentCommand, out string response)
        {
            StringBuilder builder = new();
            builder.AppendLine($"[{Assembly.GetCallingAssembly().GetName().Name}] Couldn't execute command '{parentCommand.Command}': usage cannot be validated.");
            builder.AppendLine("Available commands: ");

            foreach (ICommand command in parentCommand.AllCommands)
                builder.Append($"| {command.Command} |");

            response = builder.ToString();
            return false;
        }

        /// <summary>
        /// Invalidates the usage of a <see cref="ICommand"/>.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="error">The error message.</param>
        /// <param name="response">The response.</param>
        /// <returns>Always <see langword="false"/>.</returns>
        public static bool InvalidateCommandUsage(ICommand command, string error, out string response)
        {
            StringBuilder builder = new();
            builder.AppendLine($"[{Assembly.GetCallingAssembly().GetName().Name}] Couldn't execute command '{command.Command}': ");
            builder.Append(error);

            response = builder.ToString();
            return false;
        }

        /// <summary>
        /// Validates the usage of a <see cref="ICommand"/>.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="requiredArgs">The required arguments .</param>
        /// <param name="response">The response.</param>
        /// <returns><see langword="true"/> if the command can be validated; otherwise, <see langword="false"/>.</returns>
        public static bool ValidateCommandUsage(ICommand command, ArraySegment<string> arguments, int[] requiredArgs, out string response)
        {
            if (requiredArgs.Any(cnt => cnt == arguments.Count))
            {
                response = string.Empty;
                return true;
            }

            StringBuilder builder = new();
            builder.AppendLine($"[{Assembly.GetCallingAssembly().GetName().Name}] Couldn't execute command '{command.Command}': ");
            builder.Append("Invalid arguments.");
            response = builder.ToString();

            return false;
        }

        /// <summary>
        /// Validates the usage of a <see cref="ParentCommand"/> returning all the available commands if the check fails.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="sender">The command sender.</param>
        /// <param name="requiredPermissions">The required permissions.</param>
        /// <param name="response">The response.</param>
        /// <returns><see langword="true"/> if the command can be validated; otherwise, <see langword="false"/>.</returns>
        public static bool ValidateCommandUsage(ICommand command, ICommandSender sender, PlayerPermissions[] requiredPermissions, out string response)
        {
            if (sender.CheckPermission(requiredPermissions))
            {
                response = string.Empty;
                return true;
            }

            StringBuilder builder = new();
            builder.AppendLine($"[{Assembly.GetCallingAssembly().GetName().Name}] Couldn't execute command '{command.Command}': ");
            builder.Append("You don't have enough permissions to run this command.");

            response = builder.ToString();

            return false;
        }

        /// <summary>
        /// Validates the usage of a <see cref="ParentCommand"/> returning all the available commands if the check fails.
        /// </summary>
        /// <param name="command">The command to check.</param>
        /// <param name="sender">The command sender.</param>
        /// <param name="requiredPermission">The required permission.</param>
        /// <param name="response">The response.</param>
        /// <returns><see langword="true"/> if the command can be validated; otherwise, <see langword="false"/>.</returns>
        public static bool ValidateCommandUsage(ICommand command, ICommandSender sender, string requiredPermission, out string response)
        {
            if (sender.CheckPermission(requiredPermission))
            {
                response = string.Empty;
                return true;
            }

            StringBuilder builder = new();
            builder.AppendLine($"[{Assembly.GetCallingAssembly().GetName().Name}] Couldn't execute command '{command.Command}': ");
            builder.Append("You don't have enough permissions to run this command.");

            response = builder.ToString();

            return false;
        }

        /// <summary>
        /// Sets the scale of the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="target">The <see cref="Player"/> to modify.</param>
        /// <param name="x">The x axis value.</param>
        /// <param name="y">The y axis value.</param>
        /// <param name="z">The z axis value.</param>
        public static void SetScale(this Player target, float x, float y, float z)
        {
            try
            {
                target.Transform.localScale = new(1 * x, 1 * y, 1 * z);

                ObjectDestroyMessage destroyMessage = new()
                {
                    netId = target.NetId,
                };

                foreach (Player player in Player.List)
                {
                    if (player != target)
                        player.Connection.Send(destroyMessage, 0);

                    object[] parameters = new object[] { target.NetworkIdentity, player.Connection };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        /// <summary>
        /// Sets the scale of the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="target">The <see cref="Player"/> to modify.</param>
        /// <param name="scale">The new scale.</param>
        public static void SetScale(this Player target, float scale)
        {
            try
            {
                target.Transform.localScale = Vector3.one * scale;

                ObjectDestroyMessage destroyMessage = new()
                {
                    netId = target.NetId,
                };

                foreach (Player player in Player.List)
                {
                    if (player == target)
                        continue;

                    player.Connection.Send(destroyMessage, 0);

                    object[] parameters = new object[] { target.NetworkIdentity, player.Connection };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        /// <summary>
        /// Logs a command.
        /// </summary>
        /// <param name="query">The <see cref="QueryProcessor"/> from which log the command.</param>
        /// <param name="command">The used command.</param>
        public static void LogCommandUsed(QueryProcessor query, string command)
        {
            Player sender = Player.Get(query._hub);
            File.AppendAllText(Paths.Log, $@"{DateTime.Now}: {sender.Nickname} ({sender.Id}) tried executed: {command} {Environment.NewLine}");
            Log.Info($"{DateTime.Now}: {sender.Nickname} ({sender.Nickname}) tried to execute: {command}");
        }

        /// <summary>
        /// Formats arguments in a readable-command representation.
        /// </summary>
        /// <param name="args">The arguments to be formatted.</param>
        /// <param name="index">The amount of arguments to format.</param>
        /// <returns>The formatted arguments.</returns>
        public static string FormatArguments(string[] args, int index)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();

            foreach (string word in args.Segment(index))
            {
                sb.Append(word);
                sb.Append(" ");
            }

            string msg = sb.ToString();
            StringBuilderPool.Shared.Return(sb);

            return msg;
        }
    }
}
