// -----------------------------------------------------------------------
// <copyright file="MpegFile.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio.NLayer
{
    using System;
    using System.IO;

    using RolePlus.ExternModule.API.Features.Audio.NLayer.Decoder;

    /// <summary>
    /// The base <see cref="MpegFile"/> class.
    /// </summary>
    public class MpegFile : IDisposable
    {
        private readonly float[] _readBuf = new float[1152 * 2];
        private readonly object _seekLock = new();
        private bool _closeStream;
        private bool _eofFound;
        private MpegFrameDecoder _decoder;
        private long _position;
        private int _readBufLen;
        private int _readBufOfs;
        private MpegStreamReader _reader;
        private Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="MpegFile"/> class.
        /// Construct Mpeg file representation from filename.
        /// </summary>
        /// <param name="fileName">The file which contains Mpeg data.</param>
        public MpegFile(string fileName)
        {
            Init(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpegFile"/> class.
        /// Construct Mpeg file representation from stream.
        /// </summary>
        /// <param name="stream">The input stream which contains Mpeg data.</param>
        public MpegFile(Stream stream)
        {
            Init(stream, false);
        }

        /// <summary>
        /// Gets sample rate of source Mpeg, in Hertz.
        /// </summary>
        public int SampleRate => _reader.SampleRate;

        /// <summary>
        /// Gets channel count of source Mpeg.
        /// </summary>
        public int Channels => _reader.Channels;

        /// <summary>
        /// Gets a value indicating whether the Mpeg stream supports seek operation.
        /// </summary>
        public bool CanSeek => _reader.CanSeek;

        /// <summary>
        /// Gets data length of decoded data, in PCM.
        /// </summary>
        public long Length => _reader.SampleCount * _reader.Channels * sizeof(float);

        /// <summary>
        /// Gets media duration of the Mpeg file.
        /// </summary>
        public TimeSpan Duration => _reader.SampleCount == -1 ? TimeSpan.Zero : TimeSpan.FromSeconds((double)_reader.SampleCount / _reader.SampleRate);

        /// <summary>
        /// Gets or sets current decode position, in number of sample. Calling the setter will result in a seeking operation.
        /// </summary>
        public long Position
        {
            get => _position;
            set
            {
                if (!_reader.CanSeek)
                    throw new InvalidOperationException("Cannot Seek!");

                if (value < 0L)
                    throw new ArgumentOutOfRangeException("value");

                // we're thinking in 4-byte samples, pcmStep interleaved...  adjust accordingly
                long samples = value / sizeof(float) / _reader.Channels;
                int sampleOffset = 0;

                // seek to the frame preceding the one we want (unless we're seeking to the first frame)
                if (samples >= _reader.FirstFrameSampleCount)
                {
                    sampleOffset = _reader.FirstFrameSampleCount;
                    samples -= sampleOffset;
                }

                lock (_seekLock)
                {
                    // seek the stream
                    long newPos = _reader.SeekTo(samples);
                    if (newPos == -1)
                        throw new ArgumentOutOfRangeException("value");

                    _decoder.Reset();

                    // if we have a sample offset, decode the next frame
                    if (sampleOffset != 0)
                    {
                        _decoder.DecodeFrame(_reader.NextFrame(), _readBuf, 0); // throw away a frame (but allow the decoder to resync)
                        newPos += sampleOffset;
                    }

                    _position = newPos * sizeof(float) * _reader.Channels;
                    _eofFound = false;

                    // clear the decoder & buffer
                    _readBufOfs = _readBufLen = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets current decode position, represented by time. Calling the setter will result in a seeking operation.
        /// </summary>
        public TimeSpan Time
        {
            get => TimeSpan.FromSeconds((double)_position / sizeof(float) / _reader.Channels / _reader.SampleRate);
            set => Position = (long)(value.TotalSeconds * _reader.SampleRate * _reader.Channels * sizeof(float));
        }

        /// <summary>
        /// Gets or sets stereo mode used in decoding.
        /// </summary>
        public StereoMode StereoMode
        {
            get => _decoder.StereoMode;
            set => _decoder.StereoMode = value;
        }

        /// <summary>
        /// Implements IDisposable.Dispose.
        /// </summary>
        public void Dispose()
        {
            if (_closeStream)
            {
                _stream.Dispose();
                _closeStream = false;
            }
        }

        private void Init(Stream stream, bool closeStream)
        {
            _stream = stream;
            _closeStream = closeStream;

            _reader = new MpegStreamReader(_stream);

            _decoder = new MpegFrameDecoder();
        }

        /// <summary>
        /// Set the equalizer.
        /// </summary>
        /// <param name="eq">The equalizer, represented by an array of 32 adjustments in dB.</param>
        public void SetEQ(float[] eq)
        {
            _decoder.SetEQ(eq);
        }

        /// <summary>
        /// Read specified samples into provided buffer. Do exactly the same as <see cref="ReadSamples(float[], int, int)" />
        /// except that the data is written in type of byte, while still representing single-precision float (in local endian).
        /// </summary>
        /// <param name="buffer">Buffer to write. Floating point data will be actually written into this byte array.</param>
        /// <param name="index">Writing offset on the destination buffer.</param>
        /// <param name="count">Length of samples to be read, in bytes.</param>
        /// <returns>Sample size actually reads, in bytes.</returns>
        public int ReadSamples(byte[] buffer, int index, int count)
        {
            if (index < 0 || index + count > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            // make sure we're asking for an even number of samples
            count -= count % sizeof(float);

            return ReadSamplesImpl(buffer, index, count, 32);
        }

        /// <summary>
        /// Read specified samples into provided buffer, as PCM format.
        /// Result varies with diffirent <see cref="StereoMode" />:
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
        /// <param name="buffer">Buffer to write.</param>
        /// <param name="index">Writing offset on the destination buffer.</param>
        /// <param name="count">Count of samples to be read.</param>
        /// <returns>Sample count actually reads.</returns>
        public int ReadSamples(float[] buffer, int index, int count)
        {
            if (index < 0 || index + count > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            // ReadSampleImpl "thinks" in bytes, so adjust accordingly
            return ReadSamplesImpl(buffer, index * sizeof(float), count * sizeof(float), 32) / sizeof(float);
        }

        /// <inheritdoc cref="ReadSamples(byte[], int, int)"/>
        public int ReadSamplesInt16(byte[] buffer, int index, int count)
        {
            if (index < 0 || index + count > buffer.Length * sizeof(short))
                throw new ArgumentOutOfRangeException("index");

            return ReadSamplesImpl(buffer, index, count, 16) * sizeof(short) / sizeof(float);
        }

        /// <inheritdoc cref="ReadSamples(byte[], int, int)"/>
        public int ReadSamplesInt8(byte[] buffer, int index, int count)
        {
            if (index < 0 || index + count > buffer.Length * sizeof(float))
                throw new ArgumentOutOfRangeException("index");

            return ReadSamplesImpl(buffer, index, count, 8) * sizeof(byte) / sizeof(float);
        }

        private int ReadSamplesImpl(Array buffer, int index, int count, int bitDepth)
        {
            int cnt = 0;

            // lock around the entire read operation so seeking doesn't bork our buffers as we decode
            lock (_seekLock)
            {
                while (count > 0)
                {
                    if (_readBufLen > _readBufOfs)
                    {
                        // we have bytes in the buffer, so copy them first
                        int temp = _readBufLen - _readBufOfs;
                        if (temp > count)
                            temp = count;
                        if (bitDepth == 32)
                            Buffer.BlockCopy(_readBuf, _readBufOfs, buffer, index, temp);
                        else
                        {
                            for (int i = 0; i < temp / sizeof(float); i++)
                            {
                                switch (bitDepth)
                                {
                                    case 8:
                                        buffer.SetValue(
                                            (byte)Math.Round((127.5f * _readBuf[(_readBufOfs / sizeof(float)) + i]) +
                                                              127.5f), (index / sizeof(float)) + i);
                                        break;
                                    case 16:
                                        int value = (int)Math.Round(
                                            (32767.5f * _readBuf[(_readBufOfs / sizeof(float)) + i]) - 0.5f);
                                        if (value < 0)
                                            value += 65536;

                                        buffer.SetValue((byte)(value % 256), 2 * ((index / sizeof(float)) + i));
                                        buffer.SetValue((byte)(value / 256), (2 * ((index / sizeof(float)) + i)) + 1);

                                        break;
                                }
                            }
                        }

                        // now update our counters...
                        cnt += temp;

                        count -= temp;
                        index += temp;

                        _position += temp;
                        _readBufOfs += temp;

                        // finally, mark the buffer as empty if we've read everything in it
                        if (_readBufOfs == _readBufLen)
                            _readBufLen = 0;
                    }

                    // if the buffer is empty, try to fill it
                    //  NB: If we've already satisfied the read request, we'll still try to fill the buffer.
                    //      This ensures there's data in the pipe on the next call
                    if (_readBufLen == 0)
                    {
                        if (_eofFound)
                            break;

                        // decode the next frame (update _readBufXXX)
                        MpegFrame frame = _reader.NextFrame();
                        if (frame == null)
                        {
                            _eofFound = true;
                            break;
                        }

                        try
                        {
                            _readBufLen = _decoder.DecodeFrame(frame, _readBuf, 0) * sizeof(float);
                            _readBufOfs = 0;
                        }
                        catch (InvalidDataException)
                        {
                            // bad frame...  try again...
                            _decoder.Reset();
                            _readBufOfs = _readBufLen = 0;
                        }
                        catch (EndOfStreamException)
                        {
                            // no more frames
                            _eofFound = true;
                            break;
                        }
                        finally
                        {
                            frame.ClearBuffer();
                        }
                    }
                }
            }

            return cnt;
        }
    }
}