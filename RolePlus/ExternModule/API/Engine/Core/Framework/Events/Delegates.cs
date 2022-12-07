// -----------------------------------------------------------------------
// <copyright file="Delegates.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events
{
#pragma warning disable SA1618 // Generic type parameters should be documented
#pragma warning disable SA1611 // Element parameters should be documented

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public sealed class Delegates
    {
        /// <summary>
        /// The <see cref="System.EventHandler"/> delegate.
        /// </summary>
        public delegate void TEventHandler<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;

        /// <summary>
        /// The <see cref="System.EventHandler"/> delegate.
        /// </summary>
        public delegate void TEventHandler<T1, TEventArgs>(TEventArgs ev)
            where TEventArgs : PlexEventArgs<T1>;

        /// <summary>
        /// The <see cref="System.EventHandler"/> delegate.
        /// </summary>
        public delegate void TEventHandler<T1, T2, TEventArgs>(TEventArgs ev)
            where TEventArgs : MultiPlexEventArgs<T1, T2>;

        /// <summary>
        /// The custom <see cref="System.EventHandler"/> delegate with empty parameters.
        /// </summary>
        public delegate void TEventHandler();
    }
}
