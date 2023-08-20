// -----------------------------------------------------------------------
// <copyright file="TDynamicEventDispatcher.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.DynamicEvents
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features.Core;

    /// <summary>
    /// The <see cref="DynamicEventDispatcher"/>'s generic version which accepts a type parameter.
    /// </summary>
    /// <typeparam name="T">The event type parameter.</typeparam>
    public class TDynamicEventDispatcher<T> : TypeCastObject<DynamicEventDispatcher>, IDynamicEventDispatcher
    {
        private readonly Dictionary<object, List<Action<T>>> _boundDelegates = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TDynamicEventDispatcher{T}"/> class.
        /// </summary>
        public TDynamicEventDispatcher()
        {
        }

        /// <summary>
        /// Gets all the bound delegates.
        /// </summary>
        public IReadOnlyDictionary<object, List<Action<T>>> BoundDelegates => _boundDelegates;

        /// <summary>
        /// Binds a listener to the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="del">The delegate to be bound.</param>
        public virtual void Bind(object obj, Action<T> del)
        {
            if (!_boundDelegates.ContainsKey(obj))
                _boundDelegates.Add(obj, new List<Action<T>>() { del });
            else
                _boundDelegates[obj].Add(del);
        }

        /// <summary>
        /// Unbinds a listener from the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        public virtual void Unbind(object obj) => _boundDelegates.Remove(obj);

        /// <summary>
        /// Invokes the delegates from the specified listener.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="instance">The .</param>
        public virtual void Invoke(object obj, T instance)
        {
            if (_boundDelegates.TryGetValue(obj, out List<Action<T>> delegates))
                delegates.ForEach(del => del(instance));
        }

        /// <summary>
        /// Invokes all the delegates from all the bound delegates.
        /// </summary>
        /// <param name="instance">The parameter instance.</param>
        public virtual void InvokeAll(T instance)
        {
            foreach (KeyValuePair<object, List<Action<T>>> kvp in _boundDelegates)
                kvp.Value.ForEach(del => del(instance));
        }

        /// <inheritdoc/>
        public virtual void UnbindAll() => _boundDelegates.Clear();
    }
}