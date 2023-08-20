// -----------------------------------------------------------------------
// <copyright file="IAddittivePipe.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Interfaces
{
    /// <summary>
    /// Defines an addittive user-defined pipe.
    /// </summary>
    public interface IAddittivePipe : IAddittiveIdentifier
    {
        /// <summary>
        /// Addittive property should be adjusted here.
        /// </summary>
        public abstract void AdjustAddittiveProperty();
    }
}
