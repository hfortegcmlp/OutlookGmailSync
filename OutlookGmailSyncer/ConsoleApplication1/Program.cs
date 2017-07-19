using Syncer;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            RefreshGoogleCalendar();
        }

        private static void RefreshGoogleCalendar()
        {
            GoogleCalendar.DoStuff();
        }

        static void RefreshOutlookData()
        {
            var localPath = "c:\\ical.ical";
            var ftpPath = "ical.ical";
            OutlookExporter.ExportIcal(localPath);
            FtpHelper.Upload(localPath, ftpPath);
            //FtpHelper.Download(ftpPath, "c:\\SomeFile.ical");
        }
    }
}
