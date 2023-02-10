using System.Net.Sockets;

namespace RemoteSyslogLibrary
{
    public class SyslogServer
    {
         #region Public-Members

        /// <summary>
        /// Hostname.
        /// </summary>
        public string Hostname
        {
            get => _hostname;
            set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(Hostname));
                _hostname = value;

                SetUdp();
            }
        }

        /// <summary>
        /// UDP port.
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                if (value < 0) throw new ArgumentException("Port must be zero or greater.");
                _port = value;

                SetUdp();
            }
        }

        /// <summary>
        /// IP:port of the server.
        /// </summary>
        public string IpPort
        {
            get
            {
                return _hostname + ":" + _port;
            }
        }

        #endregion

        #region Private-Members

        internal readonly object SendLock = new object();
        internal UdpClient? Udp;
        private string _hostname = "127.0.0.1";
        private int _port = 514;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public SyslogServer()
        {
        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="hostname">Hostname.</param>
        /// <param name="port">Port.</param>
        public SyslogServer(string hostname = "127.0.0.1", int port = 514)
        {
            Hostname = hostname;
            Port = port;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        private void SetUdp()
        {
            Udp = null;
            Udp = new UdpClient(_hostname, _port);
        }

        #endregion
    }
}
