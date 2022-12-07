// -----------------------------------------------------------------------
// <copyright file="MpegFrameDecoder.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio.NLayer
{
    using System;

    using RolePlus.ExternModule.API.Features.Audio.NLayer.Decoder;

    /// <summary>
    /// The frame decoder for Mpeg objects.
    /// </summary>
    public class MpegFrameDecoder
    {
        // channel buffers for getting data out of the decoders...
        // we do it this way so the stereo interleaving code is in one place: DecodeFrameImpl(...)
        // if we ever add support for multi-channel, we'll have to add a pass after the initial
        //  stereo decode (since multi-channel basically uses the stereo channels as a reference)
        private readonly float[] _ch0;
        private readonly float[] _ch1;

        private float[] _eqFactors;
        private LayerIDecoder _layerIDecoder;
        private LayerIIDecoder _layerIIDecoder;
        private LayerIIIDecoder _layerIIIDecoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MpegFrameDecoder"/> class.
        /// </summary>
        public MpegFrameDecoder()
        {
            _ch0 = new float[1152];
            _ch1 = new float[1152];
        }

        /// <summary>
        /// Gets or sets stereo mode used in decoding.
        /// </summary>
        public StereoMode StereoMode { get; set; }

        /// <summary>
        /// Set the equalizer.
        /// </summary>
        /// <param name="eq">The equalizer, represented by an array of 32 adjustments in dB.</param>
        public void SetEQ(float[] eq)
        {
            if (eq != null)
            {
                float[] factors = new float[32];

                // convert from dB -> scaling
                for (int i = 0; i < eq.Length; i++)
                    factors[i] = (float)Math.Pow(2, eq[i] / 6);

                _eqFactors = factors;
            }
            else
            {
                _eqFactors = null;
            }
        }

        /// <summary>
        /// Decode the Mpeg frame into provided buffer. Do exactly the same as
        /// <see cref="DecodeFrame(IMpegFrame, float[], int)" />
        /// except that the data is written in type as byte array, while still representing single-precision float (in local
        /// endian).
        /// </summary>
        /// <param name="frame">The Mpeg frame to be decoded.</param>
        /// <param name="dest">Destination buffer. Decoded PCM (single-precision floating point array) will be written into it.</param>
        /// <param name="destOffset">Writing offset on the destination buffer.</param>
        /// <returns>The buffer.</returns>
        public int DecodeFrame(IMpegFrame frame, byte[] dest, int destOffset)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            if (dest == null)
                throw new ArgumentNullException("dest");
            if (destOffset % 4 != 0)
                throw new ArgumentException("Must be an even multiple of 4", "destOffset");

            int bufferAvailable = (dest.Length - destOffset) / 4;
            if (bufferAvailable < (frame.ChannelMode == MpegChannelMode.Mono ? 1 : 2) * frame.SampleCount)
            {
                throw new ArgumentException(
                    "Buffer not large enough!  Must be big enough to hold the frame's entire output.  This is up to 9,216 bytes.",
                    "dest");
            }

            return DecodeFrameImpl(frame, dest, destOffset / 4) * 4;
        }

        /// <summary>
        /// Decode the Mpeg frame into provided buffer.
        /// Result varies with different <see cref="StereoMode" />:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             For <see cref="RolePlus.ExternModule.API.Features.Audio.NLayer.StereoMode.Both" />, sample data on both two channels will occur in turn
        ///             (left first).
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             For <see cref="RolePlus.ExternModule.API.Features.Audio.NLayer.StereoMode.LeftOnly" /> and <see cref="RolePlus.ExternModule.API.Features.Audio.NLayer.StereoMode.RightOnly" />, only data
        ///             on
        ///             specified channel will occur.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             For <see cref="RolePlus.ExternModule.API.Features.Audio.NLayer.StereoMode.DownmixToMono" />, two channels will be down-mixed into
        ///             single channel.
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="frame">The Mpeg frame to be decoded.</param>
        /// <param name="dest">Destination buffer. Decoded PCM (single-precision floating point array) will be written into it.</param>
        /// <param name="destOffset">Writing offset on the destination buffer.</param>
        /// <returns>The buffer.</returns>
        public int DecodeFrame(IMpegFrame frame, float[] dest, int destOffset)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            if (dest == null)
                throw new ArgumentNullException("dest");

            if (dest.Length - destOffset < (frame.ChannelMode == MpegChannelMode.Mono ? 1 : 2) * frame.SampleCount)
            {
                throw new ArgumentException(
                    "Buffer not large enough!  Must be big enough to hold the frame's entire output.  This is up to 2,304 elements.",
                    "dest");
            }

            return DecodeFrameImpl(frame, dest, destOffset);
        }

        private int DecodeFrameImpl(IMpegFrame frame, Array dest, int destOffset)
        {
            frame.Reset();

            LayerDecoderBase curDecoder = null;
            switch (frame.Layer)
            {
                case MpegLayer.LayerI:
                    if (_layerIDecoder == null)
                        _layerIDecoder = new LayerIDecoder();
                    curDecoder = _layerIDecoder;
                    break;
                case MpegLayer.LayerII:
                    if (_layerIIDecoder == null)
                        _layerIIDecoder = new LayerIIDecoder();
                    curDecoder = _layerIIDecoder;
                    break;
                case MpegLayer.LayerIII:
                    if (_layerIIIDecoder == null)
                        _layerIIIDecoder = new LayerIIIDecoder();
                    curDecoder = _layerIIIDecoder;
                    break;
            }

            if (curDecoder != null)
            {
                curDecoder.SetEQ(_eqFactors);
                curDecoder.StereoMode = StereoMode;
                int cnt = curDecoder.DecodeFrame(frame, _ch0, _ch1);
                Buffer.BlockCopy(_ch0, 0, dest, destOffset * sizeof(float), cnt * sizeof(float));
                return cnt;
            }

            return 0;
        }

        /// <summary>
        /// Reset the decoder.
        /// </summary>
        public void Reset()
        {
            // the synthesis filters need to be cleared
            if (_layerIDecoder != null)
                _layerIDecoder.ResetForSeek();
            if (_layerIIDecoder != null)
                _layerIIDecoder.ResetForSeek();
            if (_layerIIIDecoder != null)
                _layerIIIDecoder.ResetForSeek();
        }
    }
}