using System.Net;

namespace Syncer
{
    public class FtpHelper
    {
        private readonly FtpOptions _options;

        public FtpHelper(FtpOptions options)
        {
            _options = options;
        }
        public void Upload()
        {
            using (WebClient client = GetClient())
            {   
                client.UploadFile($"{_options.FtpAddress}/{SettingsHelper.FtpIcalName}", "STOR", SettingsHelper.LocalIcalPath);
            }
        }
        

        public void Download()
        {
            using (WebClient client = GetClient())
            {
                client.DownloadFile($"{_options.FtpAddress}/{SettingsHelper.FtpIcalName}", SettingsHelper.LocalIcalPath);
            }
        }

        private WebClient GetClient()
        {
            var client = new WebClient();
            client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
            return client;
        }

        
    }

}
