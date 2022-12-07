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

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    using MapEditorReborn.API.Features.Objects;

    using MEC;

    using Mirror;
    using NorthwoodLib.Pools;
    using RemoteAdmin;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features;
    using RolePlus.ExternModule.API.Features.Audio.API;
    using RolePlus.ExternModule.API.Features.CustomRoles;
    using RolePlus.ExternModule.API.Features.CustomTeams;
    using RolePlus.ExternModule.API.Features.VirtualAssemblies;
    using RolePlus.Internal;

    using UnityEngine;

    using Camera = Exiled.API.Features.Camera;

    /// <summary>
    /// The fully exposed methods and properties library.
    /// </summary>
    public static class HLAPI
    {
        private static Escape _escape;

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all the spawned schematics.
        /// </summary>
        public static List<SchematicObject> SchematicObjects { get; } = new();

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
        /// Gets the <see cref="CustomRole.Manager"/>.
        /// </summary>
        public static HashSet<Player> RoleManager => CustomRole.Manager;

        /// <summary>
        /// Gets the <see cref="CustomTeam.Manager"/>.
        /// </summary>
        public static HashSet<Player> TeamManager => CustomTeam.Manager;

        /// <summary>
        /// Gets the world's escape position.
        /// </summary>
        public static Vector3 WorldEscapePosition => WorldEscape.worldPosition;

        /// <summary>
        /// Gets the world's escape radius.
        /// </summary>
        public static float WorldEscapeRadius => Escape.radius;

        /// <summary>
        /// Gets the <see cref="Escape"/> component.
        /// </summary>
        public static Escape WorldEscape => _escape ??= UnityEngine.Object.FindObjectOfType<Escape>();

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

        /// <inheritdoc cref="RespawnManager.NextKnownTeam"/>
        public static CustomTeam.RespawnInfo NextKnownTeam => RespawnManager.NextKnownTeam;

        /// <inheritdoc cref="RespawnManager.State"/>
        public static RespawnState RespawnState
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
        /// Gets a value indicating whether or not the <see cref="Player"/> has a <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Player"/> has a <see cref="CustomRole"/>; otherwise, <see langword="false"/>.</returns>
        public static bool HasCustomRole(this Player player) => RoleManager.Contains(player);

        /// <summary>
        /// Add a <see cref="CustomItem"/> of the specified type to the player's inventory.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <param name="customItem">The item to be added.</param>
        /// <returns><see langword="true"/> if the item has been given to the player; otherwise, <see langword="false"/>.</returns>
        public static bool AddItem(this Player player, object customItem)
        {
            if (player is null || player.IsInventoryFull)
                return false;

            try
            {
                uint value = (uint)customItem;
                CustomItem.TryGive(player, (int)value);
                return true;
            }
            catch
            {
                if (customItem is CustomItem instance)
                    return CustomItem.TryGive(player, (int)instance.Id);

                return false;
            }
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="object"/> containing all the custom items to the player's inventory.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <param name="customItemTypes">The custom items to be added.</param>
        public static void AddItem(this Player player, IEnumerable<object> customItemTypes)
        {
            foreach (object customItemType in customItemTypes)
            {
                if (!player.AddItem(customItemType))
                    continue;
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> from the specified player.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="EffectType"/>.</returns>
        public static IEnumerable<EffectType> GetEffectTypes(this Player player)
        {
            foreach (object effect in Enum.GetValues(typeof(EffectType)))
            {
                if (!Enum.TryParse(effect.ToString(), out EffectType effectType) || !player.TryGetEffect(effectType, out _))
                    continue;

                yield return effectType;
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="object"/> containing all the custom items in <paramref name="player"/>'s inventory.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="object"/> which contains all the custom items found.</returns>
        public static IEnumerable<CustomItem> GetCustomItems(this Player player) => GetCustomItems(player.Items);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="object"/> containing all the custom items in <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to check.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="object"/> which contains all the custom items found.</returns>
        public static IEnumerable<CustomItem> GetCustomItems(this IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                if (!CustomItem.TryGet(item, out CustomItem customItem))
                    continue;

                yield return customItem;
            }
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> from the given <see cref="Item"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> result.</param>
        /// <returns><see langword="true"/> if the given <paramref name="item"/> is a <see cref="CustomItem"/>; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetCustomItem(this Item item, out CustomItem customItem) => CustomItem.TryGet(item, out customItem);

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> from the given <see cref="Pickup"/> instance.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <param name="customItem">The <see cref="CustomItem"/> result.</param>
        /// <returns><see langword="true"/> if the given <paramref name="pickup"/> is a <see cref="CustomItem"/>; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetCustomItem(this Pickup pickup, out CustomItem customItem) => CustomItem.TryGet(pickup, out customItem);

#nullable enable
        /// <summary>
        /// Gets a <see cref="CustomItem"/> from the specified <see cref="Item"/> instance.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns>The corresponding <see cref="object"/>; otherwise, <see langword="null"/>.</returns>
        public static CustomItem? GetCustomItem(this Item item) => CustomItem.TryGet(item, out CustomItem customItem) ? customItem : null;

        /// <summary>
        /// Gets a <see cref="CustomItem"/> from the given <see cref="Pickup"/> instance.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to check.</param>
        /// <returns>The corresponding <see cref="CustomItem"/> or <see langword="null"/> if not found.</returns>
        public static CustomItem? GetCustomItem(this Pickup pickup) => CustomItem.TryGet(pickup, out CustomItem customItem) ? customItem : null;
#nullable disable

        /// <summary>
        /// Sets the player's <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <param name="customRole">The role to be set.</param>
        public static void SetRole(this Player player, CustomRole customRole) => CustomRole.UnsafeSpawn(player, customRole);

        /// <summary>
        /// Sets the player's <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <param name="customRole">The role to be set.</param>
        /// <param name="shouldKeepPosition"><inheritdoc cref="CustomRole.ShouldKeepPosition"/></param>
        public static void SetRole(this Player player, object customRole, bool shouldKeepPosition = false) => CustomRole.UnsafeSpawn(player, customRole, shouldKeepPosition);

        /// <summary>
        /// Gets the player's <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <param name="customRole">The <see cref="CustomRole"/> result.</param>
        /// <returns>The found <see cref="CustomRole"/>, or <see langword="null"/> if not found.</returns>
        public static bool TryGetCustomRole(this Player player, out CustomRole customRole) => CustomRole.TryGet(player, out customRole);

        /// <summary>
        /// Gets the player's <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <returns>The found <see cref="CustomRole"/>, or <see langword="null"/> if not found.</returns>
        public static CustomRole GetCustomRole(this Player player)
        {
            if (!CustomRole.TryGet(player, out CustomRole customRole))
                return null;

            return customRole;
        }

        /// <summary>
        /// Gets the player's <see cref="CustomRole.Id"/>.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <returns>The corresponding <see cref="object"/>.</returns>
        public static object GetCustomRoleType(this Player player) => !CustomRole.TryGet(player, out CustomRole customRole) ? null : customRole.Id;

        /// <summary>
        /// Spawns the specified <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="customTeam">The team to be spawned.</param>
        public static void SpawnTeam(CustomTeam customTeam) => CustomTeam.TrySpawn(customTeam);

        /// <summary>
        /// Spawns the specified <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="customTeamType">The team to be spawned.</param>
        public static void SpawnTeam(object customTeamType) => CustomTeam.TrySpawn(customTeamType);

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="Player"/> is a Custom SCP.
        /// </summary>
        /// <param name="player">The specified <see cref="Player"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Player"/> is a custom scp; otherwise, <see langword="false"/>.</returns>
        public static bool IsCustomScp(this Player player) => CustomRole.TryGet(player, out CustomRole customScp) && customScp.IsScp;

        /// <summary>
        /// Safely drops an item in order to support custom items.
        /// </summary>
        /// <param name="player">The target.</param>
        /// <param name="item">The item to be dropped.</param>
        public static void SafeDropItem(this Player player, Item item)
        {
            if (CustomItem.TryGet(item, out CustomItem customItem))
            {
                player.RemoveItem(item, false);
                customItem.Spawn(player.Position, player);
                return;
            }

            player.DropItem(item);
        }

        /// <summary>
        /// Gets a value indicating whether the given <paramref name="position"/> is stuck.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns><see langword="true"/> if the positio <paramref name="position"/> is stuck; otherwise, <see langword="false"/>.</returns>
        public static bool IsStuck(this Vector3 position)
        {
            bool result = false;
            foreach (Collider collider in Physics.OverlapBox(position, new Vector3(0.4f, 1f, 0.4f), new Quaternion(0f, 0f, 0f, 0f)))
            {
                bool flag = collider.name.Contains("Hitbox") || collider.name.Contains("mixamorig") ||
                    collider.name.Equals("Player") || collider.name.Equals("PlyCenter") || collider.name.Equals("Antijumper");
                if (!flag)
                {
                    Log.Warn($"Detect:{collider.name}");
                    result = true;
                }
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
        /// Gets a value indicating whether the given <see cref="Camera"/> can look to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="camera">The camera watcher.</param>
        /// <param name="player">The target.</param>
        /// <param name="maxDistance">The maximum distance before dropping the check.</param>
        /// <returns><see langword="true"/> if the <paramref name="camera"/> can look to <paramref name="player"/>; otherwise, <see langword="false"/>.</returns>
        public static bool CanLookToPlayer(this Camera camera, Player player, float maxDistance)
        {
            if (player.IsDead || player.Role == RoleType.Scp079)
                return false;

            float num = Vector3.Dot(camera.Base.head.transform.forward, player.Position - camera.Position);
            return num >= 0f && num * num / (player.Position - camera.Position).sqrMagnitude > 0.4225f &&
                Physics.Raycast(camera.Position, player.Position - camera.Position, out RaycastHit raycastHit, maxDistance, -117407543) &&
                raycastHit.transform.name == player.GameObject.name;
        }

        /// <summary>
        /// Gets a value indicating whether the given <see cref="Player"/> can look to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        /// <param name="player">The target.</param>
        /// <param name="maxDistance">The maximum distance before dropping the check.</param>
        /// <returns><see langword="true"/> if the <paramref name="watcher"/> can look to <paramref name="player"/>; otherwise, <see langword="false"/>.</returns>
        public static bool CanLookToPlayer(this Player watcher, Player player, float maxDistance)
        {
            if (player.IsDead || watcher.IsDead || watcher.Role == RoleType.Scp079 || player.Role == RoleType.Scp079)
                return false;

            float num = Vector3.Dot(watcher.CameraTransform.forward, player.Position - watcher.Position);
            return num >= 0f && num * num / (player.Position - watcher.Position).sqrMagnitude > 0.4225f &&
                Physics.Raycast(watcher.Position, player.Position - watcher.Position, out RaycastHit raycastHit, maxDistance, -117407543) &&
                raycastHit.transform.name == player.GameObject.name;
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
        /// Teleports the specified <see cref="Player"/> to the given position through SCP-106 mechanics.
        /// </summary>
        /// <param name="player">The target to teleport.</param>
        /// <param name="position">The position to which teleport the target.</param>
        public static void PocketDimensionTeleport(this Player player, Vector3 position) => Timing.RunCoroutine(_PocketDimensionTeleport(player.ReferenceHub.scp106PlayerScript, position));

        /// <summary>
        /// Makes the specified <see cref="Player"/> fall to the given position.
        /// </summary>
        /// <param name="player">The target to make fall.</param>
        /// <param name="position">The fall position.</param>
        public static void MakeFall(this Player player, Vector3 position = default) => Timing.RunCoroutine(_MakeFall(player, position));

        /// <summary>
        /// Play and audio from a file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="volume">file volume.</param>
        public static void PlayAudio(string path, float volume) => Timing.RunCoroutine(AudioController.PlayFromFile(path, volume));

        /// <summary>
        /// Stops an audio from playing.
        /// </summary>
        public static void StopAudio() => AudioController.Stop();

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
        /// Sets the scale of the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="target">The <see cref="GameObject"/> to modify.</param>
        /// <param name="x">The x axis value.</param>
        /// <param name="y">The y axis value.</param>
        /// <param name="z">The z axis value.</param>
        public static void SetScale(this GameObject target, float x, float y, float z)
        {
            try
            {
                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();
                target.transform.localScale = new(1 * x, 1 * y, 1 * z);

                ObjectDestroyMessage destroyMessage = new()
                {
                    netId = identity.netId,
                };

                foreach (GameObject player in PlayerManager.players)
                {
                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;
                    if (player != target)
                        playerCon.Send(destroyMessage, 0);

                    object[] parameters = new object[] { identity, playerCon };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

        /// <summary>
        /// Spawns a dummy player.
        /// </summary>
        /// <param name="role">The role of the dummy.</param>
        /// <param name="position">The spawn position.</param>
        /// <param name="rotation">The spawn rotation.</param>
        /// <param name="nickname">The name to be given to the dummy.</param>
        /// <returns>A <see cref="GameObject"/> which represents the spawned dummy.</returns>
        internal static GameObject SpawnDummy(RoleType role, Vector3 position, Quaternion rotation, string nickname)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
            if (gameObject.TryGetComponent(out CharacterClassManager ccm))
            {
                ccm.CurClass = role;
                ccm.RefreshPlyModel();
            }

            if (gameObject.TryGetComponent(out NicknameSync nicknameSync))
                nicknameSync.Network_myNickSync = nickname;

            if (gameObject.TryGetComponent(out QueryProcessor queryProcessor))
            {
                queryProcessor.PlayerId = 9999;
                queryProcessor.NetworkPlayerId = 9999;
            }

            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            NetworkServer.Spawn(gameObject);
            return gameObject;
        }

        /// <summary>
        /// Sets the scale of the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="target">The <see cref="GameObject"/> to modify.</param>
        /// <param name="scale">The new scale.</param>
        public static void SetScale(this GameObject target, float scale)
        {
            try
            {
                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();
                target.transform.localScale = Vector3.one * scale;

                ObjectDestroyMessage destroyMessage = new()
                {
                    netId = identity.netId,
                };

                foreach (GameObject player in PlayerManager.players)
                {
                    if (player == target)
                        continue;

                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;
                    playerCon.Send(destroyMessage, 0);

                    object[] parameters = new object[] { identity, playerCon };
                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SA1300
#pragma warning disable SA1600
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static IEnumerator<float> _PocketDimensionTeleport(Scp106PlayerScript scp106PlayerScript, Vector3 position)
        {
            if (scp106PlayerScript.goingViaThePortal)
                yield break;

            scp106PlayerScript.RpcTeleportAnimation();
            scp106PlayerScript.goingViaThePortal = true;
            yield return Timing.WaitForSeconds(3.5f);
            scp106PlayerScript._hub.playerMovementSync.OverridePosition(position);
            yield return Timing.WaitForSeconds(7.5f);

            if (AlphaWarheadController.Host.detonated && scp106PlayerScript.transform.position.y < 800f)
                Player.Get(scp106PlayerScript._hub).Kill(DamageType.PocketDimension);

            scp106PlayerScript.goingViaThePortal = false;
        }

        public static IEnumerator<float> _MakeFall(this Player player, Vector3 position = default)
        {
            Scp106PlayerScript scp106PlayerScript = player.ReferenceHub.scp106PlayerScript;
            if (scp106PlayerScript.goingViaThePortal)
                yield break;

            scp106PlayerScript.goingViaThePortal = true;

            player.ReferenceHub.scp106PlayerScript.GrabbedPosition = player.Position + (Vector3.up * 1.5f);
            Vector3 startPosition = player.Position, endPosition = player.Position -= Vector3.up * 2;
            bool inGodMode = player.IsGodModeEnabled;
            player.IsGodModeEnabled = true;
            player.CanSendInputs = false;
            player.EnableEffect(EffectType.SinkHole);
            for (int i = 0; i < 30; i++)
            {
                player.Position = Vector3.Lerp(startPosition, endPosition, i / 30f);
                yield return 0.01f;
            }

            player.Position = position == default ? Vector3.down * 1997f : position;
            player.IsGodModeEnabled = inGodMode;
            player.CanSendInputs = true;
            player.DisableEffect(EffectType.SinkHole);
            scp106PlayerScript.goingViaThePortal = false;
        }

        public static void LogCommandUsed(QueryProcessor query, string command)
        {
            Player sender = Player.Get(query.PlayerId);
            File.AppendAllText(Paths.Log, $@"{DateTime.Now}: {sender.Nickname} ({sender.Id}) tried executed: {command} {Environment.NewLine}");
            Log.Info($"{DateTime.Now}: {sender.Nickname} ({sender.Nickname}) tried to execute: {command}");
        }

        public static string FormatArguments(string[] sentence, int index)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();
            foreach (string word in sentence.Segment(index))
            {
                sb.Append(word);
                sb.Append(" ");
            }

            string msg = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return msg;
        }

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1300
#pragma warning restore SA1600
    }
}
