// -----------------------------------------------------------------------
// <copyright file="AudioFile.cs" company="Tesla IT Studios">
// Copyright (c) Tesla IT Studios. All rights reserved.
// Licensed under the CC BY-NC-ND 4.0 Derivative license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio
{
    using API;

    using MEC;

    /// <summary>
    /// The object to define audio files' properties.
    /// </summary>
    public class AudioFile
    {
        /// <summary>
        /// The path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The volume.
        /// </summary>
        public float Volume { get; set; } = 100f;

        /// <summary>
        /// Plays the audio file.
        /// </summary>
        /// <param name="loop">A value indicating whether the audio file should be looped.</param>
        /// <param name="automatic">A value indicating whether the audio file should be automatic.</param>
        public void Play(bool loop = false, bool automatic = false)
        {
            Timing.RunCoroutine(AudioController.PlayFromFile(Path, Volume));
        }
    }
}