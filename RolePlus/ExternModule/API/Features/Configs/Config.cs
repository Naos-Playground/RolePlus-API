// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Configs
{
#nullable enable

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Loader;

    using RolePlus.ExternModule.API.Engine.Core;

    /// <summary>
    /// The base class that handles the config system.
    /// </summary>
    public sealed class Config : TypeCastObject<object>
    {
        private static readonly Dictionary<Config, string> _cache = new();
        private static readonly List<Config> _mainConfigsValue = new();
        internal static readonly List<Config> _configsValue = new();

        private readonly HashSet<Config> _data = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="obj">The config object.</param>
        public Config(object obj) => Base = obj;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> containing all the <see cref="Config"/>.
        /// </summary>
        public static IReadOnlyCollection<Config> List => _configsValue;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all the subconfigs.
        /// </summary>
        public IEnumerable<Config> Subconfigs => _data;

        /// <summary>
        /// Gets the base config instance.
        /// </summary>
        public object? Base { get; private set; }

        /// <summary>
        /// Gets or sets the config's folder.
        /// </summary>
        public string? Folder { get; set; }

        /// <summary>
        /// Gets or sets the config's name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets the absolute path.
        /// </summary>
        public string? AbsolutePath => Path.Combine(Paths.Configs, Path.Combine(Folder, Name));

        /// <summary>
        /// Gets a <see cref="Config"/> instance given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config to look for.</typeparam>
        /// <returns>The corresponding <see cref="Config"/> instance or <see langword="null"/> if not found.</returns>
        public static Config? Get<T>()
            where T : class => _configsValue.FirstOrDefault(config => config?.Base?.GetType().FullName == typeof(T).FullName);

        /// <summary>
        /// Gets a <see cref="Config"/> instance given the specified folder.
        /// </summary>
        /// <param name="folder">The folder of the config to look for.</param>
        /// <returns>The corresponding <see cref="Config"/> instance or <see langword="null"/> if not found.</returns>
        public static Config Get(string folder) => List.FirstOrDefault(cfg => cfg.Folder == folder);

        /// <summary>
        /// Generates a new config of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config.</typeparam>
        /// <returns>The generated config.</returns>
        public static Config? GenerateNew<T>()
            where T : class
        {
            if (!typeof(T).GetInterfaces().Contains(typeof(IConfig)))
                throw new ArgumentException("Type must inherit from IConfig.");

            Config? config = Get<T>();
            if (config is not null)
                return config;

            return Load(typeof(T), typeof(T).GetCustomAttribute<ConfigAttribute>());
        }

        /// <summary>
        /// Loads all the configs.
        /// </summary>
        public static void LoadAll()
        {
            _mainConfigsValue.Clear();
            _configsValue.Clear();
            _cache.Clear();

            Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract)
                .ToList()
                .ForEach(t => Load(t));
        }

        /// <summary>
        /// Loads a config from a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The config type.</param>
        /// <param name="attribute">The config data.</param>
        /// <returns>The <see cref="Config"/> object.</returns>
        public static Config? Load(Type type, ConfigAttribute? attribute = null)
        {
            object? config = null;

            try
            {
                attribute ??= type.GetCustomAttribute<ConfigAttribute>();
                if (attribute is null)
                    return null;

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor is not null)
                    config = constructor.Invoke(null)!;
                else
                {
                    object value = Array.Find(type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == type)?.GetValue(null)!;
                    if (value is not null)
                        config = value;
                }

                if (config is null)
                {
                    Log.Error($"{type.FullName} is a valid config, but it cannot be instantiated!" +
                        $"It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                    return null;
                }

                Config wrapper = new(config);
                if (string.IsNullOrEmpty(wrapper.Folder))
                {
                    if (string.IsNullOrEmpty(attribute.Folder))
                    {
                        Log.SendRaw(
                            $"[{Assembly.GetCallingAssembly().GetName().Name}] The folder of the object of type {config!.GetType()} ({wrapper.Name}) has not been set." +
                            $"It's not possible determine the parent config of which belongs to, hence it won't be read.",
                            ConsoleColor.Red);

                        return null;
                    }
                    else
                        wrapper!.Folder = attribute.Folder;
                }

                if (string.IsNullOrEmpty(wrapper.Name))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                    {
                        wrapper!.Name = config.GetType().Name;
                        Log.SendRaw(
                            $"[{Assembly.GetCallingAssembly().GetName().Name}] The config's name of the object of type {config.GetType()} has not been set." +
                            $"The object's type name ({config.GetType().Name}) will be used instead.",
                            ConsoleColor.Red);
                    }
                    else
                        wrapper!.Name = attribute.Name;
                }

                _configsValue.Add(wrapper);
                if (!wrapper!.Name!.Contains(".yml"))
                    wrapper.Name += ".yml";

                string path = Path.Combine(Paths.Configs, wrapper.Folder);
                if (attribute.IsParent)
                {
                    if (!Directory.Exists(Path.Combine(path)))
                        Directory.CreateDirectory(path);

                    Load(wrapper, wrapper.AbsolutePath!);
                    wrapper!._data!.Add(wrapper);
                    _mainConfigsValue.Add(wrapper);

                    Dictionary<Config, string> localCache = new(_cache);
                    foreach (KeyValuePair<Config, string> elem in localCache)
                        LoadFromCache(elem.Key);

                    return wrapper;
                }

                _cache.Add(wrapper, wrapper.AbsolutePath!);
                if (!Directory.Exists(path) || !_mainConfigsValue.Any(cfg => cfg.Folder == wrapper.Folder))
                    return wrapper;

                LoadFromCache(wrapper);

                if (!_configsValue.Contains(wrapper))
                    _configsValue.Add(wrapper);

                return wrapper;
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                Log.Error($"Error while initializing config {Assembly.GetCallingAssembly().GetName().Name} (at {Assembly.GetCallingAssembly().Location})! {reflectionTypeLoadException}");

                foreach (Exception? loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    Log.Error(loaderException);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error while initializing config {Assembly.GetCallingAssembly().GetName().Name} (at {Assembly.GetCallingAssembly().Location})! {exception}");
            }

            return null;
        }

        /// <summary>
        /// Loads a config from the cached configs.
        /// </summary>
        /// <param name="config">The config to load.</param>
        public static void LoadFromCache(Config config)
        {
            if (!_cache.TryGetValue(config, out string path))
                return;

            foreach (Config cfg in _mainConfigsValue)
            {
                if (string.IsNullOrEmpty(cfg.Folder) || cfg.Folder != config.Folder)
                    continue;

                cfg._data!.Add(config);
            }

            Load(config, path);
            _cache.Remove(config);
        }

        /// <summary>
        /// Loads a config.
        /// </summary>
        /// <param name="config">The config to load.</param>
        /// <param name="path">The config's path.</param>
        public static void Load(Config config, string? path = null)
        {
            path ??= config.AbsolutePath;
            if (!File.Exists(path))
                File.WriteAllText(path, Loader.Serializer.Serialize(config.Base!));
            else
            {
                config.Base = Loader.Deserializer.Deserialize(File.ReadAllText(path), config.Base!.GetType())!;
                File.WriteAllText(path, Loader.Serializer.Serialize(config.Base!));
            }
        }

        /// <summary>
        /// Gets the path of the specified data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding data's path or <see langword="null"/> if not found.</returns>
        public static string? GetPath<T>()
            where T : class
        {
            object? config = Get<T>();
            return config is null || config is not Config configBase ? null : configBase.AbsolutePath;
        }

        /// <summary>
        /// Reads a data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding <typeparamref name="T"/> instance or <see langword="null"/> if not found.</returns>
        public T? Read<T>()
            where T : class => _data.FirstOrDefault(data => data.Base!.GetType() == typeof(T)).Cast<T>();

        /// <summary>
        /// Writes a new value contained in the specified config of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be written.</typeparam>
        /// <param name="name">The name of the parameter to be modified.</param>
        /// <param name="value">The new value to be written.</param>
        public void Write<T>(string name, object value)
            where T : class
        {
            T? param = Read<T>();
            if (param is null)
                return;

            string? path = GetPath<T>();
            PropertyInfo propertyInfo = param.GetType().GetProperty(name);
            if (propertyInfo is not null)
            {
                propertyInfo.SetValue(param, value);
                File.WriteAllText(path, Loader.Serializer.Serialize(param));
                this.CopyProperties(Loader.Deserializer.Deserialize(File.ReadAllText(path), GetType()));
            }
        }

        /// <inheritdoc/>
        public override T? Cast<T>()
            where T : class => Base as T;

        /// <summary>
        /// Safely casts the current <see cref="Base"/> instance to the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type to which to cast the <see cref="Base"/> instance.</typeparam>
        /// <param name="param">The casted object.</param>
        /// <returns><see langword="true"/> if the <see cref="Base"/> instance was successfully casted; otherwise, <see langword="false"/>.</returns>
        public override bool Cast<T>(out T param)
            where T : class
        {
            param = default!;

            if (Base is not T cast)
                return false;

            param = cast;
            return true;
        }
    }
}
