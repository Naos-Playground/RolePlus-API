// -----------------------------------------------------------------------
// <copyright file="CustomMpegMicrophone.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.Audio
{
    using System;
    using System.Collections.Generic;

    using API;

    using Dissonance.Audio.Capture;

    using Exiled.API.Features;

    using NAudio.Wave;

    using NLayer;

    using UnityEngine;

    /// <summary>
    /// A tool to manage virtual microphones.
    /// </summary>
    public class CustomMpegMicrophone : MonoBehaviour, IMicrophoneCapture
    {
        private readonly float[] _frame = new float[980];
        private readonly byte[] _frameBytes = new byte[980 * 4];
        private readonly List<IMicrophoneSubscriber> _subscribers = new();
        private float _elapsedTime;
        private WaveFormat _format = new(44100, 1);

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomMpegMicrophone"/> should be stopped.
        /// </summary>
        public bool Stop { get; set; }

        /// <summary>
        /// Gets or sets <see cref="MpegFile"/> to be used.
        /// </summary>
        public MpegFile File { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomMpegMicrophone"/> is recording.
        /// </summary>
        public bool IsRecording { get; private set; }

        /// <summary>
        /// Gets the latency.
        /// </summary>
        public TimeSpan Latency { get; private set; }

        /// <summary>
        /// Starts the capture.
        /// </summary>
        /// <param name="micName">The name of the microphone.</param>
        /// <returns>The <see cref="WaveFormat"/> which is being used.</returns>
        public WaveFormat StartCapture(string micName)
        {
            if (Stop)
                return null;

            Log.Debug("Starting capture.", Internal.RolePlus.Singleton.Config.ShowDebugMessages);
            Log.Debug($"Mic details: {File.SampleRate}hz ({File.Channels} channels) {File.Duration} Volume: {AudioController.Volume}", Internal.RolePlus.Singleton.Config.ShowDebugMessages);

            AudioController.Comms._capture._network = AudioController.Comms._net;

            File.StereoMode = StereoMode.DownmixToMono;
            _format = new WaveFormat(File.SampleRate, 1);

            IsRecording = true;
            Latency = TimeSpan.Zero;
            return _format;
        }

        /// <summary>
        /// Stops the capture.
        /// </summary>
        public void StopCapture()
        {
            Log.Debug("Stopping capture.", Internal.RolePlus.Singleton.Config.ShowDebugMessages);

            IsRecording = false;

            if (File is null)
                return;

            File.Dispose();
            File = null;
        }

        /// <summary>
        /// Adds a <see cref="IMicrophoneSubscriber"/>.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void Subscribe(IMicrophoneSubscriber listener) => _subscribers.Add(listener);

        /// <summary>
        /// Removes a <see cref="IMicrophoneSubscriber"/>.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        /// <returns><see langword="true"/> if the <paramref name="listener"/> was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool Unsubscribe(IMicrophoneSubscriber listener) => _subscribers.Remove(listener);

        /// <summary>
        /// Updates all the subscribers.
        /// </summary>
        /// <returns><see langword="true"/> if the subscribers have been successfully updated; otherwise, <see langword="false"/>.</returns>
        public bool UpdateSubscribers()
        {
            _elapsedTime += Time.unscaledDeltaTime;

            while (_elapsedTime > 0.022f)
            {
                _elapsedTime -= 0.022f;

                int readLength = File.ReadSamples(_frameBytes, 0, _frameBytes.Length);
                Array.Clear(_frame, 0, _frame.Length);
                Buffer.BlockCopy(_frameBytes, 0, _frame, 0, readLength);

                foreach (IMicrophoneSubscriber subscriber in _subscribers)
                    subscriber.ReceiveMicrophoneData(new ArraySegment<float>(_frame), _format);
            }

            if (Stop)
                return true;

            if (File.Position * File.Channels < File.Length - 9216)
                return false;

            if (AudioController.LoopMusic)
                File.Position = 0;
            else
            {
                Stop = true;
                return true;
            }

            return false;
        }
    }
}