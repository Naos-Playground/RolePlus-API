// -----------------------------------------------------------------------
// <copyright file="IMpegFrame.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio.NLayer
{
    /// <summary>
    /// Defines a standard way of representing a MPEG frame to the decoder.
    /// </summary>
    public interface IMpegFrame
    {
        /// <summary>
        /// Gets sample rate of this frame.
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        /// Gets the samplerate index (directly from the header).
        /// </summary>
        int SampleRateIndex { get; }

        /// <summary>
        /// Gets frame length in bytes.
        /// </summary>
        int FrameLength { get; }

        /// <summary>
        /// Gets bit Rate.
        /// </summary>
        int BitRate { get; }

        /// <summary>
        /// Gets mPEG Version.
        /// </summary>
        MpegVersion Version { get; }

        /// <summary>
        /// Gets mPEG Layer.
        /// </summary>
        MpegLayer Layer { get; }

        /// <summary>
        /// Gets channel Mode.
        /// </summary>
        MpegChannelMode ChannelMode { get; }

        /// <summary>
        /// Gets the number of samples in this frame.
        /// </summary>
        int ChannelModeExtension { get; }

        /// <summary>
        /// Gets the channel extension bits.
        /// </summary>
        int SampleCount { get; }

        /// <summary>
        /// Gets the bitrate index (directly from the header).
        /// </summary>
        int BitRateIndex { get; }

        /// <summary>
        /// Gets a value indicating whether whether the Copyright bit is set.
        /// </summary>
        bool IsCopyrighted { get; }

        /// <summary>
        /// Gets a value indicating whether whether a CRC is present.
        /// </summary>
        bool HasCrc { get; }

        /// <summary>
        /// Gets a value indicating whether whether the CRC check failed (use error concealment strategy).
        /// </summary>
        bool IsCorrupted { get; }

        /// <summary>
        /// Resets the bit reader so frames can be reused.
        /// </summary>
        void Reset();

        /// <summary>
        /// Provides sequential access to the bitstream in the frame (after the header and optional CRC).
        /// </summary>
        /// <param name="bitCount">The number of bits to read.</param>
        /// <returns>-1 if the end of the frame has been encountered, otherwise the bits requested.</returns>
        int ReadBits(int bitCount);
    }
}