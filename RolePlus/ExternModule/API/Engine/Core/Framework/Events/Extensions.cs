// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events
{
    using System;
    using System.Linq;

    using Exiled.API.Features;

    using static RolePlus.ExternModule.API.Engine.Framework.Events.Delegates;

#pragma warning disable SA1649 // File name should match first type name

    internal static class EventExtensions
    {
        internal static void InvokeSafely<T>(this TEventHandler<T> ev, T arg)
            where T : System.EventArgs
        {
            if (ev == null)
                return;

            string eventName = ev.GetType().FullName;
            foreach (TEventHandler<T> handler in ev.GetInvocationList().Cast<TEventHandler<T>>())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    LogException(ex, handler.Method.Name, handler.Method.ReflectedType.FullName, eventName);
                }
            }
        }

        internal static void InvokeSafely(this TEventHandler ev)
        {
            if (ev == null)
                return;

            string eventName = ev.GetType().FullName;
            foreach (TEventHandler handler in ev.GetInvocationList().Cast<TEventHandler>())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    LogException(ex, handler.Method.Name, handler.Method.ReflectedType?.FullName, eventName);
                }
            }
        }

        private static void LogException(Exception ex, string methodName, string sourceClassName, string eventName)
        {
            Log.Error($"Method \"{methodName}\" of the class \"{sourceClassName}\" caused an exception when handling the event \"{eventName}\"");
            Log.Error(ex);
        }
    }
}
