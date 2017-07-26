using CommandLine;

namespace Syncer
{
    public class FtpOptions
    {
        [Option('f', "ftp", Required = true, HelpText = "ftp site")]
        public string FtpAddress { get; set; }

        [Option('u', "ftp user", Required = true, HelpText = "ftp site user")]
        public string UserName { get; set; }

        [Option('p', "ftp password", Required = true, HelpText = "ftp site password")]
        public string Password { get; set; }
    }
}