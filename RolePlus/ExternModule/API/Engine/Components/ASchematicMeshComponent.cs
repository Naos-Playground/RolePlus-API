// -----------------------------------------------------------------------
// <copyright file="ASchematicMeshComponent.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features.Core;

    using Exiled.API.Features.Pickups;
    using InventorySystem.Items.Pickups;

    using MapEditorReborn.API.Features.Objects;

    using MEC;

    using Mirror;
    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Engine.Framework.Events.EventArgs;
    using RolePlus.ExternModule.API.Engine.Framework.Interfaces;

    using UnityEngine;

    /// <summary>
    /// The base class for custom meshes.
    /// </summary>
    public class ASchematicMeshComponent : ASkeletalMeshComponent, IAnimatorNative
    {
        private readonly Dictionary<Animator, AnimatorParameter[]> _animatorsParams = new();
        private SchematicObject _rootSchematic;
        private bool _isVisible = true;
        private bool _isCollidable = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        protected ASchematicMeshComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="EObject.Base"/></param>
        protected ASchematicMeshComponent(GameObject gameObject = null)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="EObject.Base"/></param>
        /// <param name="meshName">The name of the mash.</param>
        protected ASchematicMeshComponent(GameObject gameObject, string meshName)
            : this(gameObject) => MeshName = meshName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="mesh"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASchematicMeshComponent(
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(mesh.gameObject, scale, position, rotation) =>
            RootSchematic ??= mesh;

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="meshName"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASchematicMeshComponent(
            string meshName,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : base(null, scale, position, rotation)
        {
            RootSchematic ??= MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(meshName, position);
            Base ??= RootSchematic.gameObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="socket"><inheritdoc cref="Socket"/></param>
        /// <param name="mesh"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASchematicMeshComponent(
            FTransform socket,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : this(mesh, scale, position, rotation) =>
            Socket = socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="localPosition"><inheritdoc cref="Socket"/></param>
        /// <param name="mesh"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASchematicMeshComponent(
            Vector3 localPosition,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : this(mesh, scale, position, rotation) =>
            Socket = new(localPosition, default, default);

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="localPosition"><inheritdoc cref="FTransform.Translation"/></param>
        /// <param name="localRotation"><inheritdoc cref="FTransform.Rotation"/></param>
        /// <param name="mesh"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASchematicMeshComponent(
            Vector3 localPosition,
            Quaternion localRotation,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : this(localPosition, mesh, scale, position, rotation) =>
            Socket = new(localPosition, localRotation, default);

        /// <summary>
        /// Initializes a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="localPosition"><inheritdoc cref="FTransform.Translation"/></param>
        /// <param name="localRotation"><inheritdoc cref="FTransform.Rotation"/></param>
        /// <param name="localScale"><inheritdoc cref="FTransform.Scale"/></param>
        /// <param name="mesh"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        protected ASchematicMeshComponent(
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation)
            : this(mesh, scale, position, rotation) =>
            Socket = new(localPosition, localRotation, localScale);

        /// <summary>
        /// Gets the animators.
        /// </summary>
        public IReadOnlyDictionary<Animator, string[]> AnimatorsTriggers { get; }

        /// <summary>
        /// Gets or sets the mesh asset.
        /// </summary>
        public SchematicObject RootSchematic
        {
            get => _rootSchematic ??= Base.GetComponent<SchematicObject>();
            set
            {
                _rootSchematic?.Destroy();
                _rootSchematic = value;
            }
        }

        /// <summary>
        /// Gets the name of mesh.
        /// </summary>
        public string MeshName { get; private set; }

        /// <summary>
        /// Gets or sets the socket.
        /// </summary>
        public FTransform Socket { get; set; }

        /// <inheritdoc/>
        public override Vector3 Scale
        {
            get => RootSchematic.Scale;
            set => RootSchematic.Scale = value;
        }

        /// <inheritdoc/>
        public override Vector3 Position
        {
            get => RootSchematic.Position;
            set => RootSchematic.Position = value;
        }

        /// <inheritdoc/>
        public override Quaternion Rotation
        {
            get => RootSchematic.Rotation;
            set => RootSchematic.Rotation = value;
        }

        /// <inheritdoc/>
        public override bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;

                if (!_isVisible)
                {
                    foreach (GameObject gameObject in RootSchematic.AttachedBlocks)
                        NetworkServer.UnSpawn(gameObject);

                    return;
                }

                foreach (GameObject gameObject in RootSchematic.AttachedBlocks)
                    NetworkServer.Spawn(gameObject);
            }
        }

        /// <inheritdoc/>
        public override bool IsCollidable
        {
            get => _isCollidable;
            set
            {
                if (_isCollidable == value)
                    return;

                _isCollidable = value;

                if (!_isCollidable)
                {
                    foreach (GameObject gameObject in RootSchematic.AttachedBlocks)
                    {
                        if (gameObject.gameObject.TryGetComponent(out PrimitiveObject component))
                            component.Primitive.Collidable = false;
                    }

                    return;
                }

                foreach (GameObject gameObject in RootSchematic.AttachedBlocks)
                {
                    if (gameObject.gameObject.TryGetComponent(out PrimitiveObject component))
                        component.Primitive.Collidable = true;
                }
            }
        }

        /// <inheritdoc/>
        IReadOnlyDictionary<Animator, string[]> IAnimatorNative.AnimatorsTriggers => throw new NotImplementedException();

        /// <summary>
        /// Creates a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="EObject.Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="ASchematicMeshComponent"/> instance.</param>
        /// <param name="meshName"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        /// <param name="socket"><inheritdoc cref="Socket"/></param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(
            GameObject gameObject,
            string name,
            string meshName,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation,
            FTransform socket)
            where T : ASchematicMeshComponent
        {
            ASchematicMeshComponent outer = new(socket, MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(meshName, position, rotation, scale), scale, position, rotation)
            {
                Name = name,
                Base = gameObject
            };

            return outer.Cast<T>();
        }

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectOfType<T>(Vector3 position, IEnumerable<SchematicObject> source)
            where T : ASchematicMeshComponent =>
            FindActiveObjectsOfType<T>().FirstOrDefault(aObj =>
            aObj.RootSchematic == source.Aggregate((curClosest, nextEntry) =>
            curClosest is null ||
            (Vector3.Distance(nextEntry.Position, position) <
            Vector3.Distance(curClosest.Position, position)) ?
            nextEntry :
            curClosest)) ?? null;

        /// <summary>
        /// Finds the <see cref="Pickup"/> instance of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> to iterate over.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup FindClosestFrameOfType<T>(Vector3 position, IEnumerable<Pickup> source)
            where T : Pickup =>
            Pickup.List.FirstOrDefault(pickup =>
            pickup == source.Aggregate((curClosest, nextEntry) =>
            curClosest is null ||
            (Vector3.Distance(nextEntry.Position, position) <
            Vector3.Distance(curClosest.Position, position)) ?
            nextEntry :
            curClosest)) ?? null;

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectOfType<T>(Vector3 position)
            where T : ASchematicMeshComponent => FindClosestObjectOfType<T>(position, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="schematicObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> from which looking for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectOfType<T>(SchematicObject schematicObject)
            where T : ASchematicMeshComponent => FindClosestObjectOfType<T>(schematicObject.Position, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="schematicMeshComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicMeshComponent">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> from which looking for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectOfType<T>(T schematicMeshComponent)
            where T : ASchematicMeshComponent => FindClosestObjectOfType<T>(schematicMeshComponent.Position, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="Pickup"/> instance of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup FindClosestFrameOfType<T>(Vector3 position)
            where T : Pickup => FindClosestFrameOfType<T>(position, Pickup.List);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectWithTagOfType<T>(Vector3 position, string tag, IEnumerable<SchematicObject> source)
            where T : ASchematicMeshComponent =>
            FindActiveObjectsOfType<T>().Where(nObj =>
            source.Contains(nObj.RootSchematic) &&
            nObj.Tag.ToLower().Contains(tag)).FirstOrDefault(aObj =>
            aObj.RootSchematic == source.Aggregate((curClosest, nextEntry) =>
            curClosest is null ||
            (Vector3.Distance(nextEntry.Position, position) <
            Vector3.Distance(curClosest.Position, position)) ?
            nextEntry :
            curClosest)) ?? null;

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="name">The name look for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectWithNameOfType<T>(Vector3 position, string name, IEnumerable<SchematicObject> source)
            where T : ASchematicMeshComponent =>
            FindActiveObjectsOfType<T>().Where(nObj =>
            source.Contains(nObj.RootSchematic) &&
            nObj.Name.ToLower().Contains(name)).FirstOrDefault(aObj =>
            aObj.RootSchematic == source.Aggregate((curClosest, nextEntry) =>
            curClosest is null ||
            (Vector3.Distance(nextEntry.Position, position) <
            Vector3.Distance(curClosest.Position, position)) ?
            nextEntry :
            curClosest)) ?? null;

        /// <summary>
        /// Finds the <see cref="Pickup"/> of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> to iterate over.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup FindClosestFrameWithTagOfType<T>(Vector3 position, string tag, IEnumerable<Pickup> source)
            where T : Pickup =>
            Pickup.Get(UnityEngine.Object.FindObjectsOfType<ItemPickupBase>().Where(nObj =>
            source.Contains(Pickup.Get(nObj)) &&
            nObj.name.ToLower().Contains(tag)).FirstOrDefault(aObj =>
            aObj == source.Select(p => p.Base).Aggregate((curClosest, nextEntry) =>
            curClosest is null ||
            (Vector3.Distance(nextEntry.transform.position, position) <
            Vector3.Distance(curClosest.transform.position, position)) ?
            nextEntry :
            curClosest))) ?? null;

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectWithTagOfType<T>(Vector3 position, string tag)
            where T : ASchematicMeshComponent => FindClosestObjectWithTagOfType<T>(position, tag, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="schematicObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectWithTagOfType<T>(SchematicObject schematicObject, string tag)
            where T : ASchematicMeshComponent => FindClosestObjectWithTagOfType<T>(schematicObject.Position, tag, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to <paramref name="schematicMeshComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicMeshComponent">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T FindClosestObjectWithTagOfType<T>(T schematicMeshComponent, string tag)
            where T : ASchematicMeshComponent => FindClosestObjectWithTagOfType<T>(schematicMeshComponent.Position, tag, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="Pickup"/> instance of type <typeparamref name="T"/> closest to <paramref name="pickup"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="pickup">The <see cref="Pickup"/> of type <typeparamref name="T"/> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup FindClosestFrameWithTagOfType<T>(T pickup, string tag)
            where T : Pickup => FindClosestFrameWithTagOfType<T>(pickup.Position, tag, Pickup.List);

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> FindClosestObjectsOfType<T>(Vector3 position, IEnumerable<SchematicObject> source, uint toGet = 0)
            where T : ASchematicMeshComponent
        {
            if (toGet == 0)
                toGet = (uint)source.Count();

            List<SchematicObject> objects = source.ToList();
            for (int i = 0; i < toGet; i++)
            {
                T outer = FindClosestObjectOfType<T>(position, source);
                if (outer is null)
                    break;

                objects.Remove(outer.RootSchematic);
                yield return outer;
            }
        }

        /// <summary>
        /// Finds a fixed amount of all the <see cref="Pickup"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> to iterate over.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<Pickup> FindClosestFramesOfType<T>(Vector3 position, IEnumerable<Pickup> source, uint toGet = 0)
            where T : Pickup
        {
            if (toGet == 0)
                toGet = (uint)source.Count();

            List<Pickup> objects = source.ToList();
            for (int i = 0; i < toGet; i++)
            {
                Pickup outer = FindClosestFrameOfType<T>(position, source);
                if (outer is null)
                    break;

                objects.Remove(outer);
                yield return outer;
            }
        }

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsOfType<T>(Vector3 position, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(position, FindActiveObjectsOfType<T>().Select(elem => elem.RootSchematic), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="schematicObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> from which looking for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsOfType<T>(SchematicObject schematicObject, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(schematicObject.Position, FindActiveObjectsOfType<T>().Select(elem => elem.RootSchematic), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="schematicMeshComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicMeshComponent">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> from which looking for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsOfType<T>(T schematicMeshComponent, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(schematicMeshComponent.Position, FindActiveObjectsOfType<T>().Select(elem => elem.RootSchematic), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="Pickup"/> instances of type <typeparamref name="T"/> closest to <paramref name="pickup"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="pickup">The <see cref="Pickup"/> of type <typeparamref name="T"/> from which looking for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup[] FindClosestFramesOfType<T>(T pickup, uint toGet = 0)
            where T : Pickup => FindClosestFramesOfType<T>(pickup.Position, Pickup.List, toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsWithTagOfType<T>(Vector3 position, string tag, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(position, FindActiveObjectsWithTagOfType<T>(tag).Select(elem => elem.RootSchematic), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup[] FindClosestFramesWithTagOfType<T>(Vector3 position, string tag, uint toGet = 0)
            where T : Pickup => FindClosestFramesOfType<T>(position, Pickup.List.Where(pickup => pickup.GameObject.name.ToLower().Contains(tag)), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsWithTagOfType<T>(Vector3 position, string tag, IEnumerable<SchematicObject> source, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(
                position,
                FindActiveObjectsWithTagOfType<T>(tag).Where(obj =>
                source.Contains(obj.RootSchematic)).Select(so =>
                so.RootSchematic), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="Pickup"/> instances of type <typeparamref name="T"/> closest to <paramref name="position"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="position">The <see cref="Vector3">position</see> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> to iterate over.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public static Pickup[] FindClosestFramesWithTagOfType<T>(Vector3 position, string tag, IEnumerable<Pickup> source, uint toGet = 0)
            where T : Pickup => FindClosestFramesOfType<T>(
                position,
                Pickup.List.Where(obj =>
                obj.GameObject.name.ToLower().Contains(tag) &&
                source.Contains(obj)), toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="schematicMeshComponent"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicMeshComponent">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsWithTagOfType<T>(T schematicMeshComponent, string tag, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsWithTagOfType<T>(schematicMeshComponent.Position, tag, toGet);

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to <paramref name="schematicObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> from which looking for.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public static T[] FindClosestObjectsWithTagOfType<T>(SchematicObject schematicObject, string tag, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsWithTagOfType<T>(schematicObject.Position, tag, toGet);

        /// <summary>
        /// Creates a new instance of the <see cref="ASchematicMeshComponent"/> class.
        /// </summary>
        /// <param name="mesh"><inheritdoc cref="RootSchematic"/></param>
        /// <param name="scale"><inheritdoc cref="EActor.Scale"/></param>
        /// <param name="position"><inheritdoc cref="EActor.Position"/></param>
        /// <param name="rotation"><inheritdoc cref="EActor.Rotation"/></param>
        /// <returns>The new instance of the <see cref="ASchematicMeshComponent"/> class.</returns>
        public static ASchematicMeshComponent Create(
            SchematicObject mesh,
            Vector3 scale,
            Vector3 position,
            Quaternion rotation) =>
            new(mesh, scale, position, rotation);

        /// <summary>
        /// Spawns the <see cref="RootSchematic"/>.
        /// </summary>
        public void ShowBones() => MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(RootSchematic.Base);

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <param name="tag">The tag look for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T[] FindClosestObjectsWithTagOfType<T>(IEnumerable<SchematicObject> source, string tag, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsWithTagOfType<T>(Position, tag, source, toGet);

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="tag">The tag look for.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T[] FindClosestObjectsWithTagOfType<T>(string tag, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsWithTagOfType<T>(Position, tag, toGet);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T FindClosestObjectWithTagOfType<T>(IEnumerable<SchematicObject> source, string tag)
            where T : ASchematicMeshComponent => FindClosestObjectWithTagOfType<T>(Position, tag, source);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T FindClosestObjectWithTagOfType<T>(string tag)
            where T : ASchematicMeshComponent => FindClosestObjectWithTagOfType<T>(Position, tag, SchematicObjects);

        /// <summary>
        /// Finds the <see cref="Pickup"/> instance of type <typeparamref name="T"/> closest to this <see cref="Pickup"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="tag">The tag look for.</param>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public Pickup FindClosestFrameWithTagOfType<T>(string tag)
            where T : Pickup => FindClosestFrameWithTagOfType<T>(Position, tag, Pickup.List);

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T[] FindClosestObjectsOfType<T>(IEnumerable<SchematicObject> source, uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(Position, source, toGet).ToArray();

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T[] FindClosestObjectsOfType<T>(uint toGet = 0)
            where T : ASchematicMeshComponent => FindClosestObjectsOfType<T>(Position, toGet);

        /// <summary>
        /// Finds a fixed amount of all the <see cref="ASchematicMeshComponent"/> instances of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="toGet">The requested amount of entries.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public Pickup[] FindClosestFramesOfType<T>(uint toGet = 0)
            where T : Pickup => FindClosestFramesOfType<T>(Position, Pickup.List, toGet).ToArray();

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of <see cref="SchematicObject"/> to iterate over.</param>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T FindClosestObjectOfType<T>(IEnumerable<SchematicObject> source)
            where T : ASchematicMeshComponent => FindClosestObjectOfType<T>(Position, source);

        /// <summary>
        /// Finds the <see cref="ASchematicMeshComponent"/> instance of type <typeparamref name="T"/> closest to this <see cref="ASchematicMeshComponent"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <returns>The <see cref="ASchematicMeshComponent"/> of type <typeparamref name="T"/>.</returns>
        public T FindClosestObjectOfType<T>()
            where T : ASchematicMeshComponent => FindClosestObjectOfType<T>(Position, SchematicObjects.Where(obj => obj.gameObject.name != RootSchematic.gameObject.name));

        /// <summary>
        /// Finds the <see cref="Pickup"/> instance of type <typeparamref name="T"/> closest to this <see cref="Pickup"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Pickup"/> of type <typeparamref name="T"/> to look for.</typeparam>
        /// <returns>The <see cref="Pickup"/> of type <typeparamref name="T"/>.</returns>
        public Pickup FindClosestFrameOfType<T>()
            where T : Pickup => FindClosestFrameOfType<T>(Position, Pickup.List);

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Timing.CallDelayed(0.01f, () =>
            {
                Framework.Events.Handlers.EObject.ChangingAnimationState += SyncAnimState;

                for (int i = 0; i < RootSchematic.AnimationController.Animators.Count; i++)
                {
                    Animator animator = RootSchematic.AnimationController.Animators[i];
                    AnimatorControllerParameter[] parameters = animator.parameters.Where(param => param.type == AnimatorControllerParameterType.Bool).ToArray();
                    AnimatorParameter[] bools = new AnimatorParameter[parameters.Length];
                    Timing.RunCoroutine(SyncAnimState(this, animator));
                    for (int j = 0; j < parameters.Length; j++)
                        bools[j] = new(animator, parameters[j].name);

                    _animatorsParams.Add(animator, bools);
                }
            });
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Framework.Events.Handlers.EObject.ChangingAnimationState -= SyncAnimState;

            _animatorsParams.Clear();
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            foreach (Animator animator in RootSchematic.AnimationController.Animators)
            {
                try
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
                    {
                        CompletedAnimationStateEventArgs ev = new(this, animator, animator.GetCurrentAnimatorStateInfo(0));
                        Framework.Events.Handlers.EObject.OnCompletedAnimationState(ev);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Plays an animation.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <param name="state">The name of the state.</param>
        /// <returns><see langword="true"/> if the animation was played; otherwise, <see langword="false"/>.</returns>
        public bool PlayAnimation(string name, bool state)
        {
            Animator animator = RootSchematic.AnimationController.Animators.FirstOrDefault(x =>
            x.parameters.Any(param => param.name == name));
            if (animator is null)
                return false;

            animator.SetBool(name, state);
            return true;
        }

        private static IEnumerator<float> SyncAnimState(ASchematicMeshComponent meshComponent, Animator animator)
        {
            string lastAnimState = string.Empty;
            while (meshComponent is not null)
            {
                yield return Timing.WaitForOneFrame;

                if (animator.GetCurrentAnimatorStateInfo(0).IsName(lastAnimState))
                    continue;

                string currentAnimState = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                AnimatorParameter param = new(animator, currentAnimState, true);
                ChangingAnimationStateEventArgs ev = new(meshComponent, animator, param);
                Framework.Events.Handlers.EObject.OnChangingAnimationState(ev);
                lastAnimState = currentAnimState;
            }

            yield break;
        }

        /// <inheritdoc/>
        public bool PlayAnimation(string name, bool state, int animatorIndex = 0)
        {
            KeyValuePair<Animator, AnimatorParameter[]> pair = _animatorsParams.FirstOrDefault(kvp =>
            kvp.Key == RootSchematic.AnimationController.Animators[animatorIndex]);

            AnimatorParameter param = pair.Value.FirstOrDefault(para => para.Name == name);
            if (param == default)
                return false;

            ChangingAnimationStateEventArgs ev = new(this, pair.Key, param);
            Framework.Events.Handlers.EObject.OnChangingAnimationState(ev);

            ev.AnimationParameter.State = state;

            return true;
        }

        /// <inheritdoc/>
        public bool PlayAnimation(AnimatorParameter anim, int animatorIndex = 0)
        {
            KeyValuePair<Animator, AnimatorParameter[]> pair = _animatorsParams.FirstOrDefault(kvp =>
            kvp.Key == RootSchematic.AnimationController.Animators[animatorIndex]);

            AnimatorParameter param = pair.Value.FirstOrDefault(para => para == anim);
            if (param == default)
                return false;

            ChangingAnimationStateEventArgs ev = new(this, pair.Key, anim);
            Framework.Events.Handlers.EObject.OnChangingAnimationState(ev);

            ev.AnimationParameter.State = anim.State;

            return true;
        }

        /// <inheritdoc/>
        public void StopAnimation(int animatorIndex = 0) => RootSchematic.AnimationController.Animators[animatorIndex].StopPlayback();

        /// <inheritdoc/>
        public AnimatorParameter[] GetAnimations(Animator animator) => _animatorsParams.FirstOrDefault(kvp => kvp.Key == animator).Value;

        /// <inheritdoc/>
        public AnimatorStateInfo GetCurrentAnimation(Animator animator = null) =>
            animator is null ?
            RootSchematic.AnimationController.Animators[0].GetCurrentAnimatorStateInfo(0) :
            RootSchematic.AnimationController.Animators.FirstOrDefault(anim => anim == animator).GetCurrentAnimatorStateInfo(0);

        /// <inheritdoc/>
        public ref AnimatorParameter GetCurrentAnimation(Animator animator, AnimatorParameter anim)
        {
            for (int i = 0; i < _animatorsParams.Count; i++)
            {
                KeyValuePair<Animator, AnimatorParameter[]> kvp = _animatorsParams.ElementAt(i);
                if (kvp.Key != animator)
                    continue;

                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    if (kvp.Value[j] != anim)
                        continue;

                    return ref kvp.Value[j];
                }
            }

            throw new NullReferenceException("Coudln't find reference object for the specified AnimationParameter paramter.");
        }

        /// <inheritdoc/>
        public ref AnimatorParameter GetCurrentAnimation(Animator animator, string name)
        {
            for (int i = 0; i < _animatorsParams.Count; i++)
            {
                KeyValuePair<Animator, AnimatorParameter[]> kvp = _animatorsParams.ElementAt(i);
                if (kvp.Key != animator)
                    continue;

                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    if (kvp.Value[j].Name != name)
                        continue;

                    return ref kvp.Value[j];
                }
            }

            throw new NullReferenceException("Coudln't find reference object for the specified AnimationParameter paramter.");
        }

        /// <inheritdoc/>
        public void SetCurrentAnimation(Animator animator, AnimatorParameter anim) => _animatorsParams.FirstOrDefault(kvp => kvp.Key == animator).Key.SetBool(anim.Name, anim.State);

        /// <inheritdoc/>
        public void SetCurrentAnimation(Animator animator, string name, bool state) => _animatorsParams.FirstOrDefault(kvp => kvp.Key == animator).Key.SetBool(name, state);

        /// <summary>
        /// Tries to get the animations of the specified <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The animator to check.</param>
        /// <param name="anims">The animations.</param>
        /// <returns><see langword="true"/> if the animations were successfully found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetAnimations(Animator animator, out AnimatorParameter[] anims)
        {
            anims = null;

            if (animator is null ||
                RootSchematic.AnimationController is null ||
                RootSchematic.AnimationController.Animators.IsEmpty())
                return false;

            anims = GetAnimations(animator);

            return true;
        }

        /// <inheritdoc/>
        public virtual void OnChangingAnimationState(ChangingAnimationStateEventArgs ev)
        {
        }

        /// <inheritdoc cref="Framework.Events.Handlers.EObject.OnChangingAnimationState(ChangingAnimationStateEventArgs)"/>
        private void SyncAnimState(ChangingAnimationStateEventArgs ev)
        {
            if (ev.MeshComponent != this)
                return;

            OnChangingAnimationState(ev);
        }
    }
}
