// -----------------------------------------------------------------------
// <copyright file="Enums.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio.NLayer
{
    /// <summary>
    /// All the available Mpeg versions.
    /// </summary>
    public enum MpegVersion
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Version 1.
        /// </summary>
        Version1 = 10,

        /// <summary>
        /// Version 2.
        /// </summary>
        Version2 = 20,

        /// <summary>
        /// Version 25.
        /// </summary>
        Version25 = 25,
    }

    /// <summary>
    /// All the available Mpeg layers.
    /// </summary>
    public enum MpegLayer
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Layer 1.
        /// </summary>
        LayerI = 1,

        /// <summary>
        /// Layer 2.
        /// </summary>
        LayerII = 2,

        /// <summary>
        /// Layer 3.
        /// </summary>
        LayerIII = 3,
    }

    /// <summary>
    /// All the available Mpeg channel modes.
    /// </summary>
    public enum MpegChannelMode
    {
        /// <summary>
        /// Stereo mode.
        /// </summary>
        Stereo,

        /// <summary>
        /// Joint stereo mode.
        /// </summary>
        JointStereo,

        /// <summary>
        /// Dual channel mode.
        /// </summary>
        DualChannel,

        /// <summary>
        /// Mono mode.
        /// </summary>
        Mono,
    }

    /// <summary>
    /// All the available stereo modes.
    /// </summary>
    public enum StereoMode
    {
        /// <summary>
        /// Both.
        /// </summary>
        Both,

        /// <summary>
        /// Left only.
        /// </summary>
        LeftOnly,

        /// <summary>
        /// Right only.
        /// </summary>
        RightOnly,

        /// <summary>
        /// Downmix to mono.
        /// </summary>
        DownmixToMono,
    }
}