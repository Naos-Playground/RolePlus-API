// -----------------------------------------------------------------------
// <copyright file="UObject.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Events.Handlers
{
    using RolePlus.ExternModule.API.Engine.Components;
    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;

    using static RolePlus.ExternModule.API.Engine.Framework.Events.Delegates;

    /// <summary>
    /// Handles all the network events.
    /// </summary>
    public static class UObject
    {
        /// <summary>
        /// Fired before a <see cref="ASchematicMeshComponent"/> changes animation state.
        /// </summary>
        public static event TEventHandler<ChangingAnimationStateEventArgs> ChangingAnimationState;

        /// <summary>
        /// Fired after a <see cref="ASchematicMeshComponent"/> completes an animation.
        /// </summary>
        public static event TEventHandler<CompletedAnimationStateEventArgs> CompletedAnimationState;

        /// <summary>
        /// Called before a <see cref="ASchematicMeshComponent"/> changes animation state.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAnimationStateEventArgs"/> instance.</param>
        public static void OnChangingAnimationState(ChangingAnimationStateEventArgs ev) => ChangingAnimationState.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="ASchematicMeshComponent"/> completes an animation.
        /// </summary>
        /// <param name="ev">The <see cref="CompletedAnimationStateEventArgs"/> instance.</param>
        public static void OnCompletedAnimationState(CompletedAnimationStateEventArgs ev) => CompletedAnimationState.InvokeSafely(ev);
    }
}
