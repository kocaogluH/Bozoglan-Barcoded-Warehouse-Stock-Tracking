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

        public FrmSettings()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Ayarlar";
            this.BackColor = UiTheme.MainBackground;
            this.Size = new Size(500, 580);
            this.FormBorderStyle = FormBorderStyle.None;

            int leftMargin = 30;
            int fieldWidth = 400;

            // ── Başlık ──
            var lblTitle = new Label
            {
                Text = "⚙  Hesap Ayarları",
                ForeColor = UiTheme.Primary,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(leftMargin, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            var divider = new Label
            {
                BackColor = UiTheme.GridLine,
                Size = new Size(fieldWidth, 1),
                Location = new Point(leftMargin, 60)
            };
            Controls.Add(divider);

            // ── Kullanıcı Adı Bölümü ──
            var lblUserSection = new Label
            {
                Text = "Kullanıcı Adı Değiştir",
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(leftMargin, 80),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(lblUserSection);

            txtUsername = new Guna2TextBox
            {
                PlaceholderText = "Yeni Kullanıcı Adı",
                Text = Session.Username ?? "",
                Location = new Point(leftMargin, 110),
                Size = new Size(fieldWidth, 42),
                BorderRadius = 10,
                FillColor = UiTheme.InputFill,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder,
                Font = new Font("Segoe UI", 11),
                PlaceholderForeColor = UiTheme.TextMuted
            };
            Controls.Add(txtUsername);

            btnSaveUsername = new Guna2Button
            {
                Text = "Kullanıcı Adını Güncelle",
                Location = new Point(leftMargin, 162),
                Size = new Size(fieldWidth, 42),
                BorderRadius = 10,
                FillColor = UiTheme.Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnSaveUsername.HoverState.FillColor = ControlPaint.Dark(UiTheme.Primary, 0.08f);
            btnSaveUsername.Click += BtnSaveUsername_Click;
            Controls.Add(btnSaveUsername);

            // ── Şifre Bölümü ──
            var divider2 = new Label
            {
                BackColor = UiTheme.GridLine,
                Size = new Size(fieldWidth, 1),
                Location = new Point(leftMargin, 224)
            };
            Controls.Add(divider2);

            var lblPassSection = new Label
            {
                Text = "Şifre Değiştir",
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(leftMargin, 244),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(lblPassSection);

            txtNewPassword = new Guna2TextBox
            {
                PlaceholderText = "Yeni Şifre",
                UseSystemPasswordChar = true,
                Location = new Point(leftMargin, 278),
                Size = new Size(fieldWidth, 42),
                BorderRadius = 10,
                FillColor = UiTheme.InputFill,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder,
                Font = new Font("Segoe UI", 11),
                PlaceholderForeColor = UiTheme.TextMuted
            };
            Controls.Add(txtNewPassword);

            txtConfirmPassword = new Guna2TextBox
            {
                PlaceholderText = "Yeni Şifre (Tekrar)",
                UseSystemPasswordChar = true,
                Location = new Point(leftMargin, 330),
                Size = new Size(fieldWidth, 42),
                BorderRadius = 10,
                FillColor = UiTheme.InputFill,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder,
                Font = new Font("Segoe UI", 11),
                PlaceholderForeColor = UiTheme.TextMuted
            };
            Controls.Add(txtConfirmPassword);

            btnSavePassword = new Guna2Button
            {
                Text = "Şifreyi Güncelle",
                Location = new Point(leftMargin, 386),
                Size = new Size(fieldWidth, 42),
                BorderRadius = 10,
                FillColor = UiTheme.Success,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnSavePassword.HoverState.FillColor = ControlPaint.Dark(UiTheme.Success, 0.08f);
            btnSavePassword.Click += BtnSavePassword_Click;
            Controls.Add(btnSavePassword);

            // ── Güncelleme Bölümü ──
            var divider3 = new Label
            {
                BackColor = UiTheme.GridLine,
                Size = new Size(fieldWidth, 1),
                Location = new Point(leftMargin, 444)
            };
            Controls.Add(divider3);

            var lblUpdateSection = new Label
            {
                Text = "Sistem Güncelleme",
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(leftMargin, 460),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(lblUpdateSection);

            var btnCheckUpdate = new Guna2Button
            {
                Text = "🔄  Güncellemeleri Denetle",
                Location = new Point(leftMargin, 496),
                Size = new Size(fieldWidth, 42),
                BorderRadius = 10,
                FillColor = UiTheme.SidebarSelected,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnCheckUpdate.HoverState.FillColor = ControlPaint.Dark(UiTheme.SidebarSelected, 0.08f);
            btnCheckUpdate.Click += BtnCheckUpdate_Click;
            Controls.Add(btnCheckUpdate);
        }

        private void BtnSaveUsername_Click(object sender, EventArgs e)
        {
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
