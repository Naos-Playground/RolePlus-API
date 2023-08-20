// -----------------------------------------------------------------------
// <copyright file="HudBehaviour.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomHud
{
    using System.Collections.Generic;
    using System.Linq;

    using Hints;
    using RolePlus.ExternModule.API.Engine.Framework;
    using RolePlus.ExternModule.API.Enums;

    /// <summary>
    /// Defines the hints behaviour on a player's display.
    /// </summary>
    public sealed class HudBehaviour : EBehaviour
    {
        private const string HUD_FORMAT = "<line-height=95%><voffset=8.5em><alpha=#ff>\n\n\n<align=center>{0}{1}{2}{3}{4}</align>";

        private object[] _toFormat;
        private string _hint;

        /// <summary>
        /// Gets all attached <see cref="Display"/> instances.
        /// </summary>
        public SortedList<DisplayLocation, Display> Displays { get; } = new()
        {
            [DisplayLocation.Top] = new(),
            [DisplayLocation.MiddleTop] = new(),
            [DisplayLocation.Middle] = new(),
            [DisplayLocation.MiddleBottom] = new(),
            [DisplayLocation.Bottom] = new(),
        };

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            FixedTickRate = 1f;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            UpdateHints();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            foreach (Display display in Displays.Values)
                display.Destroy();
        }

        private void UpdateHints()
        {
            _toFormat = Displays.Values.Select(display => FormatStringForHud(display.Content ?? string.Empty, 6)).ToArray<object>();
            _hint = string.Format(HUD_FORMAT, _toFormat);
            Owner.HintDisplay.Show(new TextHint(_hint, new HintParameter[] { new StringHintParameter(_hint) }, durationScalar: 2));
        }

        private string FormatStringForHud(string text, int needNewLine)
        {
            int curNewLine = text.Count(x => x == '\n');
            for (int i = 0; i < needNewLine - curNewLine; i++)
                text += '\n';

            return text;
        }
    }
}
