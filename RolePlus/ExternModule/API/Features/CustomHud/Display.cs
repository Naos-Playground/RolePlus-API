// -----------------------------------------------------------------------
// <copyright file="Display.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomHud
{
    using System.Collections.Generic;
    using Hints;
    using MEC;

    /// <summary>
    /// Represents the display of the <see cref="HudBehaviour"/>.
    /// </summary>
    public class Display
    {
        private readonly Queue<Hint> _queue = new();
        private readonly CoroutineHandle _dequeueHandle;
        private bool _breakNextFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="Display"/> class.
        /// </summary>
        public Display() => _dequeueHandle = Timing.RunCoroutine(HandleDequeue());

        /// <summary>
        /// Gets or sets the content to be displayed.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Enqueues a hint.
        /// </summary>
        /// <param name="content">The content of the hint.</param>
        /// <param name="duration">The duration of the hint.</param>
        /// <param name="overrideQueue">A value indicating whether the queue should be cleared before adding the hint.</param>
        /// <param name="effects">The hint effects to be applied to.</param>
        /// <param name="parameters">The hint parameters.</param>
        public void Enqueue(string content, float duration, bool overrideQueue, HintEffect[] effects = null, HintParameter[] parameters = null)
        {
            Hint hint = new(content, duration, effects, parameters);
            if (overrideQueue)
            {
                _queue.Clear();
                _breakNextFrame = true;
            }

            _queue.Enqueue(hint);
        }

        /// <summary>
        /// Destroys the display.
        /// </summary>
        public void Destroy() => Timing.KillCoroutines(_dequeueHandle);

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
