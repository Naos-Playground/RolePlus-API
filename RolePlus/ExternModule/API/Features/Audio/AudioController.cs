// -----------------------------------------------------------------------
// <copyright file="AudioController.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio.API
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Dissonance;

    using MEC;

    using NLayer;

    using UnityEngine;

    using Log = Exiled.API.Features.Log;

    /// <summary>
    /// A set of tools to easily handle audio-related features.
    /// </summary>
    public static class AudioController
    {
        private static CustomMpegMicrophone _microphone;

        /// <summary>
        /// Gets the <see cref="DissonanceComms"/> object.
        /// </summary>
        public static DissonanceComms Comms => Radio.comms;

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public static float Volume { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the audio controller should play automatic music.
        /// </summary>
        public static bool AutomaticMusic { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the audio controller should loop music.
        /// </summary>
        public static bool LoopMusic { get; set; } = false;

        /// <summary>
        /// Gets or sets all the muted players.
        /// </summary>
        public static List<string> MutedPlayers { get; set; } = new();

        /// <summary>
        /// Plays an audio from a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="volume">The audio volume.</param>
        /// <param name="loop">A value indicating whether the audio should be looped.</param>
        /// <param name="automatic">A value indicating whether the audio should be automatic.</param>
        /// <returns>The play rate yield.</returns>
        public static IEnumerator<float> PlayFromFile(string path, float volume = 100, bool loop = false, bool automatic = false)
        {
            if (string.IsNullOrWhiteSpace(path))
                yield break;

            if (!File.Exists(path))
            {
                Log.Error($"Error trying to play: {path}. File not found.");
                yield break;
            }

            Stop();

            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;

            Volume = Mathf.Clamp(volume, 0, 100) / 100;
            RefreshChannels();

            if (_microphone is null)
                AddMic();

            _microphone!.File = new MpegFile(path);
            _microphone.Stop = false;

            Comms._capture._microphone = _microphone;
            Comms.ResetMicrophoneCapture();
            Comms.IsMuted = false;

            AutomaticMusic = automatic;
            LoopMusic = loop;
        }

        /// <summary>
        /// Stop the current audio which is being played.
        /// </summary>
        public static void Stop()
        {
            if (_microphone is null)
                return;

            _microphone.Stop = true;

            Log.Debug("Stopped the mic.", Internal.RolePlus.Singleton.Config.ShowDebugMessages);
        }

        /// <summary>
        /// Refreshes all the open channels.
        /// </summary>
        public static void RefreshChannels()
        {
            foreach (PlayerChannel channel in Comms.PlayerChannels._openChannelsBySubId.Values.ToList())
            {
                Comms.PlayerChannels.Close(channel);
                Comms.PlayerChannels.Open(channel.TargetId, false, ChannelPriority.Default, Volume);
            }
        }

        internal static void OnPlayerJoinedSession(VoicePlayerState player)
        {
            Log.Debug($"A player joined the session. ({player.Name})", Internal.RolePlus.Singleton.Config.ShowDebugMessages);

            Comms.PlayerChannels.Open(player.Name, false, ChannelPriority.Default, Volume);
        }

        internal static void OnPlayerLeftSession(VoicePlayerState player)
        {
            Log.Debug($"A player left the session. ({player.Name})", Internal.RolePlus.Singleton.Config.ShowDebugMessages);
        }

        private static void AddMic()
        {
            _microphone = Comms.gameObject.AddComponent<CustomMpegMicrophone>();
        }
    }
}