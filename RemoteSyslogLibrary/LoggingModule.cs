using System.Net;
using System.Text;

namespace RemoteSyslogLibrary
{
    public class LoggingModule : IDisposable
    {
        #region Public-Members

        public string ApplicationName => _applicationName;

        /// <summary>
        /// Logging settings.
        /// </summary>
        public LoggingSettings? Settings
        {
            get => _settings;
            set => _settings = value ?? new LoggingSettings();
        }

        /// <summary>
        /// List of syslog servers.
        /// </summary>
        public List<SyslogServer>? Servers
        {
            get => _servers;
            set { _servers = value == null ? new List<SyslogServer>() : DistinctBy(value, s => s.IpPort).ToList(); }
        }

        #endregion

        #region Private-Members

        private bool _disposed;
        private LoggingSettings? _settings = new();
        private string _applicationName = "syslog-writer";
        private List<SyslogServer>? _servers = new() { new SyslogServer("127.0.0.1") };
        private readonly string _hostname = Dns.GetHostName();
        private readonly CancellationTokenSource _tokenSource = new();

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object using localhost syslog (UDP port 514).
        /// </summary>
        public LoggingModule()
        {
        }

        /// <summary>
        /// Instantiate the object using the specified syslog server IP address and UDP port.
        /// </summary>
        /// <param name="serverIp">Server IP address.</param>
        /// <param name="serverPort">Server port number.</param>
        public LoggingModule(
            string serverIp,
            int serverPort)
        {
            if (string.IsNullOrEmpty(serverIp))
            {
                throw new ArgumentNullException(nameof(serverIp));
            }

            if (serverPort < 0)
            {
                throw new ArgumentException("Server port must be zero or greater.");
            }

            _servers = new List<SyslogServer>
            {
                new(serverIp, serverPort)
            };
        }

        /// <summary>
        /// Instantiate the object using a series of servers.
        /// </summary>
        /// <param name="servers">Servers.</param>
        public LoggingModule(
            List<SyslogServer>? servers)
        {
            if (servers == null)
            {
                throw new ArgumentNullException(nameof(servers));
            }

            if (servers.Count < 1)
            {
                throw new ArgumentException("At least one server must be specified.");
            }

            servers = DistinctBy(servers, s => s.IpPort).ToList();

            _servers = servers;
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Tear down the client and dispose of background workers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Send a log message using 'Debug' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Debug(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Debug, msg);
        }

        /// <summary>
        /// Send a log message using 'Info' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Info(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Info, msg);
        }

        /// <summary>
        /// Send a log message using 'Warn' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Warn(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Warn, msg);
        }

        /// <summary>
        /// Send a log message using 'Error' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Error(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Error, msg);
        }

        /// <summary>
        /// Send a log message using 'Alert' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Alert(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Alert, msg);
        }

        /// <summary>
        /// Send a log message using 'Critical' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Critical(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Critical, msg);
        }

        /// <summary>
        /// Send a log message using 'Emergency' severity.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public virtual void Emergency(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            Log(Severity.Emergency, msg);
        }


        /// <summary>
        /// Send a log message using the specified severity.
        /// </summary>
        /// <param name="sev">Severity of the message.</param>
        /// <param name="msg">Message to send.</param>
        public virtual void Log(Severity sev, string msg)
        {
            while (true)
            {
                if (string.IsNullOrEmpty(msg)) return;
                if (_settings != null && sev < _settings.MinimumSeverity) return;

                string currMsg;
                var remainder = "";

                if (_settings != null && msg.Length > _settings.MaxMessageLength)
                {
                    currMsg = msg[.._settings.MaxMessageLength];
                    remainder = msg.Substring(_settings.MaxMessageLength, (msg.Length - _settings.MaxMessageLength));
                }
                else
                {
                    currMsg = msg;
                }


                
               

                //var header = _settings?.HeaderFormat;
                //if (header != null && header.Contains("{sev_int}")) header = header.Replace("{sev_int}", $"{sev}");
                //if (header != null && header.Contains("{ts}")) header = header.Replace("{ts}", DateTime.Now.ToUniversalTime().ToString(_settings?.TimestampFormat));
                //if (header != null && header.Contains("{host}")) header = header.Replace("{host}", _hostname);
                //if (header != null && header.Contains("{thread}")) header = header.Replace("{thread}", Thread.CurrentThread.ManagedThreadId.ToString());
                //if (header != null && header.Contains("{sev}")) header = header.Replace("{sev}", sev.ToString());

                var message = sev switch
                {
                    Severity.Debug =>
                        $"<7>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    Severity.Info =>
                        $"<6>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    Severity.Warn =>
                        $"<4>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    Severity.Error =>
                        $"<3>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    Severity.Alert =>
                        $"<1>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    Severity.Critical =>
                        $"<2>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    Severity.Emergency =>
                        $"<0>{DateTime.UtcNow:O} {_hostname} {ApplicationName}[{Thread.CurrentThread.ManagedThreadId}]: [{sev}] {currMsg}",
                    _ => throw new ArgumentOutOfRangeException(nameof(sev), sev, null)
                };

                //var message = header + " " + currMsg;


                if (_servers is { Count: > 0 })
                {
                    var servers = new List<SyslogServer>(_servers);
                    SendServers(servers, message);
                }

                if (!string.IsNullOrEmpty(remainder))
                {
                    msg = remainder;
                    continue;
                }

                break;
            }
        }

        #endregion

        #region Private-Methods

        /// <summary>
        /// Dispose of the resource.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _tokenSource.Cancel();
            }

            _disposed = true;
        }


        private static void SendServers(List<SyslogServer> servers, string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            var data = Encoding.UTF8.GetBytes(msg);

            foreach (var server in servers)
            {
                lock (server.SendLock)
                {
                    try
                    {
                        server.Udp?.Send(data, data.Length);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        private static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource>? source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            if (source == null)
            {
                yield break;
            }
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        #endregion
    }
}