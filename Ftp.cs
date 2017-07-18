using System;
using System.Net;
using Microsoft.Office.Interop.Outlook;

namespace OutlookExporter
{
    public static class FtpHelper
    {
        public static string UserName = "hugoforte";
        public static string PassWord = "ruq0ljxn";
        public static string FtpSite = "ftp://ftp.axelforte.com/";
        public static void Upload(string fromPath, string toPath)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(UserName, PassWord);
                client.UploadFile($"{FtpSite}/{toPath}", "STOR", fromPath);
            }
        }

        public static void Download(string fromPath, string toPath)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(UserName, PassWord);
                client.DownloadFile($"{FtpSite}/{fromPath}", toPath);
            }
        }
    }

}
