// -----------------------------------------------------------------------
// <copyright file="AActor.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MEC;

    using RolePlus.ExternModule.API.Engine.Core;

    using UnityEngine;

    /// <summary>
    /// Actor is the base class for a <see cref="UObject"/> that can be placed or spawned in-game.
    /// </summary>
    public abstract class AActor : UObject
    {
        /// <summary>
        /// The default fixed tick rate.
        /// </summary>
        public const float DefaultFixedTickRate = 0.016f;

        private CoroutineHandle _serverTick;
        private bool _canEverTick;
        private float _fixedTickRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AActor"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="UObject.Base"/></param>
        protected AActor(GameObject gameObject = null)
            : base(gameObject)
        {
            IsEditable = true;
            CanEverTick = true;
            _fixedTickRate = DefaultFixedTickRate;
            Timing.CallDelayed(_fixedTickRate, () => OnBeginPlay());
            Timing.CallDelayed(_fixedTickRate * 2, () => _serverTick = Timing.RunCoroutine(ServerTick()));
        }

        /// <summary>
        /// Gets a <see cref="AActor"/>[] containing all the components in parent.
        /// </summary>
        protected internal AActor[] ComponentsInParent => FindActiveObjectsOfType<AActor>().Where(actor => actor.ComponentsInChildren.Any(comp => comp == this)).ToArray();

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="AActor"/> containing all the components in children.
        /// </summary>
        protected internal HashSet<AActor> ComponentsInChildren { get; } = new();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets or sets the <see cref="Vector3">position</see>.
        /// </summary>
        public virtual Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Quaternion">rotation</see>.
        /// </summary>
        public virtual Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">scale</see>.
        /// </summary>
        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set => Transform.localScale = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="AActor"/> can tick.
        /// </summary>
        public virtual bool CanEverTick
        {
            get => _canEverTick;
            set
            {
                if (!IsEditable)
                    return;

                _canEverTick = value;
            }
        }

        /// <summary>
        /// Gets or sets the value which determines the size of every tick.
        /// </summary>
        public virtual float FixedTickRate
        {
            get => _fixedTickRate;
            set
            {
                if (!IsEditable)
                    return;

                _fixedTickRate = value;
            }
        }

        /// <summary>
        /// Fired after the <see cref="AActor"/> instance is created.
        /// </summary>
        protected virtual void OnBeginPlay()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// Fired every tick.
        /// </summary>
        protected virtual void Tick()
        {
        }

        /// <summary>
        /// Fired before the current <see cref="AActor"/> instance is destroyed.
        /// </summary>
        protected virtual void OnEndPlay()
        {
            UnsubscribeEvents();
        }

        /// <summary>
        /// Subscribes all the events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes all the events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            Timing.KillCoroutines(_serverTick);
            OnEndPlay();
        }

        /// <summary>
        /// Attaches a <see cref="AActor"/> to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="comp"><see cref="AActor"/>.</param>
        /// <param name="gameObject"><see cref="GameObject"/>.</param>
        public static void AttachTo(AActor comp, GameObject gameObject) => comp.Base = gameObject;

        /// <summary>
        /// Attaches a <see cref="AActor"/> to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="comp">The <see cref="AActor"/> component.</param>
        /// <param name="actor">The target <see cref="AActor"/>.</param>
        public static void AttachTo(AActor comp, AActor actor) => comp.Base = actor.Base;

        /// <summary>
        /// Adds a component to this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="AActor"/> to be added.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The component which is being added.</returns>
        public T AddComponent<T>(string name = "")
            where T : AActor
        {
            T component = CreateDefaultSubobject<T>(Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (component is null)
                return null;

            ComponentsInChildren.Add(component);
            return component.Cast<T>();
        }

        /// <summary>
        /// Adds a component to this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="AActor"/> to be added.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The component which is being added.</returns>
        public AActor AddComponent(Type type, string name = "")
        {
            AActor component = CreateDefaultSubobject(type, Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<AActor>();
            if (component is null)
                return null;

            ComponentsInChildren.Add(component);
            return component;
        }

        /// <summary>
        /// Gets a component from this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="AActor"/> to look for.</typeparam>
        /// <returns>The component of type <typeparamref name="T"/>.</returns>
        public T GetComponent<T>()
            where T : AActor => ComponentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <summary>
        /// Gets a component from this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="AActor"/> to look for.</param>
        /// <returns>The component of type <paramref name="type"/>.</returns>
        public AActor GetComponent(Type type) => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <summary>
        /// Tries to get a component from this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="AActor"/> to look for.</typeparam>
        /// <param name="component">The <typeparamref name="T"/> <see cref="AActor"/>.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent<T>(out T component)
            where T : AActor
        {
            component = null;

            if (HasComponent<T>())
                component = GetComponent<T>().Cast<T>();

            return component is not null;
        }

        /// <summary>
        /// Tries to get a component from this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="AActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent(Type type, out AActor component)
        {
            component = null;

            if (HasComponent(type))
                component = GetComponent(type);

            return component is not null;
        }

        /// <summary>
        /// Checks if the actor has an active component.
        /// </summary>
        /// <typeparam name="T">The <see cref="AActor"/> to look for.</typeparam>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool HasComponent<T>()
            where T : AActor => ComponentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <summary>
        /// Checks if the actor has an active component.
        /// </summary>
        /// <param name="type">The <see cref="AActor"/> to look for.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool HasComponent(Type type) => ComponentsInChildren.Any(comp => type == comp.GetType());

        private IEnumerator<float> ServerTick()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(FixedTickRate);
                if (!CanEverTick)
                    continue;

                Tick();
            }
        }
    }
}
