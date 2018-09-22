namespace TwitchConnection
{

    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Threading;
    using DigiBotExtension;
    using Avalonia.Controls;

    [IrcConnection("Twitch")]
    public class TwitchConnection 
    {
        public int RetryLimit { get; set; } = 3;
        public Action<StreamWriter, IEnumerable<string>> Pong { get; set; }
        public string Channel { get; set; }
        public Action<StreamWriter> Logon { get; set; }
        public bool Connected { get; private set; }

        public Control ConfigBox => null;

        private TcpClient irc;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private bool isStarted = false;
        private ConcurrentQueue<string> queuedMessages = new ConcurrentQueue<string>();
        private CancellationTokenSource source = new CancellationTokenSource();

        public async Task Start()
        {
            if (!isStarted)
            {
                using (irc = new TcpClient(AddressFamily.InterNetwork))
                {
                    bool retry = true;
                    int tryCount = 1;
                    do
                    {
                        try
                        {
                            await irc.ConnectAsync("irc.chat.twitch.tv", 6667).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            Serilog.Log.Error(e, "Connection failed");
                            Thread.Sleep(1000);
                            retry = tryCount++ <= RetryLimit;
                        }
                    } while (retry && !irc.Connected);
                    if (!irc.Connected)
                    {
                        return;
                    }
                    using (stream = irc.GetStream())
                    using (reader = new StreamReader(stream))
                    using (writer = new StreamWriter(stream))
                    {

                        isStarted = true;
                        OnConnected?.Invoke(this, new ConnectedEventArgs("irc.chat.twitch.tv:6667", Channel));
                        await RunLoop(source.Token).ConfigureAwait(false);
                    }
                }
                OnDisconnect?.Invoke(this);
            }
        }

        public Task Stop()
        {
            source.Cancel();
            return Task.CompletedTask;
        }

        public void WriteLine(object o)
        {
            if (!isStarted)
            {
                queuedMessages.Enqueue(o.ToString());
                return;
            }
        }

        public void WriteLine(string format, params object[] os)
        {
            if (!isStarted)
            {
                queuedMessages.Enqueue(String.Format(format, os));
                return;
            }
        }

        public void DefaultPong(StreamWriter writer, IEnumerable<string> args)
        {
            writer.WriteLine(args.First());
        }

        private async Task RunLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                string line = await reader.ReadLineAsync().ConfigureAwait(false);
                OnMessageReceived?.Invoke(this, new MessageReceivedEventArgs { Line = new[] { line }, Channel = Channel });
            }
            return;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public event MessageReceived OnMessageReceived;
        public event ConnectedEvent OnConnected;
        public event DisconnectEvent OnDisconnect;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    source.Cancel();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TwitchConnection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
