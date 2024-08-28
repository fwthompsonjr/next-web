using next.processor.api.interfaces;

namespace next.processor.api.services
{
    public abstract class BaseWebInstall(IWebInstallOperation webInstallOperation) : IWebContainerInstall
    {
        protected readonly IWebInstallOperation _fileSvc = webInstallOperation;


        public string LastErrorMessage { get; protected set; } = string.Empty;
        public abstract Task<bool> InstallAsync();

        protected static string GetFireFoxDownloadUri(string version)
        {
            var address = "https://download-installer.cdn.mozilla.net/pub/firefox/releases/{0}" +
                "/linux-x86_64/en-US/firefox-{0}.tar.bz2";
            return string.Format(address, version);
        }
        protected const string _firefoxVersion = "129.0.2";
        protected const string _driverPath = "https://github.com/mozilla/geckodriver/releases/download/v0.35.0/geckodriver-v0.35.0-linux64.tar.gz";

        public virtual bool IsInstalled { get; protected set; }
    }
}
