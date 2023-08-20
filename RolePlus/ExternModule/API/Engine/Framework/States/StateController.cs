// -----------------------------------------------------------------------
// <copyright file="StateController.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.States
{
    using System.Collections.Generic;
    using Exiled.API.Features.Core;
    using RolePlus.ExternModule.Events.DynamicEvents;

    /// <summary>
    /// The base controller which handles actors using in-context states.
    /// </summary>
    public abstract class StateController : EActor
    {
        private readonly List<State> _states = new();
        private State _currentState;

        /// <summary>
        /// Gets all handled states.
        /// </summary>
        public IEnumerable<State> States => _states;

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public State CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState.Id == value.Id)
                    return;

                (PreviousState = _currentState).OnExit(this);
                (_currentState = value).OnEnter(this);

                OnStateChanged();
            }
        }

        /// <summary>
        /// Gets or sets the previous state.
        /// </summary>
        public State PreviousState { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when entering a new state.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<State> BeginStateMulticastDispatcher { get; protected set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when exiting the current state.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<State> EndStateMulticastDispatcher { get; protected set; } = new();

        /// <summary>
        /// Fired when the state is changed.
        /// </summary>
        protected virtual void OnStateChanged()
        {
            EndStateMulticastDispatcher.InvokeAll(PreviousState);
            BeginStateMulticastDispatcher.InvokeAll(_currentState);
        }

        /// <summary>
        /// Fired every tick from the current state.
        /// </summary>
        /// <param name="state">The state firing the update.</param>
        public virtual void StateUpdate(State state)
        {
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            ExternModule.Events.EventManager.CreateFromTypeInstance(this);
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            ExternModule.Events.EventManager.UnbindAllFromTypeInstance(this);
        }
    }
}
