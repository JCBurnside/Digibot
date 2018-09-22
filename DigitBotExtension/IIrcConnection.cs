namespace DigiBotExtension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public delegate void MessageReceived(object sender, MessageReceivedEventArgs args);
    public delegate void ConnectedEvent(object sender, ConnectedEventArgs args);
    public delegate void DisconnectEvent(object sender);
    public interface IIrcConnection : IDisposable
    {
        int RetryLimit { get; set; }
        Action<StreamWriter,IEnumerable<string>> Pong { get; set; }
        Action<StreamWriter> Logon { get; set; }
        Avalonia.Controls.Control ConfigBox { get; }
        event MessageReceived OnMessageReceived;
        event ConnectedEvent OnConnected;
        event DisconnectEvent OnDisconnect;
        string Channel { get; set; }
        bool Connected { get; }
    
        Task Start();
        Task Stop();
        void WriteLine(object o);
        void WriteLine(string format, params object[] o);
    }
}
