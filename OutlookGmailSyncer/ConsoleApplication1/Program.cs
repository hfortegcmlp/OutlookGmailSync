using System.Collections.Generic;
using CommandLine;

namespace Syncer
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<FetchAndUploadToFtpOptions, UploadToGoogleOptions>(args)
                .WithParsed<FetchAndUploadToFtpOptions>(RefreshOutlookData)
                .WithParsed<UploadToGoogleOptions>(RefreshGoogleCalendar);
            //    .MapResult(
            //        (FetchAndUploadToFtpOptions opts) => RefreshGoogleCalendar(opts),
            //        (CommitOptions opts) => RunCommitAndReturnExitCode(opts),
            //        (CloneOptions opts) => RunCloneAndReturnExitCode(opts),
            //        errs => 1);


            //RefreshOutlookData();
        }

        private static void RefreshGoogleCalendar(UploadToGoogleOptions options)
        {
            //GoogleCalendar.RemoveEventsFromWorkCalendar();
            GoogleCalendar.AddEventsToWorkCalendar(SettingsHelper.LocalIcalPath);
        }

        static void RefreshOutlookData(FetchAndUploadToFtpOptions options)
        {
            OutlookExporter.ExportIcal(SettingsHelper.LocalIcalPath);
            FtpHelper.Upload(options);
        }
    }
}