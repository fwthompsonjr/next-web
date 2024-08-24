using System.Diagnostics;
using System.Text;

namespace next.processor.api.utility
{
    public class FirefoxInstallComandLine
    {

        public void ExecuteCommands()
        {
            var commands = GetCommands();
            commands.ForEach(command => { 
                var process = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "/usr/bin/bash",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = command
                    }
                };
                process.Start();
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                Console.WriteLine(output);
                Console.WriteLine(error);
            });
        }

        private static List<string> GetCommands()
        {
            var installUrl = GetFireFoxDownloadUri();
            var finder = new Dictionary<string, string> {
                { "{{download_uri}}", installUrl },
                { "$FIREFOX_VERSION", _firefoxVersion }
            };
            var list = new List<string>();
            lines.ForEach(line => { 
                var sb = new StringBuilder(line);
                finder.Keys.ToList().ForEach(key
                    => sb.Replace(key, finder[key]));
                list.Add(sb.ToString());
            });
            return list;
        }

        private static readonly List<string> lines = [
            "apt-get update -qqy",
            "apt-get -qqy --no-install-recommends install firefox",
            "rm -rf /var/lib/apt/lists/* /var/cache/apt/*",
            "wget --no-verbose -O /tmp/firefox.tar.bz2 {{download_uri}} && apt-get -y purge firefox",
            "rm -rf /opt/firefox",
            "tar -C /opt -xjf /tmp/firefox.tar.bz2",
            "rm /tmp/firefox.tar.bz2",
            "mv /opt/firefox /opt/firefox-$FIREFOX_VERSION",
            "ln -fs /opt/firefox-$FIREFOX_VERSION/firefox /usr/bin/firefox"
            ];

        private static string GetFireFoxDownloadUri()
        {
            var address = "https://download-installer.cdn.mozilla.net/pub/firefox/releases/{0}" +
                "/linux-x86_64/en-US/firefox-{0}.tar.bz2";
            return string.Format(address, _firefoxVersion);
        }
        private const string _firefoxVersion = "129.0.2";
    }
}
