using CommandLine;

namespace Syncer
{
    [Verb("UploadToGoogle", HelpText = "Fetch from ftp and upload to ftp")]
    public class UploadToGoogleOptions : FtpOptions
    {
        [Option('c', "calendar name", Required = true, HelpText = "calendar name")]
        public string Calendar { get; set; }
    }
}