// -----------------------------------------------------------------------
// <copyright file="IAnimatorNative.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Interfaces
{
    using System.Collections.Generic;

    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;

    using UnityEngine;

    /// <summary>
    /// Basic implementation to handle animations inside components.
    /// </summary>
    public interface IAnimatorNative
    {
        /// <summary>
        /// Gets a <see cref="string"/>[] containing all the triggers for the paired <see cref="Animator"/>.
        /// </summary>
        IReadOnlyDictionary<Animator, string[]> AnimatorsTriggers { get; }

        /// <summary>
        /// Plays an animation given a parameter name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="state">The state of the parameter.</param>
        /// <param name="animatorIndex">The index of the animator.</param>
        /// <returns><see langword="true"/> if the animation was played successfully; otherwise, <see langword="false"/>.</returns>
        public bool PlayAnimation(string name, bool state, int animatorIndex = 0);

        /// <summary>
        /// Plays an animation given a <see cref="AnimatorParameter"/>.
        /// </summary>
        /// <param name="anim">The <see cref="AnimatorParameter"/>.</param>
        /// <param name="animatorIndex">The index of the animator.</param>
        /// <returns><see langword="true"/> if the animation was played successfully; otherwise, <see langword="false"/>.</returns>
        public bool PlayAnimation(AnimatorParameter anim, int animatorIndex = 0);

        /// <summary>
        /// Stops an animation from the <see cref="Animator"/> located in the specified index.
        /// </summary>
        /// <param name="animatorIndex">The index of the animator.</param>
        public void StopAnimation(int animatorIndex = 0);

        /// <summary>
        /// Gets the animations of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to check.</param>
        /// <returns>The animations.</returns>
        public AnimatorParameter[] GetAnimations(Animator animator);

        /// <summary>
        /// Gets the current playing animation of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to check.</param>
        /// <returns>The current playing animation.</returns>
        public AnimatorStateInfo GetCurrentAnimation(Animator animator);

        /// <summary>
        /// Gets the current playing animation of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to check.</param>
        /// <param name="anim">The <see cref="AnimatorParameter"/> to get.</param>
        /// <returns>The current playing animation.</returns>
        public ref AnimatorParameter GetCurrentAnimation(Animator animator, AnimatorParameter anim);

        /// <summary>
        /// Gets the current playing animation of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to check.</param>
        /// <param name="name">The name of the animation state to get.</param>
        /// <returns>The current playing animation.</returns>
        public ref AnimatorParameter GetCurrentAnimation(Animator animator, string name);

        /// <summary>
        /// Sets the current playing animation of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to modify.</param>
        /// <param name="anim">The <see cref="AnimatorParameter"/> to set.</param>
        public void SetCurrentAnimation(Animator animator, AnimatorParameter anim);

        /// <summary>
        /// Sets the current playing animation of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to modify.</param>
        /// <param name="name">The name of the animation state to set.</param>
        /// <param name="state">The state of the animation state to set.</param>
        public void SetCurrentAnimation(Animator animator, string name, bool state);

        /// <inheritdoc cref="Events.Handlers.UObject.OnChangingAnimationState(ChangingAnimationStateEventArgs)"/>
        public void OnChangingAnimationState(ChangingAnimationStateEventArgs ev);
    }
}
