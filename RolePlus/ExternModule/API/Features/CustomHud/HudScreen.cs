// -----------------------------------------------------------------------
// <copyright file="HudScreen.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomHud
{
    using System.Collections.Generic;

    using MEC;

    /// <summary>
    /// Represents a display part of a <see cref="HudManager"/>.
    /// </summary>
    public class HudScreen
    {
        private readonly Queue<Hint> _queue = new();
        private readonly CoroutineHandle _coroutineHandle;
        private bool _breakNextFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="HudScreen"/> class.
        /// </summary>
        public HudScreen() => _coroutineHandle = Timing.RunCoroutine(HandleDequeue());

        /// <summary>
        /// Gets or sets the content to be displayed.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Enqueues a hint.
        /// </summary>
        /// <param name="content">The content of the hint.</param>
        /// <param name="duration">The duration of the hint.</param>
        /// <param name="overrideQueue">Whether the queue should be cleared before adding the hint.</param>
        public void Enqueue(string content, float duration, bool overrideQueue)
        {
            Hint hint = new(content, duration);
            if (overrideQueue)
            {
                _queue.Clear();
                _breakNextFrame = true;
            }

            _queue.Enqueue(hint);
        }

        /// <summary>
        /// Kills the coroutine that handles the display.
        /// </summary>
        public void Kill() => Timing.KillCoroutines(_coroutineHandle);

        private IEnumerator<float> HandleDequeue()
        {
            while (true)
            {
                if (_queue.TryDequeue(out Hint hint))
                {
                    _breakNextFrame = false;
                    Content = hint.Content;
                    for (int i = 0; i < 50 * hint.Duration; i++)
                    {
                        if (_breakNextFrame)
                        {
                            _breakNextFrame = false;
                            break;
                        }

                        yield return Timing.WaitForOneFrame;
                    }

                    Content = string.Empty;
                    continue;
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
