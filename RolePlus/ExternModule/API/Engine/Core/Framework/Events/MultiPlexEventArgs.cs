// -----------------------------------------------------------------------
// <copyright file="MultiPlexEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events
{
#pragma warning disable SA1618 // Generic type parameters should be documented

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public class MultiPlexEventArgs<T1, T2> : PlexEventArgs<T1>
    {
        /// <summary>
        /// The <see cref="System.EventHandler"/> delegate.
        /// </summary>
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
#pragma warning disable SA1611 // Element parameters should be documented
        public delegate void TEventHandler<T1, T2, TEventArgs>(TEventArgs ev)
#pragma warning restore SA1611 // Element parameters should be documented
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
            where TEventArgs : MultiPlexEventArgs<T1, T2>;
    }
}
