using CommandLine;
using RemoteSyslogLibrary;

namespace SyslogWriter
{
    public class Options
    {
        [Option('s', "server", Required = true, HelpText = "The IP Address (or hostname) of the Syslog Server to Write To")]
        public string SyslogServerHost { get; set; }

        [Option('p', "port", Required = true, HelpText = "The port that the Syslog Server is listening on (UDP)")]
        public int SyslogServerPort { get; set; }

        [Option('l', "level", Required = true, HelpText = "Valid values are Debug, Info, Warn, Error, Alert, Critical, Emergency")]
        public Severity Level { get; set; }

        [Option('m', "message", Required = true, HelpText = "The message you want to log")]
        public string Message { get; set; }
    }
}
