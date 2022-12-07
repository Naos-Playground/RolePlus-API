// -----------------------------------------------------------------------
// <copyright file="AActorFrameComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using RolePlus.ExternModule.API.Engine.Framework;

    /// <summary>
    /// <see cref="AActorFrameComponent"/> is the base class for <see cref="AActor"/> instances which need to be managed internally.
    /// </summary>
    public abstract class AActorFrameComponent : AActor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AActorFrameComponent"/> class.
        /// </summary>
        public AActorFrameComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AActorFrameComponent"/> class.
        /// </summary>
        /// <param name="root"><inheritdoc cref="RootComponent"/></param>
        protected AActorFrameComponent(AActor root)
            : this() => RootComponent = root;

        /// <summary>
        /// Gets or sets the root <see cref="AActor"/>.
        /// </summary>
        public abstract AActor RootComponent { get; protected set; }
    }
}
