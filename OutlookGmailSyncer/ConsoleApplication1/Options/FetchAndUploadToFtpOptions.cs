using CommandLine;

namespace Syncer
{
    [Verb("FetchAndUpload", HelpText = "Fetch from outlook and upload to ftp")]
    public class FetchAndUploadToFtpOptions : FtpOptions
    {
    }
}