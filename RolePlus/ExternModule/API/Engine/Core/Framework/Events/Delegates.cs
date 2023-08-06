// -----------------------------------------------------------------------
// <copyright file="Delegates.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events
{
    /// <summary>
    /// A class containing all existing delegates.
    /// </summary>
    public sealed class Delegates
    {
        /// <summary>
        /// The <see cref="System.EventHandler"/> delegate.
        /// </summary>
        public delegate void TEventHandler<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;

        /// <summary>
        /// The custom <see cref="System.EventHandler"/> delegate with empty parameters.
        /// </summary>
        public delegate void TEventHandler();
    }
}
