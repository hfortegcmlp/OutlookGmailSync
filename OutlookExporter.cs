using System;
using Microsoft.Office.Interop.Outlook;

namespace OutlookExporter
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
            cs.StartDate = DateTime.Now.AddMonths(-1);
            cs.EndDate = DateTime.Now;
            cs.SaveAsICal(path);
        }
    }

}
