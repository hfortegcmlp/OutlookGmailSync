using OutlookExporter;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var localPath = "c:\\ical.ical";
            var ftpPath = "ical.ical";
            OutlookExporter.OutlookExporter.ExportIcal(localPath);
            FtpHelper.Upload(localPath, ftpPath);
            FtpHelper.Download(ftpPath, "c:\\SomeFile.ical");
        }
    }
}
