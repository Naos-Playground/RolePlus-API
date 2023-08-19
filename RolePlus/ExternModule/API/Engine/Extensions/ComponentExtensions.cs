// -----------------------------------------------------------------------
// <copyright file="ComponentExtensions.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    using UnityEngine;

    /// <summary>
    /// A set of useful extensions to easily interact with <see cref="EActor"/>.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Adds a <typeparamref name="T"/> <see cref="EActor"/> to the given <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="player">The <see cref="Player"/> who should own the <see cref="EActor"/>.</param>
        /// <param name="name">The name of the <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns>The added <typeparamref name="T"/> <see cref="EActor"/> instance.</returns>
        public static T AddActorComponent<T>(this Player player, string name = "")
            where T : EActor
        {
            T outer = EObject.CreateDefaultSubobject<T>(player.GameObject, name).Cast<EActor>().Cast<T>();
            EActor.AttachTo(outer, player.GameObject);
            return outer;
        }

        /// <summary>
        /// Adds a <typeparamref name="T"/> <see cref="EActor"/> to the given <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> who should own the <see cref="EActor"/>.</param>
        /// <param name="name">The name of the <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns>The added <typeparamref name="T"/> <see cref="EActor"/> instance.</returns>
        public static T AddActorComponent<T>(this GameObject gameObject, string name = "")
            where T : EActor
        {
            T outer = EObject.CreateDefaultSubobject<T>(gameObject, name).Cast<EActor>().Cast<T>();
            EActor.AttachTo(outer, gameObject);
            return outer;
        }

        /// <summary>
        /// Adds a <typeparamref name="T"/> <see cref="EActor"/> to the given <see cref="EActor"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="actor">The <see cref="EActor"/> who should own the <see cref="EActor"/>.</param>
        /// <param name="name">The name of the <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns>The added <typeparamref name="T"/> <see cref="EActor"/> instance.</returns>
        public static T AddActorComponent<T>(this EActor actor, string name = "")
            where T : EActor
        {
            T outer = EObject.CreateDefaultSubobject<T>(actor.Base, name).Cast<EActor>().Cast<T>();
            EActor.AttachTo(outer, actor.Base);
            return outer;
        }

        /// <summary>
        /// Gets a <typeparamref name="T"/> <see cref="EActor"/> from the given <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="player">The <see cref="Player"/> who owns the <see cref="EActor"/>.</param>
        /// <returns>The <typeparamref name="T"/> <see cref="EActor"/> instance.</returns>
        public static T GetActorComponent<T>(this Player player)
            where T : EActor => player.GameObject.GetActorComponent<T>();

        /// <summary>
        /// Gets a <typeparamref name="T"/> <see cref="EActor"/> from the given <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> who owns the <see cref="EActor"/>.</param>
        /// <returns>The <typeparamref name="T"/> <see cref="EActor"/> instance.</returns>
        public static T GetActorComponent<T>(this GameObject gameObject)
            where T : EActor =>
            EObject.FindActiveObjectsOfType<T>().FirstOrDefault(comp => comp.Cast(out T _) && comp.Base == gameObject);

        /// <summary>
        /// Gets a <typeparamref name="T"/> <see cref="EActor"/> from the given <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="player">The <see cref="Player"/> who owns the <see cref="EActor"/>.</param>
        /// <param name="component">The <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns><see langword="true"/> if the <typeparamref name="T"/> <see cref="EActor"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetActorComponent<T>(this Player player, out T component)
            where T : EActor
        {
            component = null;

            if (player.GameObject.TryGetActorComponent(out T param))
                component = param;

            return component is not null;
        }

        /// <summary>
        /// Gets a <typeparamref name="T"/> <see cref="EActor"/> from the given <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> who owns the <see cref="EActor"/>.</param>
        /// <param name="component">The <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns><see langword="true"/> if the <typeparamref name="T"/> <see cref="EActor"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetActorComponent<T>(this GameObject gameObject, out T component)
            where T : EActor
        {
            component = null;

            foreach (EActor comp in EObject.FindActiveObjectsOfType<EActor>())
            {
                if (comp.Base != gameObject || !comp.Cast(out T outer))
                    continue;

                component = outer;
            }

            return component is not null;
        }

        /// <summary>
        /// Gets all active <see cref="EActor"/>s from the given <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who owns the active <see cref="EActor"/>s.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="EActor"/> contaning all the active <see cref="EActor"/>s.</returns>
        public static IEnumerable<EActor> GetActiveActorComponentsAll(this Player player) => player.GameObject.GetActiveComponentsAll();

        /// <summary>
        /// Gets all active <see cref="EActor"/>s from the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> who owns the active <see cref="EActor"/>s.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="EActor"/> contaning all the active <see cref="EActor"/>s.</returns>
        public static IEnumerable<EActor> GetActiveComponentsAll(this GameObject gameObject)
        {
            foreach (EActor component in EObject.FindActiveObjectsOfType<EActor>())
            {
                if (component.Base != gameObject)
                    continue;

                yield return component;
            }
        }

        /// <summary>
        /// Checks if the given <see cref="Player"/> has an active <typeparamref name="T"/> <see cref="EActor"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="EActor"/> to look for.</typeparam>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the <typeparamref name="T"/> <see cref="EActor"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool HasActorComponent<T>(this Player player)
            where T : EActor => player.GameObject.HasActorComponent<T>();

        /// <summary>
        /// Checks if the given <see cref="GameObject"/> has an active <typeparamref name="T"/> <see cref="EActor"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="EActor"/> to look for.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> to check.</param>
        /// <returns><see langword="true"/> if the <typeparamref name="T"/> <see cref="EActor"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool HasActorComponent<T>(this GameObject gameObject)
            where T : EActor =>
            EObject.FindActiveObjectsOfType<T>().Any(component =>
            component.GetType() == typeof(T) && component.Base == gameObject);
    }
}
