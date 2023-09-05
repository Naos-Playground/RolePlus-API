// -----------------------------------------------------------------------
// <copyright file="Branch.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.VirtualAssemblies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.Configs;

    /// <summary>
    /// A tool to manage virtual assemblies.
    /// </summary>
    public abstract class Branch
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="Branch"/>'s.
        /// </summary>
        public static List<Branch> Registered { get; private set; } = new();

        private BranchAttribute BranchProject => GetType().GetCustomAttributes(typeof(BranchAttribute), true).FirstOrDefault() as BranchAttribute;

        /// <summary>
        /// Gets the master branch's name.
        /// </summary>
        public string Master => BranchProject.Master;

        /// <summary>
        /// Gets the branch's name.
        /// </summary>
        public string Name => BranchProject.Name;

        /// <summary>
        /// Gets the branch's prefix.
        /// </summary>
        public string Prefix => BranchProject.Prefix;

        /// <summary>
        /// Gets or sets the <see cref="IConfig"/> object.
        /// </summary>
        public Config Config { get; set; }

        /// <summary>
        /// Gets the branch's <see cref="System.Version"/>.
        /// </summary>
        public abstract Version Version { get; }

        /// <summary>
        /// Gets the <see cref="BranchTypeBase"/>.
        /// </summary>
        public abstract BranchTypeBase BranchType { get; }

        /// <summary>
        /// Gets a value indicating whether the branch is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Enables all the branches present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Branch"/> which contains all the enabled branches.</returns>
        public static IEnumerable<Branch> RegisterBranches()
        {
            List<Branch> branches = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType != typeof(Branch) || type.GetCustomAttribute(typeof(BranchAttribute)) is null)
                    continue;

                Branch branch = Activator.CreateInstance(type) as Branch;

                branch.TryRegister();
                branches.Add(branch);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {branches.Count()} branches have been successfully registered!", ConsoleColor.Cyan);

            return branches;
        }

        /// <summary>
        /// Disables all the branches present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Branch"/> which contains all the disabled branches.</returns>
        public static IEnumerable<Branch> UnregisterBranches()
        {
            List<Branch> branches = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType != typeof(Branch) || type.GetCustomAttribute(typeof(BranchAttribute)) is null)
                    continue;

                Branch branch = Activator.CreateInstance(type) as Branch;

                branch.TryUnregister();
                branches.Remove(branch);
            }

            Log.SendRaw($"[{Assembly.GetCallingAssembly().GetName().Name}] {branches.Count()} branches have been successfully unregistered!", ConsoleColor.Cyan);

            return branches;
        }

        /// <summary>
        /// Reloads all the branches present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Branch"/> which contains all the reloaded branches.</returns>
        public static IEnumerable<Branch> ReloadBranches()
        {
            List<Branch> branches = new();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType != typeof(Branch))
                    continue;

                Branch branch = Activator.CreateInstance(type) as Branch;

                branch.OnReloaded();
                branches.Add(branch);
            }

            return branches;
        }

        /// <summary>
        /// Tries to register a <see cref="Branch"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="Branch"/> was registered; otherwise, <see langword="false"/>.</returns>
        public bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(x => x.Name == Name))
                {
                    Log.SendRaw(
                        $"[VirtualAssembly] Couldn't register {Name}. " +
                        "Another Branch has been registered with the same name:" +
                        $" {Registered.FirstOrDefault(x => x.Name == Name)}",
                        ConsoleColor.Red);

                    return false;
                }

                OnEnabled_Internal();

                return true;
            }

            Log.SendRaw($"[VirtualAssembly] Couldn't register {Name}. This Branch has been already registered.", ConsoleColor.Red);

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="Branch"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="Branch"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        public bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.SendRaw($"[VirtualAssembly] Couldn't unregister {Name}. This Branch hasn't been registered yet.", ConsoleColor.Red);

                return false;
            }

            OnDisabled();

            return true;
        }

        /// <summary>
        /// Fired after enabling the branch.
        /// </summary>
        protected virtual void OnEnabled()
        {
            SubscribeEvents();
            Log.SendRaw($"[VirtualAssembly] [{Prefix}] {Name} v{Version} has been enabled!", ConsoleColor.Magenta);
            Log.SendRaw($"[VirtualAssembly] [{Prefix}] {Name} Branch Type: {BranchType} - Master: {Master}", ConsoleColor.Magenta);
            IsRunning = true;
        }

        /// <summary>
        /// Fired after disabling the branch.
        /// </summary>
        protected virtual void OnDisabled()
        {
            DestroyInstance();
            Registered.Remove(this);
            UnsubscribeEvents();
            IsRunning = false;
        }

        /// <summary>
        /// Fired after reloading the branch.
        /// </summary>
        protected virtual void OnReloaded()
        {
            DestroyInstance();

            try
            {
                UnsubscribeEvents();
            }
            catch
            {
            }

            IsRunning = false;

            OnEnabled_Internal();
        }

        /// <summary>
        /// Fired before the branch is enabled.
        /// </summary>
        protected virtual void CreateInstance()
        {
            Singleton<Branch>.Create(this);
        }

        /// <summary>
        /// Fired before the branch is disabled.
        /// </summary>
        protected virtual void DestroyInstance()
        {
            Singleton<Branch>.Destroy(this);
            Config = null;
        }

        /// <summary>
        /// Fired after enabling the branch.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Fired after disabling the branch.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        private void OnEnabled_Internal()
        {
            CreateInstance();

            if (!Registered.Contains(this))
                Registered.Add(this);

            if (!Config.Cast<IConfig>().IsEnabled)
            {
                Log.SendRaw($"[VirtualAssembly] [{Prefix}] {Name} is not enabled.", ConsoleColor.DarkMagenta);
                return;
            }

            OnEnabled();
        }
    }
}
