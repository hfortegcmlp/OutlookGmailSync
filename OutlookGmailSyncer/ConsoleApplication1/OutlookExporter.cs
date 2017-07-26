using System;
using Microsoft.Office.Interop.Outlook;

namespace Syncer
{
    public static class OutlookExporter
    {
        public static void ExportIcal(string path)
        {
            var oApp = new Application();
            var mapiNamespace = oApp.GetNamespace("MAPI"); ;

            MAPIFolder f = mapiNamespace.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);

            CalendarSharing cs = f.GetCalendarExporter();
            cs.CalendarDetail = OlCalendarDetail.olFullDetails;
            cs.StartDate = DateTime.Now.AddDays(-7);
            cs.EndDate = DateTime.Now.AddMonths(2);
            cs.SaveAsICal(path);
        }
    }

}
