using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class LoginForm : Form
    {
        private readonly Guna2TextBox _txtUser  = new Guna2TextBox();
        private readonly Guna2TextBox _txtPass  = new Guna2TextBox();
        private readonly Guna2Button  _btnLogin = new Guna2Button();
        private readonly Label        _lblError = new Label();

        public LoginForm()
        {
            // ── Form ──────────────────────────────────────────────────────────
            Text            = "Poseidon — Giriş";
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            AutoScaleMode   = AutoScaleMode.None;   // DPI ölçeklemeyi kapat
            Font            = new Font("Segoe UI", 9f);

            // Form: 460 × 660 (sabit, DPI bağımsız)
            ClientSize = new Size(460, 660);

            // Gradient arka plan
            this.Paint += (s, pe) =>
            {
                using (var br = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(10, 25,  77),   // lacivert
                    Color.FromArgb( 3, 82, 143),   // safir mavisi
                    150f))
                    pe.Graphics.FillRectangle(br, this.ClientRectangle);
            };

            // ── Logo ──────────────────────────────────────────────────────────
            const int LS = 110;  // Logo Size
            var pbLogo = new PictureBox
            {
                Size      = new Size(LS, LS),
                Location  = new Point((460 - LS) / 2, 24),  // x=175, y=24
                SizeMode  = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };

            var cp = new GraphicsPath();
            cp.AddEllipse(0, 0, LS, LS);
            pbLogo.Region = new Region(cp);

            pbLogo.Paint += (s, pe) =>
            {
                if (pbLogo.Image == null) return;
                pe.Graphics.SmoothingMode     = SmoothingMode.AntiAlias;
                pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, pbLogo.Width - 1, pbLogo.Height - 1);
                    pe.Graphics.SetClip(path);
                    var img = pbLogo.Image;
                    float sc = Math.Max((float)pbLogo.Width / img.Width, (float)pbLogo.Height / img.Height);
                    float dw = img.Width * sc, dh = img.Height * sc;
                    pe.Graphics.DrawImage(img, (pbLogo.Width - dw) / 2f, (pbLogo.Height - dh) / 2f, dw, dh);
                }
            };

            try
            {
                string exe  = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string proj = System.IO.Path.GetFullPath(System.IO.Path.Combine(exe, "..", ".."));
                foreach (var p in new[]
                {
                    System.IO.Path.Combine(proj, "Resources", "poseidon_logo.png"),
                    System.IO.Path.Combine(exe,  "Resources", "poseidon_logo.png"),
                    System.IO.Path.Combine(exe,  "poseidon_logo.png"),
                    @"C:\Users\Halil Kocaoğlu\OneDrive\Masaüstü\iş\Poseidon Otomasyon&Yazılım\Logo-2--removebg-preview.png"
                })
                { if (System.IO.File.Exists(p)) { pbLogo.Image = Image.FromFile(p); break; } }
            }
            catch { }

            // ── Marka Başlığı  (logo alt = 24+110 = 134) ─────────────────────
            // lblBrand: y=140, h=40  →  alt=180
            var lblBrand = new Label
            {
                Text      = "Poseidon Yazılım",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size      = new Size(460, 40),
                Location  = new Point(0, 140),
                BackColor = Color.Transparent
            };

            // lblSub: y=182, h=22  →  alt=204
            var lblSub = new Label
            {
                Text      = "Depo & Stok Yönetim Sistemi",
                ForeColor = Color.FromArgb(180, 210, 255),
                Font      = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                Size      = new Size(460, 22),
                Location  = new Point(0, 182),
                BackColor = Color.Transparent
            };

            // ── Kart  (lblSub alt=204 → +14 boşluk = 218) ────────────────────
            // Kart: x=40, y=218, w=380, h=370  →  sağ kenar=420, alt=588
            const int CW = 380, CH = 370;
            const int CX = (460 - CW) / 2;  // 40
            const int CY = 218;

            var card = new Guna2Panel
            {
                Size         = new Size(CW, CH),
                Location     = new Point(CX, CY),
                FillColor    = Color.White,
                BorderRadius = 22,
                ShadowDecoration =
                {
                    Enabled      = true,
                    Color        = Color.FromArgb(90, 0, 20, 80),
                    Depth        = 22,
                    BorderRadius = 22,
                    Shadow       = new Padding(10)
                }
            };

            // Kart içi ─ tüm Y değerleri karta görelidir
            // CTRL_W=300, iç yatay başlangıç: (380-300)/2 = 40
            const int TW = 300;
            const int TX = (CW - TW) / 2;   // 40

            // lblTitle: y=28, h=36  →  alt=64
            var lblTitle = new Label
            {
                Text      = "Hesabınıza Giriş Yapın",
                ForeColor = Color.FromArgb(15, 23, 42),
                Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size      = new Size(TW, 36),
                Location  = new Point(TX, 28),
                BackColor = Color.Transparent
            };

            // ince ayırıcı çizgi: y=70, h=1  →  alt=71
            var divider = new Label
            {
                BackColor = Color.FromArgb(226, 232, 240),
                Size      = new Size(TW, 1),
                Location  = new Point(TX, 70)
            };

            // txtUser: y=90, h=50  →  alt=140
            _txtUser.Size               = new Size(TW, 50);
            _txtUser.Location           = new Point(TX, 90);
            _txtUser.PlaceholderText    = "Kullanıcı Adı";
            _txtUser.BorderRadius       = 10;
            _txtUser.FillColor          = Color.FromArgb(241, 245, 249);
            _txtUser.BorderColor        = Color.FromArgb(203, 213, 225);
            _txtUser.ForeColor          = Color.FromArgb(15, 23, 42);
            _txtUser.Font               = new Font("Segoe UI", 11);
            _txtUser.PlaceholderForeColor = Color.FromArgb(148, 163, 184);
            _txtUser.TextOffset         = new Point(10, 0);

            // txtPass: y=154, h=50  →  alt=204  (140+14 boşluk=154)
            _txtPass.Size               = new Size(TW, 50);
            _txtPass.Location           = new Point(TX, 154);
            _txtPass.PlaceholderText    = "Şifre";
            _txtPass.BorderRadius       = 10;
            _txtPass.FillColor          = Color.FromArgb(241, 245, 249);
            _txtPass.BorderColor        = Color.FromArgb(203, 213, 225);
            _txtPass.ForeColor          = Color.FromArgb(15, 23, 42);
            _txtPass.Font               = new Font("Segoe UI", 11);
            _txtPass.UseSystemPasswordChar  = true;
            _txtPass.PlaceholderForeColor   = Color.FromArgb(148, 163, 184);
            _txtPass.TextOffset         = new Point(10, 0);

            // btnLogin: y=224, h=52  →  alt=276  (204+20 boşluk=224)
            _btnLogin.Text              = "SİSTEME GİRİŞ YAP";
            _btnLogin.Size              = new Size(TW, 52);
            _btnLogin.Location          = new Point(TX, 224);
            _btnLogin.BorderRadius      = 10;
            _btnLogin.FillColor         = Color.FromArgb(14, 165, 233);
            _btnLogin.HoverState.FillColor = Color.FromArgb(2, 132, 199);
            _btnLogin.Font              = new Font("Segoe UI", 11, FontStyle.Bold);
            _btnLogin.ForeColor         = Color.White;
            _btnLogin.Cursor            = Cursors.Hand;
            _btnLogin.Animated          = true;
            _btnLogin.Click            += (_, __) => DoLogin();

            // lblError: y=292, h=22  →  alt=314  (276+16 boşluk=292)
            // Kart yüksekliği 370  →  314 < 370 ✓
            _lblError.Size      = new Size(TW, 22);
            _lblError.Location  = new Point(TX, 292);
            _lblError.ForeColor = UiTheme.Danger;
            _lblError.TextAlign = ContentAlignment.MiddleCenter;
            _lblError.Font      = new Font("Segoe UI", 9);
            _lblError.Text      = "";
            _lblError.BackColor = Color.Transparent;

            card.Controls.Add(lblTitle);
            card.Controls.Add(divider);
            card.Controls.Add(_txtUser);
            card.Controls.Add(_txtPass);
            card.Controls.Add(_btnLogin);
            card.Controls.Add(_lblError);

            // ── Telif  (kart alt=218+370=588 → +14 = 602) ────────────────────
            // lblCopy: y=604, h=22  →  alt=626 < 660 ✓
            var lblCopy = new Label
            {
                Text      = "© 2026 Poseidon Yazılım — Software & Automation",
                ForeColor = Color.FromArgb(150, 185, 225),
                Font      = new Font("Segoe UI", 8),
                TextAlign = ContentAlignment.MiddleCenter,
                Size      = new Size(460, 22),
                Location  = new Point(0, 604),
                BackColor = Color.Transparent
            };

            // Controls.Add sırası: sonra eklenen ÜSTE çıkar.
            Controls.Add(card);
            Controls.Add(lblCopy);
            Controls.Add(lblSub);
            Controls.Add(lblBrand);
            Controls.Add(pbLogo);   // en üstte

            AcceptButton = _btnLogin;
        }

        private void DoLogin()
        {
            var user = _txtUser.Text.Trim();
            var pass = _txtPass.Text;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                _lblError.Text = "Lütfen tüm alanları doldurun.";
                return;
            }

            try
            {
                var result = Database.AuthenticateUser(user, pass);
                if (result.HasValue)
                {
                    Session.UserId   = result.Value.Id;
                    Session.Username = user;
                    Session.Role     = result.Value.Role;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    _lblError.Text = "Kullanıcı adı veya şifre hatalı!";
                    _txtPass.Clear();
                    _txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = "Giriş sırasında hata oluştu.";
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}