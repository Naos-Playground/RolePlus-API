// -----------------------------------------------------------------------
// <copyright file="CommonPatchProcessor.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    using RolePlus.ExternModule.Events.EventArgs;

    /// <inheritdoc/>
    public class CommonPatchProcessor : GlobalPatchProcessor
    {
        private static readonly Dictionary<EventInfo, Delegate> _handlers = new();

        /// <summary>
        /// Begins the delivery of all existing <see cref="Exiled.Events.Handlers"/>.
        /// </summary>
        public static void BeginEventHandlersDelivery()
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

        /// <summary>
        /// Stops the delivery of all currently delivered <see cref="Exiled.Events.Handlers"/>.
        /// </summary>
        public static void StopEventHandlersDelivery()
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

        /// <summary>
        /// Delivers the specified <typeparamref name="T"/> event to the invokation handler.
        /// </summary>
        /// <typeparam name="T">The type of the handler.</typeparam>
        /// <param name="ev">The event handler instance.</param>
        public static void MessageHandler<T>(T ev)
            where T : EventArgs
        {
            InvokingHandlerEventArgs toInvoke = new(ev);
            ExternModule.Events.Handlers.Server.InvokingHandlerDispatcher.InvokeAll(toInvoke);
        }
    }
}