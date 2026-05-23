using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class FrmUpdateDownload : Form
    {
        private readonly string _downloadUrl;
        private readonly WebClient _webClient;
        private readonly string _tempFilePath;

        private readonly Guna2ProgressBar _progressBar = new Guna2ProgressBar();
        private readonly Label _lblStatus = new Label();
        private readonly Label _lblProgressPercentage = new Label();
        private readonly Guna2Button _btnCancel = new Guna2Button();

        public FrmUpdateDownload(string downloadUrl, string latestVersion)
        {
            _downloadUrl = downloadUrl;
            _tempFilePath = Path.Combine(Path.GetTempPath(), "Poseidon_Setup_v" + latestVersion + ".exe");
            _webClient = new WebClient();

            // ── Form Kurulumu ──────────────────────────────────────────────────
            this.Text = "Poseidon Güncelleme Sihirbazı";
            this.Size = new Size(420, 260);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UiTheme.MainBackground;
            this.AutoScaleMode = AutoScaleMode.None;
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Gradient Başlık Arka Planı ──
            this.Paint += (s, pe) =>
            {
                using (var br = new LinearGradientBrush(
                    new Rectangle(0, 0, this.ClientSize.Width, 70),
                    Color.FromArgb(10, 25, 77),
                    Color.FromArgb(3, 82, 143),
                    0f))
                {
                    pe.Graphics.FillRectangle(br, new Rectangle(0, 0, this.ClientSize.Width, 70));
                }
            };

            // ── Başlık Etiketi ──
            var lblTitle = new Label
            {
                Text = "🚀  Sistem Güncelleniyor",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            // ── Durum Açıklaması ──
            _lblStatus.Text = "Güncelleme dosyaları indiriliyor, lütfen bekleyin...";
            _lblStatus.ForeColor = UiTheme.TextPrimary;
            _lblStatus.Font = new Font("Segoe UI", 9.5f);
            _lblStatus.Location = new Point(25, 90);
            _lblStatus.Size = new Size(370, 20);
            _lblStatus.BackColor = Color.Transparent;
            Controls.Add(_lblStatus);

            // ── İlerleme Çubuğu (Guna2ProgressBar) ──
            _progressBar.Size = new Size(370, 20);
            _progressBar.Location = new Point(25, 120);
            _progressBar.BorderRadius = 10;
            _progressBar.FillColor = Color.FromArgb(226, 232, 240);
            _progressBar.ProgressColor = UiTheme.Primary;
            _progressBar.ProgressColor2 = Color.FromArgb(2, 132, 199);
            _progressBar.Minimum = 0;
            _progressBar.Maximum = 100;
            _progressBar.Value = 0;
            Controls.Add(_progressBar);

            // ── Yüzde Göstergesi ──
            _lblProgressPercentage.Text = "%0 tamamlandı (0 MB / 0 MB)";
            _lblProgressPercentage.ForeColor = UiTheme.TextMuted;
            _lblProgressPercentage.Font = new Font("Segoe UI", 8.5f);
            _lblProgressPercentage.Location = new Point(25, 145);
            _lblProgressPercentage.Size = new Size(370, 20);
            _lblProgressPercentage.BackColor = Color.Transparent;
            Controls.Add(_lblProgressPercentage);

            // ── İptal Butonu ──
            _btnCancel.Text = "İptal Et";
            _btnCancel.Size = new Size(120, 36);
            _btnCancel.Location = new Point(275, 190);
            _btnCancel.BorderRadius = 8;
            _btnCancel.FillColor = UiTheme.Danger;
            _btnCancel.HoverState.FillColor = ControlPaint.Dark(UiTheme.Danger, 0.08f);
            _btnCancel.ForeColor = Color.White;
            _btnCancel.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.Animated = true;
            _btnCancel.Click += BtnCancel_Click;
            Controls.Add(_btnCancel);

            // WebClient Events
            _webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            _webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            StartDownload();
        }

        private void StartDownload()
        {
            try
            {
                // Geçici eski dosyayı temizle
                if (File.Exists(_tempFilePath))
                {
                    File.Delete(_tempFilePath);
                }

                _webClient.DownloadFileAsync(new Uri(_downloadUrl), _tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("İndirme işlemi başlatılamadı:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _progressBar.Value = e.ProgressPercentage;

            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            
            double mbytesIn = Math.Round(bytesIn / 1024 / 1024, 2);
            double mtotalBytes = Math.Round(totalBytes / 1024 / 1024, 2);

            _lblProgressPercentage.Text = $"%{e.ProgressPercentage} tamamlandı ({mbytesIn} MB / {mtotalBytes} MB)";
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Güncelleme indirme işlemi kullanıcı tarafından iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else if (e.Error != null)
            {
                MessageBox.Show("İndirme sırasında bir hata oluştu:\n" + e.Error.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                _lblStatus.Text = "Yükleme başlatılıyor...";
                _progressBar.Value = 100;
                _progressBar.ProgressColor = UiTheme.Success;
                _progressBar.ProgressColor2 = Color.FromArgb(4, 120, 87);
                
                MessageBox.Show("Güncelleme başarıyla indirildi. Kurulum başlatılacak ve uygulama kapatılacaktır.", 
                    "Güncelleme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    if (File.Exists(_tempFilePath))
                    {
                        Process.Start(_tempFilePath);
                        Application.Exit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kurulum dosyası çalıştırılamadı:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_webClient.IsBusy)
            {
                _webClient.CancelAsync();
            }
            else
            {
                this.Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_webClient.IsBusy)
            {
                _webClient.CancelAsync();
            }
            _webClient.Dispose();
            base.OnClosing(e);
        }
    }
}
