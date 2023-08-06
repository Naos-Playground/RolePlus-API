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

    using MEC;

    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.Internal;

    /// <summary>
    /// CustomAbility is the base class used to create user-defined types treated as abilities applicable to a <see cref="Player"/>.
    /// </summary>
    public abstract class CustomAbility : AActor
    {
        private CoroutineHandle _abilityCooldownHandle;
        private byte _level;

        internal static readonly Dictionary<Player, CustomAbility> _playersValue = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAbility"/> class.
        /// </summary>
        /// <param name="cooldown">The ability cooldown.</param>
        protected CustomAbility(float cooldown = 0f)
            : base() => Cooldown = cooldown;

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="CustomAbility"/> containing all the registered custom abilites.
        /// </summary>
        public static IReadOnlyList<CustomAbility> Registered
        {
            get
            {
                HashSet<CustomAbility> customAbilities = new();
                foreach (CustomAbility ability in FindActiveObjectsOfType<CustomAbility>())
                    customAbilities.Add(ability);

                return customAbilities.ToList();
            }
        }

        /// <summary>
        /// Gets the ability manager which contains all the players with a <see cref="CustomAbility"/>.
        /// </summary>
        public static HashSet<Player> Manager => _playersValue.Keys.ToHashSet();

        /// <summary>
        /// Gets the players and their respective <see cref="CustomAbility"/>.
        /// </summary>
        public static IReadOnlyDictionary<Player, CustomAbility> PlayersValue => _playersValue;

        /// <summary>
        /// Gets the owner of the ability.
        /// </summary>
        public Player Owner { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ability is ready.
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ability is active.
        /// <br>Value is changed only if the ability is duration-based.</br>
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets or sets the required cooldown before using the ability again.
        /// </summary>
        public virtual float Cooldown { get; protected set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is used.
        /// </summary>
        public virtual string UsedMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation is denied.
        /// </summary>
        public virtual string DeniedActivationMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is unlocked.
        /// </summary>
        public virtual string UnlockedMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reaches a new level.
        /// </summary>
        public virtual string NewLevelReachedMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the time to wait before the ability is activated.
        /// </summary>
        public virtual float WindupTime { get; protected set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        public virtual float Duration { get; protected set; }

        /// <summary>
        /// Gets or sets the level of the ability.
        /// </summary>
        public virtual byte Level
        {
            get => _level;
            set => OnLevelAdded(value);
        }

        /// <summary>
        /// Gets a value indicating whether the ability can enter the cooldown phase.
        /// </summary>
        protected virtual bool CanEnterCooldown => !IsReady;

        /// <summary>
        /// Gets a value indicating whether the ability can be used.
        /// </summary>
        protected virtual bool CanBeUsed => true;

        /// <summary>
        /// Gets the ability type name.
        /// </summary>
        public virtual string Type => GetUObjectTypeFromRegisteredTypes(GetType(), Name).Name;

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> given the specified type name.
        /// </summary>
        /// <param name="type">The ability type name.</param>
        /// <returns>The corresponding <see cref="CustomAbility"/>, or <see langword="null"/> if not found.</returns>
        public static CustomAbility Get(string type) => Registered.FirstOrDefault(t => t.Type.ToLower() == type.ToLower());

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> from a <see cref="Player"/> given the specified type name.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="type">The ability type name.</param>
        /// <returns>The corresponding <see cref="CustomAbility"/>, or <see langword="null"/> if not found.</returns>
        public static CustomAbility Get(Player player, string type) => _playersValue.FirstOrDefault(kvp => kvp.Value.Owner == player && kvp.Value.Type == type).Value;

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> from a <see cref="Player"/> given the specified type name.
        /// </summary>
        /// <typeparam name="T">The <see cref="CustomAbility"/> type.</typeparam>
        /// <param name="player">The player to check.</param>
        /// <returns>The corresponding <see cref="CustomAbility"/>, or <see langword="null"/> if not found.</returns>
        public static T Get<T>(Player player)
            where T : CustomAbility => _playersValue.FirstOrDefault(kvp => kvp.Value.Owner == player && kvp.Value.GetType() == typeof(T)).Value.Cast<T>();

        /// <summary>
        /// Adds a <typeparamref name="T"/> <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The custom ability type.</typeparam>
        /// <param name="player">The player to affect.</param>
        /// <param name="customAbility">The given ability.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool AddAbility<T>(Player player, out T customAbility)
            where T : CustomAbility
        {
            customAbility = null;

            if (_playersValue.ContainsKey(player))
                return false;

            T outer = Activator.CreateInstance<T>();
            customAbility = CreateDefaultSubobject<T>(player.GameObject, outer.Type);
            _playersValue.Add(player, customAbility);

            return customAbility is not null;
        }

        /// <summary>
        /// Adds a <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool AddAbility(Player player, Type type)
        {
            if (_playersValue.ContainsKey(player))
                return false;

            Type t = GetUObjectTypeFromRegisteredTypes(type);
            if (t is null)
                return false;

            CustomAbility customAbility = CreateDefaultSubobject(t, player.GameObject).Cast<CustomAbility>();
            _playersValue.Add(player, customAbility);

            return customAbility is not null;
        }

        /// <summary>
        /// Adds a <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was given successfully; otherwise, <see langword="false"/>.</returns>
        public static bool AddAbility(Player player, string type)
        {
            if (_playersValue.ContainsKey(player))
                return false;

            CustomAbility customAbility = Get(type);
            if (customAbility is null)
                return false;

            AddAbility(player, customAbility.GetType());
            _playersValue.Add(player, customAbility);

            return customAbility is not null;
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void AddAbilities(Player player, IEnumerable<Type> types)
        {
            foreach (Type type in types)
                AddAbility(player, type);
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void AddAbilities(Player player, IEnumerable<string> types)
        {
            foreach (string type in types)
                AddAbility(player, type);
        }

        /// <summary>
        /// Removes a <typeparamref name="T"/> <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The custom ability type.</typeparam>
        /// <param name="player">The player to affect.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool RemoveAbility<T>(Player player)
            where T : CustomAbility
        {
            _playersValue.Remove(player);
            return DestroyActiveObject<T>(player.GameObject);
        }

        /// <summary>
        /// Removes a <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool RemoveAbility(Player player, Type type)
        {
            _playersValue.Remove(player);
            return DestroyActiveObject(type, player.GameObject);
        }

        /// <summary>
        /// Removes a <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="type">The custom ability type.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public static bool RemoveAbility(Player player, string type)
        {
            Type t = GetUObjectTypeFromRegisteredTypesByName(type);
            if (t is null)
                return false;

            _playersValue.Remove(player);
            return DestroyActiveObject(t, player.GameObject);
        }

        /// <summary>
        /// Removes all the custom abilities from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        public static void RemoveAbilities(Player player)
        {
            foreach (KeyValuePair<Player, CustomAbility> kvp in _playersValue)
            {
                if (kvp.Key != player)
                    continue;

                RemoveAbility(player, kvp.Value.GetType());
            }
        }

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void RemoveAbilities(Player player, IEnumerable<Type> types)
        {
            foreach (KeyValuePair<Player, CustomAbility> kvp in _playersValue)
            {
                if (kvp.Key != player || types.Contains(kvp.Value.GetType()))
                    continue;

                RemoveAbility(player, kvp.Value.GetType());
            }
        }

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to affect.</param>
        /// <param name="types">The custom abilities.</param>
        public static void RemoveAbilities(Player player, IEnumerable<string> types)
        {
            foreach (KeyValuePair<Player, CustomAbility> kvp in _playersValue)
            {
                if (kvp.Key != player || types.Contains(kvp.Value.Type))
                    continue;

                RemoveAbility(player, kvp.Value.GetType());
            }
        }

        /// <summary>
        /// Enables all the custom abilities present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all the enabled custom abilities.</returns>
        public static IEnumerable<CustomAbility> RegisterAbilities()
        {
            List<CustomAbility> customAbilities = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(CustomAbility)) || type.GetCustomAttribute(typeof(CustomAbilityAttribute)) is null)
                    continue;

                CustomAbility customAbility = Activator.CreateInstance(type) as CustomAbility;

                if (customAbility.TryRegister())
                    customAbilities.Add(customAbility);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] [CustomAbility] {customAbilities.Count()} custom abilities have been successfully registered!", ConsoleColor.DarkGreen);
            return customAbilities;
        }

        /// <summary>
        /// Disables all the custom abilities present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all the disabled custom abilities.</returns>
        public static IEnumerable<CustomAbility> UnregisterAbilities()
        {
            List<CustomAbility> customAbilities = new();
            foreach (CustomAbility ability in Registered)
            {
                if (ability.TryUnregister())
                    customAbilities.Add(ability);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] [CustomAbility] {customAbilities.Count()} custom abilities have been successfully unregistered!", ConsoleColor.DarkGreen);
            return customAbilities;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomAbility"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomAbility"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(x => x.Type == Type))
                {
                    Log.Debug(
                        $"[CustomAbility] Couldn't register {Name}. Another CustomAbility has been registered with the same Type: " +
                        $" {Registered.FirstOrDefault(x => x.Type == Type)}");

                    return false;
                }

                RegisterUObjectType(GetType(), Name);

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

            UnregisterUObjectType(Name);

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="Player"/> is the owner of this <see cref="CustomAbility"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Player"/> is the owner of this <see cref="CustomAbility"/>; otherwise, <see langword="false"/>.</returns>
        public bool Check(Player player) => player is not null && player == Owner;

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Owner = Player.Get(Base);
            _abilityCooldownHandle = Timing.RunCoroutine(AbilityCooldown());
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (OnAddingLevel(Level))
                Level += 1;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Timing.KillCoroutines(_abilityCooldownHandle);
        }

        /// <summary>
        /// Fired when the ability is used.
        /// </summary>
        protected virtual void OnAbilityUsed()
        {
            if (!CanBeUsed)
            {
                if (!string.IsNullOrEmpty(DeniedActivationMessage))
                    Owner.ShowHint(DeniedActivationMessage, 4);

                return;
            }

            if (Duration != 0f)
            {
                IsActive = true;

                Timing.CallDelayed(Duration, () =>
                {
                    IsReady = false;
                    IsActive = false;
                    OnAbilityExpired();
                });

                return;
            }

            IsReady = false;
        }

        /// <summary>
        /// Fired when the time's up after using the ability.
        /// <para>Called using duration-based abilities.</para>
        /// </summary>
        protected virtual void OnAbilityExpired()
        {
        }

        /// <summary>
        /// Fired when the level is changed.
        /// </summary>
        /// <param name="level">The new level.</param>
        protected virtual void OnLevelAdded(byte level)
        {
            _level = level;
            Owner.ShowHint(NewLevelReachedMessage.Replace("%level", Level.ToString()), 5);
        }

        /// <summary>
        /// Fired before adding a new level.
        /// </summary>
        /// <param name="curLevel">The current level.</param>
        /// <returns><see langword="true"/> if the level can change; otherwise, <see langword="false"/>.</returns>
        protected virtual bool OnAddingLevel(byte curLevel) => false;

        private IEnumerator<float> AbilityCooldown()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;

                if (!CanEnterCooldown)
                    continue;

                IsReady = false;
                yield return Timing.WaitForSeconds(Cooldown);
                IsReady = true;
            }
        }
    }
}