// -----------------------------------------------------------------------
// <copyright file="SendingConsoleCommandEventArgs.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.Events.EventArgs
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before sending a console message.
    /// </summary>
    public sealed class SendingConsoleCommandEventArgs : System.EventArgs
    {
        private string _returnMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendingConsoleCommandEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="arguments"><inheritdoc cref="Arguments"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SendingConsoleCommandEventArgs(
            Player player,
            string name,
            List<string> arguments,
            bool isAllowed = true)
        {
            Player = player;
            Name = name;
            Arguments = arguments;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's sending the command.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the command arguments.
        /// </summary>
        public List<string> Arguments { get; }

        /// <summary>
        /// Gets or sets the return message, that will be shown to the user in the console.
        /// </summary>
        public string ReturnMessage
        {
            get => _returnMessage;
            set
            {
                _returnMessage = value;
                Player.SendConsoleMessage(_returnMessage, Color);
            }
        }

        /// <summary>
        /// Gets or sets the color of the return message.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the console command can be sent.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
