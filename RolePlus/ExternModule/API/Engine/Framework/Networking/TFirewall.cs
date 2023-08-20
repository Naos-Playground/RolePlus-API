// -----------------------------------------------------------------------
// <copyright file="TFirewall.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    /// <summary>
    /// The class which allows to handle <see cref="TRule{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The data type used by the managed rules.</typeparam>
    public class TFirewall<T> : TypeCastObject<TFirewall<T>>
    {
        private static readonly List<TFirewall<T>> _firewalls = new();
        private TRule<T>[] _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="TFirewall{T}"/> class.
        /// </summary>
        /// <param name="rule">The rule to handle.</param>
        protected TFirewall(TRule<T> rule)
        {
            _rules = new TRule<T>[1];
            _rules[0] = rule;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TFirewall{T}"/> class.
        /// </summary>
        /// <param name="rules">The rules to handle.</param>
        protected TFirewall(IEnumerable<TRule<T>> rules)
        {
            TRule<T>[] toLoad = rules.ToArray();
            _rules = new TRule<T>[toLoad.Length];
            for (int i = 0; i < toLoad.Length; i++)
                _rules[i] = toLoad[i];
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TFirewall{T}"/> class.
        /// </summary>
        ~TFirewall()
        {
            foreach (TRule<T> rule in _rules)
                rule.Unload();

            Log.SendRaw($"[TFirewall:{Name}] has been unloaded - Exit Result: Shutdown", ConsoleColor.DarkMagenta);
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="TFirewall{T}"/> containing all the active configurations.
        /// </summary>
        public static IReadOnlyCollection<TFirewall<T>> Configurations => _firewalls.AsReadOnly();

        /// <summary>
        /// Gets the active rules.
        /// </summary>
        public IReadOnlyCollection<TRule<T>> Rules => Array.AsReadOnly(_rules);

        /// <summary>
        /// Gets the name of the current configuration.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Configures a new <see cref="TFirewall{T}"/> instance.
        /// </summary>
        /// <param name="rule">The configuration.</param>
        /// <param name="name">The name to identify the firewall.</param>
        /// <returns>The configured <see cref="TFirewall{T}"/> instance.</returns>
        public static TFirewall<T> Configure(TRule<T> rule, string name)
        {
            TFirewall<T> firewall = new(rule);
            firewall.Name = name;
            firewall._rules[0].Load();

            Log.SendRaw($"[TFirewall:{name}] has been configured - Exit Result: Operative", ConsoleColor.DarkGreen);

            return firewall;
        }

        /// <summary>
        /// Configures a new <see cref="TFirewall{T}"/> instance.
        /// </summary>
        /// <param name="rules">The configuration.</param>
        /// <param name="name">The name to identify the firewall.</param>
        /// <returns>The configured <see cref="TFirewall{T}"/> instance.</returns>
        public static TFirewall<T> Configure(IEnumerable<TRule<T>> rules, string name)
        {
            TFirewall<T> firewall = new(rules);
            firewall.Name = name;
            foreach (TRule<T> rule in firewall._rules)
                rule.Load();

            Log.SendRaw($"[TFirewall:{name}] has been configured - Exit Result: Operative", ConsoleColor.DarkGreen);

            return firewall;
        }

        /// <summary>
        /// Configures a new <see cref="TFirewall{T}"/> instance.
        /// </summary>
        /// <param name="group">The configuration group.</param>
        /// <param name="name">The name to identify the firewall.</param>
        /// <returns>The configured <see cref="TFirewall{T}"/> instance.</returns>
        public static TFirewall<T> Configure(string group, string name)
        {
            List<TRule<T>> rules = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if ((type.BaseType != typeof(TRule<T>) && !type.IsSubclassOf(typeof(TRule<T>))) || type.GetCustomAttribute(typeof(RuleAttribute)) is null)
                    continue;

                RuleAttribute ruleAttribute = type.GetCustomAttribute<RuleAttribute>();
                if (ruleAttribute.Group != group)
                    continue;

                rules.Add(Activator.CreateInstance(type) as TRule<T>);
            }

            return Configure(rules, name);
        }

        /// <summary>
        /// Reconfigures the <see cref="TFirewall{T}"/> instance.
        /// </summary>
        /// <param name="firewall">The <see cref="TFirewall{T}"/> instance.</param>
        /// <param name="rule">The new rule.</param>
        /// <returns><see langword="true"/> if the <paramref name="firewall"/> was successfully reconfigured; otherwise, <see langword="false"/>.</returns>
        public static bool Reconfigure(ref TFirewall<T> firewall, TRule<T> rule)
        {
            if (firewall._rules.Length > 1)
                return false;

            firewall._rules[0]?.Unload();
            firewall._rules[0] = rule;
            firewall._rules[0].Load();

            Log.SendRaw($"[TFirewall:{firewall.Name}] has been reconfigured - Exit Result: Operative", ConsoleColor.DarkGreen);

            return true;
        }

        /// <summary>
        /// Reconfigures the <see cref="TFirewall{T}"/> instance.
        /// </summary>
        /// <param name="firewall">The <see cref="TFirewall{T}"/> instance.</param>
        /// <param name="index">The index of the rule to reconfigure.</param>
        /// <param name="rule">The new rule.</param>
        /// <returns><see langword="true"/> if the <paramref name="firewall"/> was successfully reconfigured; otherwise, <see langword="false"/>.</returns>
        public static bool Reconfigure(ref TFirewall<T> firewall, int index, TRule<T> rule)
        {
            if (firewall._rules.Length >= index)
            {
                Log.SendRaw($"[TFirewall:{firewall.Name}] unexpected error whilst reconfiguration - Exit Result: Inactive", ConsoleColor.Red);

                return false;
            }

            TRule<T> reconf = firewall._rules[index];
            if (reconf is not null)
            {
                firewall._rules[index]?.Unload();
                firewall._rules[index] = rule;
                firewall._rules[index].Load();

                Log.SendRaw($"[TFirewall:{firewall.Name}] has been reconfigured - Exit Result: Operative", ConsoleColor.Yellow);

                return true;
            }

            Log.SendRaw($"[TFirewall:{firewall.Name}] unexpected error whilst reconfiguration - Exit Result: Inactive", ConsoleColor.Red);

            return false;
        }

        /// <summary>
        /// Reconfigures the <see cref="TFirewall{T}"/> instance.
        /// </summary>
        /// <param name="firewall">The <see cref="TFirewall{T}"/> instance.</param>
        /// <param name="rules">The new rules.</param>
        /// <returns>The reconfigured <see cref="TFirewall{T}"/> instance.</returns>
        public static TFirewall<T> Reconfigure(ref TFirewall<T> firewall, IEnumerable<TRule<T>> rules)
        {
            firewall._rules = null;
            TRule<T>[] toLoad = rules.ToArray();
            firewall._rules = new TRule<T>[toLoad.Length];
            for (int i = 0; i < toLoad.Length; i++)
            {
                firewall._rules[i]?.Unload();
                firewall._rules[i] = toLoad[i];
                firewall._rules[i].Load();
            }

            Log.SendRaw($"[TFirewall:{firewall.Name}] has been reconfigured - Exit Result: Operative", ConsoleColor.DarkGreen);

            return firewall;
        }
    }
}
