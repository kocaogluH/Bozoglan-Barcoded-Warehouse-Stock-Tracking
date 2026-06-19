using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Guna.UI2.WinForms;
using Barcoded_Warehouse_Stock_Tracking.DataAccess;
using Barcoded_Warehouse_Stock_Tracking.Entities;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class FrmCategories : Form
    {
        private DataGridView _dgv;
        private Guna2TextBox _txtName;
        private Guna2Button _btnAdd;
        private Guna2Button _btnDelete;
        
        public FrmCategories()
        {
            this.Text = "Kategoriler";
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            this.BackColor = UiTheme.MainBackground;

            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            var pnlTop = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FillColor = UiTheme.Surface,
                CustomBorderColor = UiTheme.GridLine,
                CustomBorderThickness = new Padding(0, 0, 0, 1),
                Padding = new Padding(20)
            };
            this.Controls.Add(pnlTop);

            var lblTitle = new Label
            {
                Text = "Kategori Yönetimi",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(20, 25),
                BackColor = Color.Transparent
            };
            pnlTop.Controls.Add(lblTitle);

            var pnlInput = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FillColor = UiTheme.MainBackground,
                Padding = new Padding(20)
            };
            this.Controls.Add(pnlInput);

            _txtName = new Guna2TextBox
            {
                PlaceholderText = "Yeni Kategori Adı...",
                Size = new Size(300, 40),
                Location = new Point(20, 20),
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10F),
                FillColor = UiTheme.InputFill,
                ForeColor = UiTheme.TextPrimary,
                BorderColor = UiTheme.InputBorder
            };
            pnlInput.Controls.Add(_txtName);

            _btnAdd = new Guna2Button
            {
                Text = "＋ Ekle",
                Size = new Size(120, 40),
                Location = new Point(330, 20),
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FillColor = UiTheme.Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnAdd.HoverState.FillColor = UiTheme.PrimaryDark;
            _btnAdd.Click += BtnAdd_Click;
            pnlInput.Controls.Add(_btnAdd);

            _btnDelete = new Guna2Button
            {
                Text = "🗑 Sil",
                Size = new Size(120, 40),
                Location = new Point(460, 20),
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FillColor = UiTheme.Danger,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnDelete.HoverState.FillColor = Color.DarkRed;
            _btnDelete.Click += BtnDelete_Click;
            pnlInput.Controls.Add(_btnDelete);

            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = UiTheme.Surface,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 40 }
            };
            
            _dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = UiTheme.GridHeaderBg,
                ForeColor = UiTheme.TextMuted,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = UiTheme.GridHeaderBg
            };
            _dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 10F),
                SelectionBackColor = UiTheme.CardLavender,
                SelectionForeColor = UiTheme.TextPrimary,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            _dgv.SelectionChanged += (s, e) => 
            {
                _btnDelete.Enabled = _dgv.SelectedRows.Count > 0;
            };

            var pnlGrid = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            pnlGrid.Controls.Add(_dgv);
            this.Controls.Add(pnlGrid);

            pnlTop.SendToBack();
            pnlGrid.BringToFront();
        }

        private void LoadData()
        {
            using (var ctx = new WarehouseContext())
            {
                var list = ctx.Categories.OrderBy(c => c.Id).Select(c => new { No = c.Id, Kategori_Adı = c.Name }).ToList();
                _dgv.DataSource = list;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = _txtName.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;

            using (var ctx = new WarehouseContext())
            {
                if (ctx.Categories.Any(c => c.Name.ToLower() == name.ToLower()))
                {
                    MessageBox.Show("Bu kategori zaten mevcut!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ctx.Categories.Add(new Category { Name = name });
                ctx.SaveChanges();
            }

            _txtName.Clear();
            LoadData();

            // Form1'in de güncellenmesi için event tetikleyebiliriz ya da açık sayfaları yenileyebiliriz.
            if (Application.OpenForms["Form1"] is Form1 mainForm)
            {
                mainForm.RefreshCategoriesUI();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;

            long id = Convert.ToInt64(_dgv.SelectedRows[0].Cells["No"].Value);

            if (MessageBox.Show("Seçili kategoriyi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (var ctx = new WarehouseContext())
                {
                    var cat = ctx.Categories.Find(id);
                    if (cat != null)
                    {
                        ctx.Categories.Remove(cat);
                        ctx.SaveChanges();
                    }
                }
                LoadData();
                
                if (Application.OpenForms["Form1"] is Form1 mainForm)
                {
                    mainForm.RefreshCategoriesUI();
                }
            }
        }
    }
}
