// -----------------------------------------------------------------------
// <copyright file="CustomAbility.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    using HarmonyLib;
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;
    using RolePlus.ExternModule.API.Features.CustomRoles;

    /// <summary>
    /// CustomAbility is the base class used to create user-defined types treated as abilities applicable to a <see cref="Player"/>.
    /// </summary>
    public abstract class CustomAbility : TypeCastObject<CustomAbility>, IAddittiveBehaviour
    {
        private static readonly List<CustomAbility> _registered = new();

        internal static readonly Dictionary<Player, HashSet<CustomAbility>> PlayersValue = new();

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="CustomAbility"/> containing all the registered custom abilites.
        /// </summary>
        public static IEnumerable<CustomAbility> Registered => _registered;

        /// <summary>
        /// Gets all players and all their respective <see cref="CustomAbility"/>'s.
        /// </summary>
        public static IReadOnlyDictionary<Player, HashSet<CustomAbility>> Manager => PlayersValue;

        /// <summary>
        /// Gets all players belonging to a <see cref="CustomAbility"/>.
        /// </summary>
        public static IEnumerable<Player> Players => PlayersValue.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="AbilityBehaviour"/>.
        /// </summary>
        public abstract Type BehaviourComponent { get; }

        /// <summary>
        /// Gets the ability type name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomAbility"/>'s id.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the ability is enabled.
        /// </summary>
        public virtual bool IsEnabled => true;

        /// <summary>
        /// Gets a <see cref="CustomRole"/> given the specified <paramref name="customAbilityType"/>.
        /// </summary>
        /// <param name="customAbilityType">The specified ability type.</param>
        /// <returns>The <see cref="CustomRole"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomAbility Get(object customAbilityType) => Registered.FirstOrDefault(customAbility => customAbility == customAbilityType && customAbility.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomAbility Get(string name) => Registered.FirstOrDefault(customAbility => customAbility.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> given the specified <see cref="Name"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Name"/>.</param>
        /// <returns>The <see cref="CustomAbility"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomAbility Get(Type type) => type.BaseType != typeof(AbilityBehaviour) ? null : Registered.FirstOrDefault(customAbility => customAbility.BehaviourComponent == type);

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> given the specified <see cref="AbilityBehaviour"/>.
        /// </summary>
        /// <param name="abilityBuilder">The specified <see cref="AbilityBehaviour"/>.</param>
        /// <returns>The <see cref="CustomAbility"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomAbility Get(AbilityBehaviour abilityBuilder) => Get(abilityBuilder.GetType());

        /// <summary>
        /// Gets all <see cref="CustomAbility"/>'s from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="CustomAbility"/> owner.</param>
        /// <returns>The <see cref="CustomAbility"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static IEnumerable<CustomAbility> Get(Player player) => Manager.FirstOrDefault(kvp => kvp.Key == player).Value;

        /// <summary>
        /// Tries to get a <see cref="CustomAbility"/> given the specified <paramref name="customAbility"/>.
        /// </summary>
        /// <param name="customAbilityType">The <see cref="object"/> to look for.</param>
        /// <param name="customAbility">The found <paramref name="customAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <paramref name="customAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object customAbilityType, out CustomAbility customAbility) => (customAbility = Get(customAbilityType)) is not null;

        /// <summary>
        /// Tries to get a <paramref name="customAbility"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomAbility"/> name to look for.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomAbility customAbility) => (customAbility = Registered.FirstOrDefault(cAbility => cAbility.Name == name)) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomAbility"/>'s.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to search on.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility"/>'s, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player player, out IEnumerable<CustomAbility> customAbility) => (customAbility = Get(player)) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="abilityBuilder">The <see cref="AbilityBehaviour"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(AbilityBehaviour abilityBuilder, out CustomAbility customAbility) => (customAbility = Get(abilityBuilder.GetType())) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="type">The <see cref="Name"/> to search for.</param>
        /// <param name="customAbility">The found <see cref="CustomAbility"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomAbility customAbility) => (customAbility = Get(type.GetType())) is not null;

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> from a <see cref="Player"/> given the specified type name.
        /// </summary>
        /// <typeparam name="T">The <see cref="CustomAbility"/> type.</typeparam>
        /// <param name="player">The player to check.</param>
        /// <returns>The corresponding <see cref="CustomAbility"/>, or <see langword="null"/> if not found.</returns>
        public static EActor Get<T>(Player player)
            where T : CustomAbility
        {
            CustomAbility customAbility = Get(typeof(T));
            return player.GetComponent(customAbility.BehaviourComponent);
        }

        /// <summary>
        /// Adds a <typeparamref name="T"/> <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The custom ability type.</typeparam>
        /// <param name="player">The player to affect.</param>
        /// <param name="param">The given ability.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Add<T>(Player player, out T param)
            where T : CustomAbility
        {
            param = null;

            if (!TryGet(typeof(T), out CustomAbility ability))
                return false;

            (param = ability.Cast<T>()).Add(player);
            return true;
        }

        /// <summary>
        /// Adds a <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Add(Player player, Type type)
        {
            if (!TryGet(type, out CustomAbility customAbility))
                return false;

            customAbility.Add(player);
            return true;
        }

        /// <summary>
        /// Adds a <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Add(Player player, string type)
        {
            if (!TryGet(type, out CustomAbility customAbility))
                return false;

            customAbility.Add(player);
            return true;
        }

        /// <summary>
        /// Adds a <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="id">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Add(Player player, uint id)
        {
            if (!TryGet(id, out CustomAbility customAbility))
                return false;

            customAbility.Add(player);
            return true;
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void Add(Player player, IEnumerable<Type> types)
        {
            foreach (Type type in types)
                Add(player, type);
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void Add(Player player, IEnumerable<string> types)
        {
            foreach (string type in types)
                Add(player, type);
        }

        /// <summary>
        /// Removes a <typeparamref name="T"/> <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The custom ability type.</typeparam>
        /// <param name="player">The player to affect.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Remove<T>(Player player)
            where T : CustomAbility =>
            TryGet(typeof(T), out CustomAbility customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, player.GameObject);

        /// <summary>
        /// Removes a <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Remove(Player player, Type type) =>
            TryGet(type, out CustomAbility customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, player.GameObject);

        /// <summary>
        /// Removes a <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Remove(Player player, string type) =>
            TryGet(type, out CustomAbility customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, player.GameObject);

        /// <summary>
        /// Removes a <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="id">The custom ability id.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Remove(Player player, uint id) =>
            TryGet(id, out CustomAbility customAbility) && EObject.DestroyActiveObject(customAbility.BehaviourComponent, player.GameObject);

        /// <summary>
        /// Removes all the custom abilities from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        public static void RemoveAll(Player player) =>
            PlayersValue.DoIf(kvp => kvp.Key == player, (kvp) => kvp.Value.Do((ability) => ability.Remove(player)));

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void RemoveRange(Player player, IEnumerable<Type> types) =>
            PlayersValue.DoIf(kvp => kvp.Key == player, (kvp) => kvp.Value.DoIf(ability => types.Contains(ability.GetType()), (ability) => ability.Remove(player)));

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void RemoveRange(Player player, IEnumerable<string> types) =>
            PlayersValue.DoIf(kvp => kvp.Key == player, (kvp) => kvp.Value.DoIf(ability => types.Contains(ability.Name), (ability) => ability.Remove(player)));

        /// <summary>
        /// Enables all the custom abilities present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all the enabled custom abilities.</returns>
        public static IEnumerable<CustomAbility> EnableAll()
        {
            List<CustomAbility> customAbilities = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                CustomAbilityAttribute attribute = type.GetCustomAttribute<CustomAbilityAttribute>();
                if (!type.IsSubclassOf(typeof(CustomAbility)) || attribute is null)
                    continue;

                CustomAbility customAbility = Activator.CreateInstance(type) as CustomAbility;

                if (!customAbility.IsEnabled)
                    continue;

                if (customAbility.TryRegister(attribute))
                    customAbilities.Add(customAbility);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] [CustomAbility] {customAbilities.Count()} custom abilities have been successfully registered!", ConsoleColor.DarkGreen);

            return customAbilities;
        }

        /// <summary>
        /// Disables all the custom abilities present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all the disabled custom abilities.</returns>
        public static IEnumerable<CustomAbility> DisableAll()
        {
            List<CustomAbility> customAbilities = new();
            customAbilities.AddRange(Registered.Where(ability => ability.TryUnregister()));

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] [CustomAbility] {customAbilities.Count()} custom abilities have been successfully unregistered!", ConsoleColor.DarkGreen);

            return customAbilities;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomAbility"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomAbilityAttribute attribute = null)
        {
            if (!Registered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                    Id = attribute.Id;

                if (Registered.Any(x => x.Name == Name))
                {
                    Log.Debug(
                        $"[CustomAbility] Couldn't register {Name}. Another CustomAbility has been registered with the same Type: " +
                        $" {Registered.FirstOrDefault(x => x.Name == Name)}");

                    return false;
                }

                EObject.RegisterObjectType(BehaviourComponent, Name);
                _registered.Add(this);

                return true;
            }

            Log.Debug(
                $"[CustomAbility] Couldn't register {Name}." +
                $"This CustomAbility has been already registered.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomAbility"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug(
                    $"[CustomAbility] Couldn't unregister {Name}." +
                    $"This CustomAbility hasn't been registered yet.");

                return false;
            }

            EObject.UnregisterObjectType(Name);
            _registered.Remove(this);

            return true;
        }

        /// <summary>
        /// Adds a <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        public void Add(Player player)
        {
            player.AddComponent(BehaviourComponent);

            try
            {
                PlayersValue[player].Add(this);
            }
            catch
            {
                PlayersValue.Add(player, new HashSet<CustomAbility>() { this });
            }
        }

        /// <summary>
        /// Removes a <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Player player)
        {
            try
            {
                if (Get(player).Count() == 1)
                    PlayersValue.Remove(player);
                else
                    PlayersValue[player].Remove(this);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Activates a <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="isForced">A value indicating whether the activation should be forced.</param>
        /// <returns><see langword="true"/> if the ability was activated; otherwise, <see langword="false"/>.</returns>
        public bool Activate(Player player, bool isForced = false) => player.TryGetComponent(BehaviourComponent, out AbilityBehaviour ability) && ability.Activate(isForced);
    }
}