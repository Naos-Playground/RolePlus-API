// -----------------------------------------------------------------------
// <copyright file="GlobalPatchProcessor.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;
    using HarmonyLib;
    using RolePlus.ExternModule.Events.EventArgs;
#pragma warning disable format

    /// <summary>
    /// A <see cref="Harmony"/> alternative detour tool which adds more ways to manage patches and external assemblies.
    /// </summary>
    public class GlobalPatchProcessor
    {
        private static readonly Dictionary<MethodBase, HashSet<string>> _patchedGroupMethods = new();
        private static readonly Dictionary<EventInfo, Delegate> _handlers = new();

        /// <summary>
        /// Gets all the patched methods.
        /// </summary>
        public static IEnumerable<MethodBase> PatchedMethods => Harmony.GetAllPatchedMethods();

        /// <summary>
        /// Gets all the patched methods and their relative patch group.
        /// </summary>
        public static IReadOnlyDictionary<MethodBase, HashSet<string>> PatchedGroupMethods => _patchedGroupMethods;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
        public static void AddEventHandlers()
        {
            IPlugin<IConfig> eventsAssembly = Loader.Plugins.FirstOrDefault(x => x.Name == "Exiled.Events");

            if (eventsAssembly == null)
            {
                Log.Warn("Exiled.Events not found. Skipping AddEventHandlers.");
                return;
            }

            foreach (EventInfo eventInfo in
                from Type eventClass in eventsAssembly.Assembly.GetTypes().Where(x => x.Namespace == "Exiled.Events.Handlers")
                from EventInfo eventInfo in eventClass.GetEvents()
                select eventInfo)
            {
                if (!eventInfo.EventHandlerType.GenericTypeArguments.Any())
                    continue;

                MethodInfo mi = typeof(GlobalPatchProcessor).GetMethod(nameof(MessageHandler), BindingFlags.NonPublic | BindingFlags.Static);
                if (mi is null)
                    continue;

                Delegate del = Delegate.CreateDelegate(eventInfo.EventHandlerType, mi.MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments));
                eventInfo.AddEventHandler(null, del);
            }
        }

        public static void RemoveEventHandlers()
        {
            foreach (EventInfo eventInfo in
                from Type eventClass in Exiled.Events.Events.Instance.Assembly.GetTypes().Where(x => x.Namespace == "Exiled.Events.Handlers")
                from EventInfo eventInfo in eventClass.GetEvents()
                select eventInfo)
            {
                if (!_handlers.ContainsKey(eventInfo))
                    continue;

                eventInfo.RemoveEventHandler(null, _handlers[eventInfo]);
                _handlers.Remove(eventInfo);
            }
        }

        public static void MessageHandler<T>(T ev)
            where T : EventArgs
        {
            InvokingHandlerEventArgs toInvoke = new(ev);
            ExternModule.Events.Handlers.Server.OnInvokingHandler(toInvoke);
        }
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Searches the current assembly for Harmony annotations and uses them to create patches.
        /// <br>It supports target-patching using <see cref="PatchGroupAttribute"/> and the relative <paramref name="groupId"/>.</br>
        /// </summary>
        /// <param name="id">The Harmony instance id.</param>
        /// <param name="groupId">The target group to include.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="PatchGroupAttribute.GroupId"/> is <see langword="null"/> or empty.</exception>
        /// <returns>The <see cref="Harmony"/> instance.</returns>
        public static Harmony PatchAll(string id = "", string groupId = null)
        {
            try
            {
                Harmony harmony = new(id);
                bool isPatchGroup = false;
                foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
                {
                    PatchClassProcessor processor = harmony.CreateClassProcessor(type);
                    PatchGroupAttribute patchGroup = type.GetCustomAttribute<PatchGroupAttribute>();

                    if (patchGroup is null)
                    {
                        processor.Patch();
                        continue;
                    }

                    if (string.IsNullOrEmpty(patchGroup.GroupId))
                        throw new ArgumentNullException("GroupId");

                    if (string.IsNullOrEmpty(groupId) || patchGroup.GroupId != groupId)
                        continue;

                    isPatchGroup = true;
                    processor.Patch();
                }

                if (!isPatchGroup)
                    return harmony;

                foreach (MethodBase methodBase in harmony.GetPatchedMethods())
                {
                    if (_patchedGroupMethods.TryGetValue(methodBase, out HashSet<string> ids))
                        ids.Add(groupId);
                    else
                        _patchedGroupMethods.Add(methodBase, new() { groupId });

                    Log.Debug($"Target method ({methodBase.Name}) has been successfully patched.", Internal.RolePlus.Singleton.Config.ShowDebugMessages);
                }

                MethodBase callee = new StackTrace().GetFrame(1).GetMethod();
                Log.Debug($"Patching completed. Requested by: ({callee.DeclaringType.Name}::{callee.Name})", Internal.RolePlus.Singleton.Config.ShowDebugMessages);
                return harmony;
            }
            catch (Exception ex)
            {
                MethodBase callee = new StackTrace().GetFrame(1).GetMethod();
                Log.Error($"Callee ({callee.DeclaringType.Name}::{callee.Name}) Patching failed!, " + ex);
            }

            return null;
        }

        /// <summary>
        /// Unpatches methods by patching them with zero patches.
        /// </summary>
        /// <param name="id">The Harmony instance id.</param>
        /// <param name="groupId">The target group to include.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="PatchGroupAttribute.GroupId"/> is <see langword="null"/> or empty.</exception>
        public static void UnpatchAll(string id = "", string groupId = null)
        {
            Harmony harmony = new(id);
            foreach (MethodBase methodBase in Harmony.GetAllPatchedMethods().ToList())
            {
                PatchProcessor processor = harmony.CreateProcessor(methodBase);

                Patches patchInfo = Harmony.GetPatchInfo(methodBase);
                if (!patchInfo.Owners.Contains(id))
                    continue;

                PatchGroupAttribute patchGroup = methodBase.GetCustomAttribute<PatchGroupAttribute>();
                if (patchGroup is null)
                    goto Unpatch;

                if (string.IsNullOrEmpty(patchGroup.GroupId))
                    throw new ArgumentNullException("GroupId");

                if (string.IsNullOrEmpty(groupId) || patchGroup.GroupId != groupId)
                    continue;

                Unpatch:
                bool hasMethodBody = methodBase.HasMethodBody();
                if (hasMethodBody)
                {
                    patchInfo.Postfixes.Do(delegate(Patch patchInfo)
                    {
                        harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                    });
                    patchInfo.Prefixes.Do(delegate(Patch patchInfo)
                    {
                        harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                    });
                }

                patchInfo.Transpilers.Do(delegate(Patch patchInfo)
                {
                    harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                });

                if (hasMethodBody)
                {
                    patchInfo.Finalizers.Do(delegate(Patch patchInfo)
                    {
                        harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                    });
                }

                _patchedGroupMethods.Remove(methodBase);
            }
        }
    }
}
