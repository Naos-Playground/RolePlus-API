// -----------------------------------------------------------------------
// <copyright file="CustomEscape.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomEscapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    using MonoMod.Utils;

    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.Attributes;
    using RolePlus.ExternModule.API.Features.CustomRoles;

    using Hint = CustomHud.Hint;

    /// <summary>
    /// A class to easily manage escaping behavior.
    /// </summary>
    public abstract class CustomEscape : TypeCastObject<CustomEscape>
    {
        private static readonly List<CustomEscape> _registered = new();
        private static readonly Dictionary<EscapeScenarioTypeBase, Hint> _allScenarios = new();
        private static readonly Dictionary<Player, CustomEscape> _playerValues = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomEscape"/>'s.
        /// </summary>
        public static IEnumerable<CustomEscape> Registered => _registered;

        /// <summary>
        /// Gets all existing <see cref="Hint"/>'s to be displayed based on the relative <see cref="EscapeScenarioTypeBase"/>.
        /// </summary>
        public static IReadOnlyDictionary<EscapeScenarioTypeBase, Hint> AllScenarios => _allScenarios;

        /// <summary>
        /// Gets all players and their respective <see cref="CustomEscape"/>.
        /// </summary>
        public static IReadOnlyDictionary<Player, CustomEscape> Manager => _playerValues;

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s id.
        /// </summary>
        public virtual uint Id { get; }

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type EscapeBehaviourComponent { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomEscape"/> is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; }

        /// <summary>
        /// Gets all <see cref="Hint"/>'s to be displayed based on the relative <see cref="EscapeScenarioTypeBase"/>.
        /// </summary>
        protected virtual Dictionary<EscapeScenarioTypeBase, Hint> Scenarios { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape settings.
        /// </summary>
        protected virtual List<EscapeSettings> Settings { get; }

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomEscape left, object right)
        {
            try
            {
                uint value = (uint)right;
                return left.Id == value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomEscape left, object right)
        {
            try
            {
                uint value = (uint)right;
                return left.Id != value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(object left, CustomEscape right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomEscape right) => right != left;

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomEscape left, CustomEscape right) => left.Id == right.Id;

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomEscape left, CustomEscape right) => left.Id != right.Id;

        /// <summary>
        /// Enables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomEscape"/> which contains all the enabled custom roles.</returns>
        public static List<CustomEscape> RegisterAll()
        {
            List<CustomEscape> customEscapes = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if ((type.BaseType != typeof(CustomEscape) && !type.IsSubclassOf(typeof(CustomEscape))) || type.GetCustomAttribute(typeof(CustomEscapeAttribute)) is null)
                    continue;

                CustomEscape customEscape = Activator.CreateInstance(type) as CustomEscape;

                if (!customEscape.IsEnabled)
                    continue;

                customEscape.TryRegister();
                customEscapes.Add(customEscape);
            }

            if (customEscapes.Count() != Registered.Count())
                Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customEscapes.Count()} custom escapes have been successfully registered!", ConsoleColor.Cyan);

            return customEscapes;
        }

        /// <summary>
        /// Disables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomEscape"/> which contains all the disabled custom roles.</returns>
        public static List<CustomEscape> UnregisterAll()
        {
            List<CustomEscape> customEscapes = new();
            foreach (CustomEscape customEscape in Registered)
            {
                customEscape.TryUnregister();
                customEscapes.Add(customEscape);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {customEscapes.Count()} custom escapes have been successfully unregistered!", ConsoleColor.Cyan);

            return customEscapes;
        }

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified <see cref="Id"/>.
        /// </summary>
        /// <param name="customEscapeType">The specified <see cref="Id"/>.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomEscape Get(object customEscapeType) => Registered.FirstOrDefault(customEscape => customEscape == customEscapeType && customEscape.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomEscape Get(string name) => Registered.FirstOrDefault(customEscape => customEscape.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomEscape Get(Type type) => type.BaseType != typeof(EscapeBehaviour) ? null : Registered.FirstOrDefault(customEscape => customEscape == type);

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified <see cref="EscapeBehaviour"/>.
        /// </summary>
        /// <param name="escapeBuilder">The specified <see cref="EscapeBehaviour"/>.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomEscape Get(EscapeBehaviour escapeBuilder) => Get(escapeBuilder.GetType());

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="CustomEscape"/> owner.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomEscape Get(Player player)
        {
            CustomEscape customEscape = default;

            foreach (KeyValuePair<Player, CustomEscape> kvp in Manager)
            {
                if (kvp.Key != player)
                    continue;

                customEscape = Get(kvp.Value.Id);
            }

            return customEscape;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomEscape"/> given the specified <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="customEscapeType">The <see cref="object"/> to look for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object customEscapeType, out CustomEscape customEscape) => (customEscape = Get(customEscapeType)) is not null;

        /// <summary>
        /// Tries to get a <see cref="CustomEscape"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomEscape"/> name to look for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomEscape customEscape) => (customEscape = Registered.FirstOrDefault(cRole => cRole.Name == name)) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to search on.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player player, out CustomEscape customEscape) => (customEscape = Get(player)) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="escapeBuilder">The <see cref="EscapeBehaviour"/> to search for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(EscapeBehaviour escapeBuilder, out CustomEscape customEscape) => (customEscape = Get(escapeBuilder.GetType())) is not null;

        /// <summary>
        /// Tries to get the player's current <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomEscape customEscape) => (customEscape = Get(type.GetType())) is not null;

        /// <summary>
        /// Tries to register a <see cref="CustomEscape"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomEscape"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(x => x.Id == Id))
                {
                    Log.Debug(
                        $"[CustomEscapes] Couldn't register {Name}. " +
                        $"Another custom escape has been registered with the same Id:" +
                        $" {Registered.FirstOrDefault(x => x.Id == Id)}");

                    return false;
                }

                _allScenarios.AddRange(Scenarios);
                _registered.Add(this);

                return true;
            }

            Log.Debug($"[CustomRoles] Couldn't register {Name}. This custom escape has been already registered.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomRole"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug($"[CustomEscapes] Couldn't unregister {Name}. This custom escape hasn't been registered yet.");

                return false;
            }

            foreach (EscapeScenarioTypeBase scenario in Scenarios.Keys)
                _allScenarios.Remove(scenario);

            _registered.Remove(this);

            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is CustomEscape customEscape && customEscape == this;

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Attaches a <see cref="CustomEscape"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to attach the escape rules to.</param>
        public void Attach(Player player)
        {
            _playerValues.Remove(player);
            _playerValues.Add(player, this);
            player.AddComponent(EscapeBehaviourComponent, $"ECS-{Name}");
        }

        /// <summary>
        /// Detaches a <see cref="CustomEscape"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to detach the escape rules from.</param>
        public void Detach(Player player)
        {
            _playerValues.Remove(player);
            player.GetComponent(EscapeBehaviourComponent).Destroy();
        }
    }
}
