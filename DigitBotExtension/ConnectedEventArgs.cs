namespace DigiBotExtension
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ConnectedEventArgs : EventArgs
    {
        public string Channel { get; private set; }
        public string Server { get; private set; }
        public ConnectedEventArgs(string server, string channel)
        {
            this.Server = server;
            this.Channel = channel;
        }
    }
}
