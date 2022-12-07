// -----------------------------------------------------------------------
// <copyright file="ObjectController.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Controllers
{
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// A controller to be used with any type of components.
    /// </summary>
    public abstract class ObjectController : MonoBehaviour
    {
        /// <inheritdoc/>
        protected virtual void Awake() => SubscribeEvents();

        /// <inheritdoc/>
        protected virtual void Start()
        {
        }

        /// <inheritdoc/>
        protected virtual void FixedUpdate()
        {
        }

        /// <summary>
        /// Partially destroys the component.
        /// </summary>
        protected virtual void PartiallyDestroy() => UnsubscribeEvents();

        /// <summary>
        /// Destroys the component.
        /// </summary>
        public virtual void Destroy()
        {
            try
            {
                Destroy(this);
            }
            catch (Exception e)
            {
                Log.Error($"Exception: {e}\n Couldn't destroy: {this}\n");
            }
        }

        private void OnDestroy() => PartiallyDestroy();

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes the events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }
    }
}
