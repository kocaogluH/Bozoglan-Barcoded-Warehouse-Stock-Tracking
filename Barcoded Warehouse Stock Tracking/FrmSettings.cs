using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class FrmSettings : Form
    {
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtNewPassword;
        private Guna2TextBox txtConfirmPassword;
        private Guna2Button btnSaveUsername;
        private Guna2Button btnSavePassword;
        private Guna2Button btnCheckUpdate;

        public FrmSettings()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Ayarlar";
            this.BackColor = UiTheme.MainBackground;
            this.Size = new Size(900, 650);
            this.FormBorderStyle = FormBorderStyle.None;

            // ── Başlık ──
            var lblTitle = new Label
            {
                Text = "⚙  Sistem ve Hesap Ayarları",
                ForeColor = UiTheme.Sidebar,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(40, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            var divider = new Label
            {
                BackColor = UiTheme.GridLine,
                Size = new Size(820, 1),
                Location = new Point(40, 75)
            };
            Controls.Add(divider);

            // ── SOL KART: Kullanıcı Adı Güncelleme ──
            var pnlUserCard = new Guna2Panel
            {
                Size = new Size(390, 260),
                Location = new Point(40, 100),
                FillColor = Color.White,
                BorderRadius = 16,
                ShadowDecoration = {
                    Enabled = true,
                    Color = Color.FromArgb(40, 0, 0, 0),
                    Depth = 12,
                    BorderRadius = 16
                }
            };
            Controls.Add(pnlUserCard);

            var lblUserSection = new Label
            {
                Text = "Kullanıcı Adı Değiştir",
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlUserCard.Controls.Add(lblUserSection);

            var lblUserDesc = new Label
            {
                Text = "Sistemde görünen giriş kullanıcı adınızı güncelleyin.",
                ForeColor = UiTheme.TextMuted,
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 48),
                Size = new Size(350, 35),
                BackColor = Color.Transparent
            };
            pnlUserCard.Controls.Add(lblUserDesc);

            txtUsername = new Guna2TextBox
            {
                PlaceholderText = "Yeni Kullanıcı Adı",
                Text = Session.Username ?? "",
                Location = new Point(20, 95),
                Size = new Size(350, 45),
                BorderRadius = 10,
                FillColor = UiTheme.SurfaceMuted,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder,
                Font = new Font("Segoe UI", 10.5f),
                PlaceholderForeColor = UiTheme.TextMuted,
                TextOffset = new Point(10, 0)
            };
            pnlUserCard.Controls.Add(txtUsername);

            btnSaveUsername = new Guna2Button
            {
                Text = "Kullanıcı Adını Güncelle",
                Location = new Point(20, 180),
                Size = new Size(350, 45),
                BorderRadius = 10,
                FillColor = UiTheme.Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnSaveUsername.HoverState.FillColor = ControlPaint.Dark(UiTheme.Primary, 0.08f);
            btnSaveUsername.Click += BtnSaveUsername_Click;
            pnlUserCard.Controls.Add(btnSaveUsername);


            // ── SAĞ KART: Şifre Değiştirme ──
            var pnlPassCard = new Guna2Panel
            {
                Size = new Size(390, 340),
                Location = new Point(470, 100),
                FillColor = Color.White,
                BorderRadius = 16,
                ShadowDecoration = {
                    Enabled = true,
                    Color = Color.FromArgb(40, 0, 0, 0),
                    Depth = 12,
                    BorderRadius = 16
                }
            };
            Controls.Add(pnlPassCard);

            var lblPassSection = new Label
            {
                Text = "Şifre Değiştir",
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlPassCard.Controls.Add(lblPassSection);

            var lblPassDesc = new Label
            {
                Text = "Hesabınızın güvenliği için güçlü bir şifre belirleyin.",
                ForeColor = UiTheme.TextMuted,
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 48),
                Size = new Size(350, 35),
                BackColor = Color.Transparent
            };
            pnlPassCard.Controls.Add(lblPassDesc);

            txtNewPassword = new Guna2TextBox
            {
                PlaceholderText = "Yeni Şifre",
                UseSystemPasswordChar = true,
                Location = new Point(20, 95),
                Size = new Size(350, 45),
                BorderRadius = 10,
                FillColor = UiTheme.SurfaceMuted,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder,
                Font = new Font("Segoe UI", 10.5f),
                PlaceholderForeColor = UiTheme.TextMuted,
                TextOffset = new Point(10, 0)
            };
            pnlPassCard.Controls.Add(txtNewPassword);

            txtConfirmPassword = new Guna2TextBox
            {
                PlaceholderText = "Yeni Şifre (Tekrar)",
                UseSystemPasswordChar = true,
                Location = new Point(20, 155),
                Size = new Size(350, 45),
                BorderRadius = 10,
                FillColor = UiTheme.SurfaceMuted,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder,
                Font = new Font("Segoe UI", 10.5f),
                PlaceholderForeColor = UiTheme.TextMuted,
                TextOffset = new Point(10, 0)
            };
            pnlPassCard.Controls.Add(txtConfirmPassword);

            btnSavePassword = new Guna2Button
            {
                Text = "Şifreyi Güncelle",
                Location = new Point(20, 260),
                Size = new Size(350, 45),
                BorderRadius = 10,
                FillColor = UiTheme.Success,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnSavePassword.HoverState.FillColor = ControlPaint.Dark(UiTheme.Success, 0.08f);
            btnSavePassword.Click += BtnSavePassword_Click;
            pnlPassCard.Controls.Add(btnSavePassword);


            // ── SİSTEM BÖLÜMÜ (GÜNCELLEME): SOL ALT KART ──
            var pnlSystemCard = new Guna2Panel
            {
                Size = new Size(390, 190),
                Location = new Point(40, 390),
                FillColor = Color.White,
                BorderRadius = 16,
                ShadowDecoration = {
                    Enabled = true,
                    Color = Color.FromArgb(40, 0, 0, 0),
                    Depth = 12,
                    BorderRadius = 16
                }
            };
            Controls.Add(pnlSystemCard);

            var lblSystemSection = new Label
            {
                Text = "Sistem Güncelleme",
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlSystemCard.Controls.Add(lblSystemSection);

            var lblSystemDesc = new Label
            {
                Text = "Uygulamanın en yeni sürümünü denetleyin ve güncellemeleri otomatik yükleyin.",
                ForeColor = UiTheme.TextMuted,
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 42),
                Size = new Size(350, 38),
                BackColor = Color.Transparent
            };
            pnlSystemCard.Controls.Add(lblSystemDesc);

            btnCheckUpdate = new Guna2Button
            {
                Text = "🔄  Güncellemeleri Denetle",
                Location = new Point(20, 110),
                Size = new Size(350, 45),
                BorderRadius = 10,
                FillColor = UiTheme.SidebarSelected,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnCheckUpdate.HoverState.FillColor = ControlPaint.Dark(UiTheme.SidebarSelected, 0.08f);
            btnCheckUpdate.Click += BtnCheckUpdate_Click;
            pnlSystemCard.Controls.Add(btnCheckUpdate);
        }

        private void BtnSaveUsername_Click(object sender, EventArgs e)
        {
            if (!Session.UserId.HasValue)
            {
                MessageBox.Show("Oturum açık değil. Lütfen sistemi yeniden başlatın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newName = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Kullanıcı adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Database.UpdateUsername(Session.UserId.Value, newName);
                Session.Username = newName;
                MessageBox.Show("Kullanıcı adınız başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSavePassword_Click(object sender, EventArgs e)
        {
            if (!Session.UserId.HasValue)
            {
                MessageBox.Show("Oturum açık değil. Lütfen sistemi yeniden başlatın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var p1 = txtNewPassword.Text;
            var p2 = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(p1) || p1.Length < 4)
            {
                MessageBox.Show("Şifre en az 4 karakter olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (p1 != p2)
            {
                MessageBox.Show("Şifreler birbiriyle eşleşmiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Database.UpdatePassword(Session.UserId.Value, p1);
                MessageBox.Show("Şifreniz başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCheckUpdate_Click(object sender, EventArgs e)
        {
            var btn = sender as Guna2Button;
            if (btn != null)
            {
                btn.Enabled = false;
                btn.Text = "🔄  Kontrol Ediliyor...";
            }

            try
            {
                var updateInfo = await AutoUpdater.CheckForUpdatesAsync();
                if (updateInfo != null && updateInfo.IsUpdateAvailable)
                {
                    string notes = string.IsNullOrWhiteSpace(updateInfo.ReleaseNotes) ? "Detay belirtilmemiş." : updateInfo.ReleaseNotes;
                    string msg = $"Poseidon için yeni bir güncelleme mevcut!\n\n" +
                                 $"Mevcut Sürüm: v{updateInfo.CurrentVersion}\n" +
                                 $"Yeni Sürüm: v{updateInfo.LatestVersion}\n\n" +
                                 $"Güncelleme Notları:\n{notes}\n\n" +
                                 $"Güncelleme dosyasını şimdi indirip kurmak istiyor musunuz?";

                    if (MessageBox.Show(msg, "Yeni Güncelleme Mevcut", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        var dlForm = new FrmUpdateDownload(updateInfo.DownloadUrl, updateInfo.LatestVersion);
                        dlForm.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show($"Sisteminiz güncel.\nMevcut Sürüm: v{Application.ProductVersion}", "Güncelleme Kontrolü", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme kontrolü sırasında hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (btn != null)
                {
                    btn.Enabled = true;
                    btn.Text = "🔄  Güncellemeleri Denetle";
                }
            }
        }
    }
}

