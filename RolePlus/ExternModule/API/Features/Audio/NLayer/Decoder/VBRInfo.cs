// -----------------------------------------------------------------------
// <copyright file="VBRInfo.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio.NLayer.Decoder
{
    internal class VBRInfo
    {
        internal int SampleCount { get; set; }

        internal int SampleRate { get; set; }

        internal int Channels { get; set; }

        internal int VBRFrames { get; set; }

        internal int VBRBytes { get; set; }

        internal int VBRQuality { get; set; }

        internal int VBRDelay { get; set; }

        internal long VBRStreamSampleCount =>

            // we assume the entire stream is consistent wrt samples per frame
            VBRFrames * SampleCount;

        internal int VBRAverageBitrate => (int)(VBRBytes / (VBRStreamSampleCount / (double)SampleRate) * 8);
    }
}