// -----------------------------------------------------------------------
// <copyright file="CustomGamemode.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;

    using MEC;

    using RolePlus.ExternModule.API.Features.Controllers;
    using RolePlus.Internal;

#pragma warning disable SA1402 // File may only contain a single type

    /// <inheritdoc/>
    public abstract class CustomGamemode<T> : CustomGamemode
        where T : class
    {
        /// <inheritdoc/>
        public override Type PlayerScript => typeof(T);
    }

    /// <summary>
    /// A class to easily manage custom gamemodes.
    /// </summary>
    public abstract class CustomGamemode
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomGamemode"/>'s.
        /// </summary>
        public static List<CustomGamemode> Registered { get; private set; } = new();

        /// <summary>
        /// Gets all the players with a <see cref="PlayerScript"/>.
        /// </summary>
        public static List<Player> Players { get; private set; } = new();

        /// <summary>
        /// Gets the currently loaded <see cref="CustomGamemode"/>.
        /// </summary>
        public static CustomGamemode CurrentGamemode { get; private set; }

        /// <summary>
        /// Gets the previously loaded <see cref="CustomGamemode"/>.
        /// </summary>
        public static CustomGamemode PreviousGamemode { get; private set; }

        /// <summary>
        /// Gets the <see cref="PlayerScriptController"/>.
        /// </summary>
        public virtual Type PlayerScript { get; private set; }

        /// <summary>
        /// Gets the gamemode type.
        /// </summary>
        public string Type => GetType().Name;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets the name to be displayed.
        /// </summary>
        public virtual string DisplayName { get; }

        /// <summary>
        /// Gets a value indicating whether the gamemode is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; }

        /// <summary>
        /// Gets the ending delay before restarting the server.
        /// </summary>
        public virtual float EndingDelay => 10f;

        /// <summary>
        /// Gets a value indicating whether the respawn is enabled.
        /// </summary>
        public virtual bool IsRespawnAllowed { get; }

        /// <summary>
        /// Gets the <see cref="Broadcast"/> to be displayed when the game starts.
        /// </summary>
        public virtual Broadcast StartingBroadcast { get; }

        /// <summary>
        /// Gets a value indicating whether the gamemode is registered.
        /// </summary>
        public bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Tries to get a <see cref="CustomGamemode"/> given a specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <param name="customGamemode">The found <see cref="CustomGamemode"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomGamemode"/> has been found successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomGamemode customGamemode)
        {
            customGamemode = default;

            if (customGamemode.Get(name) is null)
            {
                customGamemode = null;
                return false;
            }

            customGamemode = customGamemode.Get(name);

            return true;
        }

        /// <summary>
        /// Tries to start a <see cref="CustomGamemode"/> given a <see cref="CustomGamemode"/> instance.
        /// </summary>
        /// <param name="customGamemode">The <see cref="CustomGamemode"/> instance.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomGamemode"/> has been started successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryStart(CustomGamemode customGamemode)
        {
            if (customGamemode is null)
                return false;

            customGamemode.OnStarted();

            return true;
        }

        /// <summary>
        /// Tries to start a <see cref="CustomGamemode"/> given a specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomGamemode"/> has been started successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryStart(string name)
        {
            if (!TryGet(name, out CustomGamemode customGamemode))
                return false;

            customGamemode.OnStarted();

            return true;
        }

        /// <summary>
        /// Tries to end a <see cref="CustomGamemode"/> given a <see cref="CustomGamemode"/> instance.
        /// </summary>
        /// <param name="customGamemode">The <see cref="CustomGamemode"/> instance.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomGamemode"/> has been ended successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryEnd(CustomGamemode customGamemode)
        {
            if (customGamemode is null)
                return false;

            customGamemode.OnEnded();

            return true;
        }

        /// <summary>
        /// Tries to end a <see cref="CustomGamemode"/> given a specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomGamemode"/> has been ended successfully; otherwise <see langword="false"/>.</returns>
        public static bool TryEnd(string name)
        {
            if (!TryGet(name, out CustomGamemode customGamemode))
                return false;

            customGamemode.OnEnded();

            return true;
        }

        /// <summary>
        /// Enables all the custom gamemodes present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomGamemode"/> which contains all the enabled custom gamemodes.</returns>
        public static IEnumerable<CustomGamemode> RegisterGamemodes()
        {
            List<CustomGamemode> customGamemodes = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if ((type.BaseType != typeof(CustomGamemode) && !type.IsSubclassOf(typeof(CustomGamemode))) || type.GetCustomAttribute(typeof(CustomGamemodeAttribute)) is null)
                    continue;

                CustomGamemode customGamemode = Activator.CreateInstance(type) as CustomGamemode;

                customGamemode.TryRegister();
                customGamemodes.Add(customGamemode);
            }

            if (customGamemodes.Count() != Registered.Count())
                Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customGamemodes.Count()} custom gamemodes have been successfully registered!", ConsoleColor.Cyan);

            return customGamemodes;
        }

        /// <summary>
        /// Disables all the custom gamemodes present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomGamemode"/> which contains all the disabled custom gamemodes.</returns>
        public static IEnumerable<CustomGamemode> UnregisterGamemodes()
        {
            List<CustomGamemode> customGamemodes = new();
            foreach (CustomGamemode customGamemode in Registered)
            {
                customGamemode.TryUnregister();
                customGamemodes.Remove(customGamemode);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customGamemodes.Count()} custom gamemodes have been successfully unregistered!", ConsoleColor.Cyan);

            return customGamemodes;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomGamemode"/>.
        /// </summary>
        /// <returns>Returns a value indicating whether the <see cref="CustomGamemode"/> was registered or not.</returns>
        public bool TryRegister()
        {
            if (!IsEnabled)
            {
                Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] Couldn't register {Name}. Check your config to enable it.", ConsoleColor.DarkMagenta);

                return false;
            }

            if (!Registered.Contains(this))
            {
                if (Registered.Any(x => x.Type == Type))
                {
                    Log.Debug(
                        $"[CustomGamemodes] Couldn't register {Name}. " +
                        $"Another CustomGamemode has been registered with the same GamemodeType:" +
                        $" {Registered.FirstOrDefault(x => x.Type == Type)}");

                    return false;
                }

                Registered.Add(this);

                return true;
            }

            Log.Debug(
                $"[CustomGamemodes] Couldn't register {Name}. This custom gamemode has been already registered.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomGamemode"/>.
        /// </summary>
        /// <returns>Returns a value indicating whether the <see cref="CustomGamemode"/> was unregistered or not.</returns>
        public bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug(
                    $"[CustomGamemodes] Couldn't unregister {Name}. This custom gamemode hasn't been registered yet.");

                return false;
            }

            Registered.Remove(this);

            return true;
        }

        /// <summary>
        /// Gets the <see cref="CustomGamemode"/> given a specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The corresponding <see cref="CustomGamemode"/> or <see langword="null"/> if not found.</returns>
        public CustomGamemode Get(string name) => Registered.FirstOrDefault(x => x.Name == name || x.Type == name);

        /// <summary>
        /// Starts the gamemode.
        /// </summary>
        protected virtual void OnStarted()
        {
            PreviousGamemode = CurrentGamemode;
            CurrentGamemode = this;

            SubscribeEvents();

            if (!IsRespawnAllowed)
                HLAPI.TimeUntilNextRespawn = int.MaxValue;

            RoundManager.IsLocked = true;
            RoundManager.CurrentGamemode = DisplayName;

            if (StartingBroadcast is not null)
                Map.Broadcast(StartingBroadcast, true);

            Players.AddRange(Player.List);

            foreach (Player player in Players)
                player.GameObject.AddComponent(PlayerScript);
        }

        /// <summary>
        /// Ends the gamemode.
        /// </summary>
        protected virtual void OnEnded()
        {
            PreviousGamemode = this;
            CurrentGamemode = null;
            UnsubscribeEvents();

            Timing.CallDelayed(EndingDelay, () => RoundManager.EndRound());
        }

        /// <summary>
        /// Subscribes all the events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes all the events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }
    }
}
