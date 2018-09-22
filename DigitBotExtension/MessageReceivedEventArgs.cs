namespace DigiBotExtension
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Event Args for OnMessageRecieved event in <see cref="IIrcConnection"/>.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        private string[] line;
        private string channel;

        /// <summary>
        /// Gets or sets line of text recieved.
        /// </summary>
        public string[] Line { get => this.line; set => this.line = value ?? throw new ArgumentNullException(nameof(value)); }

        /// <summary>
        /// Gets or sets the origin channel for this message.
        /// </summary>
        public string Channel { get => this.channel; set => this.channel = value ?? throw new ArgumentNullException(nameof(value)); }
    }
}
