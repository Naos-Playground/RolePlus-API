// -----------------------------------------------------------------------
// <copyright file="TRule.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Network
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using MEC;

    /// <summary>
    /// Rule is the base class for objects used by for <see cref="TFirewall{T}"/>.
    /// </summary>
    /// <typeparam name="T">The data type to handle and identify actions.</typeparam>
    public abstract class TRule<T> : TypeCastObject<TRule<T>>
    {
        private readonly HashSet<T> _allowList = new();
        private readonly HashSet<T> _denyList = new();
        private string _allowPath;
        private string _denyPath;
        private CoroutineHandle _networkSyncHandle;

        /// <summary>
        /// Finalizes an instance of the <see cref="TRule{T}"/> class.
        /// </summary>
        ~TRule()
        {
            Unload();
            Timing.KillCoroutines(_networkSyncHandle);
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <typeparamref name="T"/> containing all the allowed <typeparamref name="T"/> objects.
        /// </summary>
        protected IReadOnlyCollection<T> Allowlist => _allowList.ToList().AsReadOnly();

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <typeparamref name="T"/> containing all the denied <typeparamref name="T"/> objects.
        /// </summary>
        protected IReadOnlyCollection<T> Denylist => _denyList.ToList().AsReadOnly();

        /// <summary>
        /// Gets or sets the name of the rule.
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Loads the rule.
        /// </summary>
        public virtual void Load()
        {
            Log("Activating rule...");
            string dir = Path.Combine(Internal.RolePlus.Singleton.Config.DataDirectory, Name);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _allowPath = Path.Combine(dir, $"Allow-{Name}.txt");
            _denyPath = Path.Combine(dir, $"Deny-{Name}.txt");
            _allowList.Clear();
            _denyList.Clear();

            LoadDefaultRuleset(_allowPath);
            LoadDefaultRuleset(_denyPath);

            _networkSyncHandle = Timing.RunCoroutine(Internal_NetworkSync());
            Log("Rule Activated");
        }

        /// <summary>
        /// Fired every frame.
        /// </summary>
        protected virtual void NetworkSync()
        {
        }

        /// <summary>
        /// Unloads the rule.
        /// </summary>
        public virtual void Unload() => Timing.KillCoroutines(_networkSyncHandle);

        /// <summary>
        /// Logs the specified message into the server console.
        /// </summary>
        /// <param name="info">The message to display.</param>
        protected virtual void Log(string info) => Exiled.API.Features.Log.SendRaw($"NETWORK RULE OUTPUT - [{Name}] {info}", System.ConsoleColor.Yellow);

        /// <summary>
        /// Tries to parse raw data into <typeparamref name="T"/> data.
        /// </summary>
        /// <param name="raw">Data to parse.</param>
        /// <param name="data">Parsed data.</param>
        /// <returns><see langword="true"/> if data was parsed successfully; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryParse(string raw, out T data);

        /// <summary>
        /// Allows the given <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="param">The object to allow.</param>
        public void Allow(T param)
        {
            if (_denyList.Remove(param))
            {
                string item = param.ToString();
                string[] data = File.ReadAllLines(_denyPath).Where(ln => ln.Trim() != item).ToArray();
                File.WriteAllLines(_denyPath, data);
            }

            if (_allowList.Add(param))
            {
                using StreamWriter sw = File.AppendText(_allowPath);
                sw.WriteLine(param.ToString());
            }
        }

        /// <summary>
        /// Denies the given <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="param">The object to deny.</param>
        public void Deny(T param)
        {
            if (_allowList.Remove(param))
            {
                string item = param.ToString();
                string[] data = File.ReadAllLines(_allowPath).Where(ln => ln.Trim() != item).ToArray();
                File.WriteAllLines(_allowPath, data);
            }

            _denyList.Add(param);
            if (_denyList.Add(param))
            {
                using StreamWriter sw = File.AppendText(_denyPath);
                sw.WriteLine(param.ToString());
            }
        }

        /// <summary>
        /// Checks whether the given <typeparamref name="T"/> object is allowed.
        /// </summary>
        /// <param name="param">The object to allow.</param>
        /// <returns><see langword="true"/> if is allowed; otherwise, <see langword="false"/>.</returns>
        public bool IsAllowed(T param) => _allowList.Contains(param);

        /// <summary>
        /// Checks whether the given <typeparamref name="T"/> object is denied.
        /// </summary>
        /// <param name="param">The object to allow.</param>
        /// <returns><see langword="true"/> if is denied; otherwise, <see langword="false"/>.</returns>
        public bool IsDenied(T param) => _denyList.Contains(param);

        private void LoadDefaultRuleset(string path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, null);

            foreach (string ln in File.ReadAllLines(path))
            {
                if (TryParse(ln, out T data))
                {
                    if (path.Contains("Allow"))
                        _allowList.Add(data);
                    else
                        _denyList.Add(data);
                }
            }
        }

        private IEnumerator<float> Internal_NetworkSync()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;

                NetworkSync();
            }
        }
    }
}
