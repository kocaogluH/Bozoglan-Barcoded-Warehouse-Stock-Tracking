using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class FrmChangePassword : Form
    {
        private long _userId;
        private Guna2TextBox txtNewPassword;
        private Guna2TextBox txtConfirmPassword;
        private Guna2Button btnSave;

        public FrmChangePassword(long userId)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Zorunlu Şifre Değiştirme";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UiTheme.MainBackground;

            var lblInfo = new Label
            {
                Text = "Güvenliğiniz için varsayılan şifrenizi (1234) değiştirmeniz gerekmektedir.",
                Location = new Point(20, 20),
                Size = new Size(340, 40),
                ForeColor = UiTheme.Danger,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblInfo);

            txtNewPassword = new Guna2TextBox
            {
                PlaceholderText = "Yeni Şifre",
                UseSystemPasswordChar = true,
                Location = new Point(20, 80),
                Size = new Size(340, 36)
            };
            this.Controls.Add(txtNewPassword);

            txtConfirmPassword = new Guna2TextBox
            {
                PlaceholderText = "Yeni Şifre (Tekrar)",
                UseSystemPasswordChar = true,
                Location = new Point(20, 130),
                Size = new Size(340, 36)
            };
            this.Controls.Add(txtConfirmPassword);

            btnSave = new Guna2Button
            {
                Text = "Şifreyi Kaydet",
                Location = new Point(20, 190),
                Size = new Size(340, 45),
                FillColor = UiTheme.Primary,
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var p1 = txtNewPassword.Text;
            var p2 = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(p1) || p1.Length < 4)
            {
                MessageBox.Show("Şifre en az 4 karakter olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (p1 != p2)
            {
                MessageBox.Show("Şifreler birbiriyle eşleşmiyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Database.UpdatePassword(_userId, p1);
                MessageBox.Show("Şifreniz başarıyla değiştirildi. Yeni şifrenizle devam edebilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
