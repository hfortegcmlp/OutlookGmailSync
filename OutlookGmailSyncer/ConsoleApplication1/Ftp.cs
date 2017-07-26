using System.Net;

namespace Syncer
{
    public static class FtpHelper
    {
        public static void Upload(FetchAndUploadToFtpOptions options)
        {
            using (WebClient client = GetClient(options))
            {   
                client.UploadFile($"{options.FtpAddress}/{SettingsHelper.FtpIcalName}", "STOR", SettingsHelper.LocalIcalPath);
            }
        }
        

        public static void Download(FetchAndUploadToFtpOptions options)
        {
            using (WebClient client = GetClient(options))
            {
                client.DownloadFile($"{options.FtpAddress}/{SettingsHelper.FtpIcalName}", SettingsHelper.LocalIcalPath);
            }
        }

        private static WebClient GetClient(FetchAndUploadToFtpOptions options)
        {
            var client = new WebClient();
            client.Credentials = new NetworkCredential(options.UserName, options.Password);
            return client;
        }

        
    }

}
