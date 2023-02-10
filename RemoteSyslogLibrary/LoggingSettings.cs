namespace RemoteSyslogLibrary
{
    public class LoggingSettings
    {
        #region Public-Members
        
        /// <summary>
        /// Minimum severity required to send a message.
        /// </summary>
        public Severity MinimumSeverity { get; set; } = Severity.Debug;


        /// <summary>
        /// Maximum message length.  Must be greater than or equal to 32.  Default is 1024.
        /// </summary>
        public int MaxMessageLength
        {
            get => _maxMessageLength;
            set
            {
                if (value < 32)
                {
                    throw new ArgumentException("Maximum message length must be at least 32.");
                }
                _maxMessageLength = value;
            }
        }

        #endregion

        #region Private-Members

        private int _maxMessageLength = 1024;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public LoggingSettings()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
