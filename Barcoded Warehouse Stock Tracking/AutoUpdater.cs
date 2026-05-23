using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class UpdateInfo
    {
        public bool IsUpdateAvailable { get; set; }
        public string LatestVersion { get; set; }
        public string CurrentVersion { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleaseNotes { get; set; }
    }

    public static class AutoUpdater
    {
        private const string RepoOwner = "kocaogluH";
        private const string RepoName = "Barcoded-Warehouse-Stock-Tracking";
        private const string ApiUrl = "https://api.github.com/repos/" + RepoOwner + "/" + RepoName + "/releases/latest";

        static AutoUpdater()
        {
            // GitHub API requires TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Arka planda asenkron olarak en son güncellemeyi denetler.
        /// </summary>
        public static async Task<UpdateInfo> CheckForUpdatesAsync()
        {
            return await Task.Run(() =>
            {
                var info = new UpdateInfo
                {
                    IsUpdateAvailable = false,
                    CurrentVersion = Application.ProductVersion
                };

                try
                {
                    using (var webClient = new WebClient())
                    {
                        // GitHub API requires a User-Agent header
                        webClient.Headers.Add("User-Agent", "Poseidon-Stock-Tracking-Updater");
                        webClient.Headers.Add("Accept", "application/vnd.github.v3+json");

                        string json = webClient.DownloadString(ApiUrl);

                        // Regex ile JSON ayrıştırma (Harici bağımlılıkları önlemek için)
                        var tagMatch = Regex.Match(json, @"\""tag_name\""\s*:\s*\""([^\""]+)\""");
                        var urlMatch = Regex.Match(json, @"\""browser_download_url\""\s*:\s*\""([^\""]+\.exe)\""");
                        var bodyMatch = Regex.Match(json, @"\""body\""\s*:\s*\""([^\""]+)\""");

                        if (tagMatch.Success && urlMatch.Success)
                        {
                            string latestVersionStr = tagMatch.Groups[1].Value;
                            string downloadUrl = urlMatch.Groups[1].Value;
                            string releaseNotes = bodyMatch.Success ? bodyMatch.Groups[1].Value : "";

                            // Unicode karakter kaçışlarını çöz (Örn: \r\n, \u003c vb.)
                            releaseNotes = Regex.Unescape(releaseNotes);

                            info.LatestVersion = latestVersionStr;
                            info.DownloadUrl = downloadUrl;
                            info.ReleaseNotes = releaseNotes;

                            // Sürüm karşılaştırması
                            string cleanLatest = Regex.Replace(latestVersionStr, @"^[^\d]+", ""); // "v1.0.1" -> "1.0.1"
                            string cleanCurrent = Regex.Replace(info.CurrentVersion, @"^[^\d]+", "");

                            if (Version.TryParse(cleanLatest, out Version latestVer) &&
                                Version.TryParse(cleanCurrent, out Version currentVer))
                            {
                                if (latestVer > currentVer)
                                {
                                    info.IsUpdateAvailable = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Güncelleme kontrolü sırasında hata: " + ex.Message);
                }

                return info;
            });
        }
    }
}
