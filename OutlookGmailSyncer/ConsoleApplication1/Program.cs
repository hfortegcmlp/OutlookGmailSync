using System.Collections.Generic;
using CommandLine;

namespace Syncer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<FetchAndUploadToFtpOptions, UploadToGoogleOptions>(args)
                .WithParsed<FetchAndUploadToFtpOptions>(RefreshOutlookData)
                .WithParsed<UploadToGoogleOptions>(RefreshGoogleCalendar);
        }

        private static void RefreshGoogleCalendar(UploadToGoogleOptions options)
        {
            var ftpHelper = new FtpHelper(options);
            var googleCal = new GoogleCalendar(options);
            ftpHelper.Download();
            googleCal.RemoveEventsFromWorkCalendar();
            googleCal.AddEventsToWorkCalendar(SettingsHelper.LocalIcalPath);
        }

        static void RefreshOutlookData(FetchAndUploadToFtpOptions options)
        {
            var ftpHelper = new FtpHelper(options);
            OutlookExporter.ExportIcal(SettingsHelper.LocalIcalPath);
            ftpHelper.Upload();
        }

    }
}