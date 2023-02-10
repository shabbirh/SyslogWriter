using System.Text;
using CommandLine;
using RemoteSyslogLibrary;

namespace SyslogWriter
{
    internal class Program
    {
        public static LoggingModule? Logger;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

        }

        private static void RunOptions(Options opts)
        {
            var servers = new List<SyslogServer>
            {
                new(opts.SyslogServerHost, opts.SyslogServerPort),
            };

            Logger = new LoggingModule(servers);


            Logger?.Log(opts.Level, opts.Message);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
           
        }
    }
}