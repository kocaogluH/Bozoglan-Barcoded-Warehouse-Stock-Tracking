using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public class FrmReports : Form
    {
        private static readonly Color BgDark = UiTheme.MainBackground;
        private static readonly Color BgMid = UiTheme.Surface;
        private static readonly Color BgInput = UiTheme.InputFill;
        private static readonly Color Accent = UiTheme.Primary;
        private static readonly Color AccentBlu = UiTheme.PrimaryDark;
        private static readonly Color AccentGrn = UiTheme.Success;
        private static readonly Color TextMain = UiTheme.TextPrimary;
        private static readonly Color TextDim = UiTheme.TextMuted;

        private readonly TabControl _tabs = new TabControl();
        private readonly DateTimePicker _dtFrom = new DateTimePicker();
        private readonly DateTimePicker _dtTo = new DateTimePicker();
        private readonly Guna2Button _btnRefreshSales = new Guna2Button();
        private readonly Guna2DataGridView _gridDaily = new Guna2DataGridView();
        private readonly Guna2DataGridView _gridProducts = new Guna2DataGridView();
        private readonly NumericUpDown _nudThreshold = new NumericUpDown();
        private readonly Guna2Button _btnRefreshStock = new Guna2Button();
        private readonly Guna2DataGridView _gridLowStock = new Guna2DataGridView();
        private readonly Guna2Button _btnRefreshCust = new Guna2Button();
        private readonly Guna2DataGridView _gridCustomers = new Guna2DataGridView();
        private readonly Guna2Button _btnBackup = new Guna2Button();

        // ── Grid stilini uygula ────────────────────────────────────────────────
        private static void StyleGrid(Guna2DataGridView g, Color _, Color accent)
        {
            g.AllowUserToAddRows = false;
            g.ReadOnly = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoGenerateColumns = true;
            g.Dock = DockStyle.Fill;
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = UiTheme.Surface;
            g.GridColor = UiTheme.GridLine;

            g.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            g.ThemeStyle.BackColor = UiTheme.Surface;
            g.ThemeStyle.GridColor = UiTheme.GridLine;
            
            g.ThemeStyle.HeaderStyle.BackColor = UiTheme.GridHeaderBg;
            g.ThemeStyle.HeaderStyle.ForeColor = accent;
            g.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            
            g.ThemeStyle.RowsStyle.BackColor = UiTheme.Surface;
            g.ThemeStyle.RowsStyle.ForeColor = TextMain;
            g.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10);
            g.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
            g.ThemeStyle.RowsStyle.SelectionForeColor = UiTheme.TextPrimary;

            g.ThemeStyle.AlternatingRowsStyle.BackColor = UiTheme.GridRowAlt;
            g.ThemeStyle.AlternatingRowsStyle.ForeColor = TextMain;

            // ── Satır & Başlık Boyutları ────────────────────────────────────
            g.RowTemplate.Height = 40;
            g.ColumnHeadersHeight = 45;
            
            g.EnableHeadersVisualStyles = false;
            g.RowHeadersVisible = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Paint ghosting sorununu önlemek için
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(g, true, null);
        }

        // ── TabPage oluştur ────────────────────────────────────────────────
        private TabPage MakeDarkTab(string title)
        {
            return new TabPage(title) { BackColor = BgDark };
        }

        public FrmReports()
        {
            Text = "Poseidon Yazılım — Raporlar & Yedek";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1080; Height = 700;
            BackColor = BgDark;
            DoubleBuffered = true;

            // ══════════════════════════════════════════════════════════════════
            // TOP PANEL
            // ══════════════════════════════════════════════════════════════════
            var top = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = BgMid,
                Padding = new Padding(15)
            };

            // Tarih Aralığı
            _dtFrom.Format = DateTimePickerFormat.Short;
            _dtFrom.Value = DateTime.Today.AddDays(-7);
            _dtFrom.Location = new Point(15, 30);
            _dtFrom.Width = 130;
            _dtFrom.CalendarForeColor = TextMain;
            _dtFrom.CalendarMonthBackground = UiTheme.Surface;
            _dtFrom.Font = new Font("Segoe UI", 10);

            _dtTo.Format = DateTimePickerFormat.Short;
            _dtTo.Value = DateTime.Today.AddDays(1);
            _dtTo.Location = new Point(160, 30);
            _dtTo.Width = 130;
            _dtTo.Font = new Font("Segoe UI", 10);

            // SATIŞ RAPORLARINI YENİLE butonu
            _btnRefreshSales.Text = "SATIŞ RAPORLARINI YENİLE";
            _btnRefreshSales.Location = new Point(305, 26);
            _btnRefreshSales.Size = new Size(220, 42);
            _btnRefreshSales.BorderRadius = 8;
            _btnRefreshSales.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _btnRefreshSales.FillColor = UiTheme.Primary;
            _btnRefreshSales.ForeColor = Color.White;
            _btnRefreshSales.HoverState.FillColor = UiTheme.PrimaryDark;
            _btnRefreshSales.Click += (_, __) => RefreshSales();

            // VERİTABANI YEDEKLE butonu
            _btnBackup.Text = "VERİTABANI YEDEKLE";
            _btnBackup.Location = new Point(540, 26);
            _btnBackup.Size = new Size(220, 42);
            _btnBackup.BorderRadius = 8;
            _btnBackup.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _btnBackup.FillColor = UiTheme.Sidebar;
            _btnBackup.ForeColor = Color.White;
            _btnBackup.HoverState.FillColor = UiTheme.SidebarHover;
            _btnBackup.Click += (_, __) => BackupDb();

            top.Controls.Add(_dtFrom);
            top.Controls.Add(_dtTo);
            top.Controls.Add(_btnRefreshSales);
            top.Controls.Add(_btnBackup);

            // ══════════════════════════════════════════════════════════════════
            // TAB CONTROL
            // ══════════════════════════════════════════════════════════════════
            _tabs.Dock = DockStyle.Fill;
            _tabs.BackColor = BgDark;
            _tabs.Font = new Font("Segoe UI", 10);

            StyleGrid(_gridDaily, UiTheme.Surface, Accent);
            StyleGrid(_gridProducts, UiTheme.Surface, AccentBlu);
            StyleGrid(_gridLowStock, UiTheme.Surface, Accent);
            StyleGrid(_gridCustomers, UiTheme.Surface, AccentGrn);

            // ── Tab 1: Günlük Satış ─────────────────────────────────────────
            var tabDaily = MakeDarkTab("📅 Günlük Satış");
            tabDaily.Controls.Add(_gridDaily);

            // ── Tab 2: Ürün Satışları ───────────────────────────────────────
            var tabProd = MakeDarkTab("📦 Ürün Satışları");
            tabProd.Controls.Add(_gridProducts);

            // ── Tab 3: Stok Azalanlar ──────────────────────────────────────
            var tabStock = MakeDarkTab("⚠ Stok Azalanlar");
            var stockTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BgMid,
                Padding = new Padding(15)
            };

            var lblThreshold = new Label
            {
                Text = "Eşik Seviyesi:",
                ForeColor = TextDim,
                AutoSize = true,
                Location = new Point(15, 18),
                Font = new Font("Segoe UI", 10)
            };

            _nudThreshold.Location = new Point(125, 14);
            _nudThreshold.Width = 90;
            _nudThreshold.Height = 36;
            _nudThreshold.Minimum = 0;
            _nudThreshold.Maximum = 100000;
            _nudThreshold.Value = 5;
            _nudThreshold.BackColor = BgInput;
            _nudThreshold.ForeColor = TextMain;
            _nudThreshold.Font = new Font("Segoe UI", 10);
            _nudThreshold.BorderStyle = BorderStyle.FixedSingle;

            _btnRefreshStock.Text = "🔄  Yenile";
            _btnRefreshStock.Location = new Point(230, 13);
            _btnRefreshStock.Size = new Size(130, 38);
            _btnRefreshStock.BorderRadius = 8;
            _btnRefreshStock.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _btnRefreshStock.FillColor = Accent;
            _btnRefreshStock.ForeColor = Color.White;
            _btnRefreshStock.HoverState.FillColor = ControlPaint.Dark(Accent, 0.1f);
            _btnRefreshStock.Click += (_, __) => RefreshLowStock();

            stockTop.Controls.Add(lblThreshold);
            stockTop.Controls.Add(_nudThreshold);
            stockTop.Controls.Add(_btnRefreshStock);

            tabStock.Controls.Add(_gridLowStock);
            tabStock.Controls.Add(stockTop);

            // ── Tab 4: Cari Bakiye ──────────────────────────────────────────
            var tabCust = MakeDarkTab("💳 Cari Bakiye");
            var custTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BgMid,
                Padding = new Padding(15)
            };

            _btnRefreshCust.Text = "🔄  Yenile";
            _btnRefreshCust.Location = new Point(15, 13);
            _btnRefreshCust.Size = new Size(130, 38);
            _btnRefreshCust.BorderRadius = 8;
            _btnRefreshCust.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _btnRefreshCust.FillColor = AccentGrn;
            _btnRefreshCust.ForeColor = Color.White;
            _btnRefreshCust.HoverState.FillColor = ControlPaint.Dark(AccentGrn, 0.1f);
            _btnRefreshCust.Click += (_, __) => RefreshCustomers();

            custTop.Controls.Add(_btnRefreshCust);
            tabCust.Controls.Add(_gridCustomers);
            tabCust.Controls.Add(custTop);

            // TabPage'leri ekle
            _tabs.TabPages.Add(tabDaily);
            _tabs.TabPages.Add(tabProd);
            _tabs.TabPages.Add(tabStock);
            _tabs.TabPages.Add(tabCust);

            Controls.Add(_tabs);
            Controls.Add(top);

            RefreshSales();
            RefreshLowStock();
            RefreshCustomers();
        }

        private void RefreshSales()
        {
            var from = _dtFrom.Value.Date;
            var to = _dtTo.Value.Date;
            if (to <= from) to = from.AddDays(1);

            _gridDaily.DataSource = Database.ReportDailySales(from, to);
            _gridProducts.DataSource = Database.ReportProductSales(from, to);

            // Günlük Satış başlıkları
            if (_gridDaily.Columns.Count > 0)
            {
                if (_gridDaily.Columns["Day"] != null) _gridDaily.Columns["Day"].HeaderText = "Tarih";
                if (_gridDaily.Columns["SaleCount"] != null) _gridDaily.Columns["SaleCount"].HeaderText = "Satış Adedi";
                if (_gridDaily.Columns["Total"] != null) _gridDaily.Columns["Total"].HeaderText = "Toplam Tutar";
                
                foreach (DataGridViewColumn col in _gridDaily.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Ürün Satışları başlıkları
            if (_gridProducts.Columns.Count > 0)
            {
                if (_gridProducts.Columns["Barcode"] != null) _gridProducts.Columns["Barcode"].HeaderText = "Barkod";
                if (_gridProducts.Columns["Name"] != null) _gridProducts.Columns["Name"].HeaderText = "Ürün Adı";
                if (_gridProducts.Columns["Qty"] != null) _gridProducts.Columns["Qty"].HeaderText = "Adet";
                if (_gridProducts.Columns["Total"] != null) _gridProducts.Columns["Total"].HeaderText = "Ciro";

                foreach (DataGridViewColumn col in _gridProducts.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void RefreshLowStock()
        {
            _gridLowStock.DataSource = Database.ReportLowStock((int)_nudThreshold.Value);
            if (_gridLowStock.Columns.Count > 0)
            {
                if (_gridLowStock.Columns["Barcode"] != null) _gridLowStock.Columns["Barcode"].HeaderText = "Barkod";
                if (_gridLowStock.Columns["Name"] != null) _gridLowStock.Columns["Name"].HeaderText = "Ürün Adı";
                if (_gridLowStock.Columns["StockQty"] != null) _gridLowStock.Columns["StockQty"].HeaderText = "Mevcut Stok";

                foreach (DataGridViewColumn col in _gridLowStock.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void RefreshCustomers()
        {
            _gridCustomers.DataSource = Database.ReportCustomerBalances();
            if (_gridCustomers.Columns.Count > 0)
            {
                if (_gridCustomers.Columns["Name"] != null) _gridCustomers.Columns["Name"].HeaderText = "Müşteri";
                if (_gridCustomers.Columns["Phone"] != null) _gridCustomers.Columns["Phone"].HeaderText = "Telefon";
                if (_gridCustomers.Columns["Email"] != null) _gridCustomers.Columns["Email"].HeaderText = "E-Posta";
                if (_gridCustomers.Columns["Balance"] != null) _gridCustomers.Columns["Balance"].HeaderText = "Bakiye";

                foreach (DataGridViewColumn col in _gridCustomers.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void BackupDb()
        {
            var src = AppPaths.DatabaseFilePath;
            if (!File.Exists(src))
            {
                MessageBox.Show("DB dosyası bulunamadı: " + src, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Veritabanı Yedeği Kaydet";
                sfd.Filter = "SQLite DB (*.db)|*.db";
                sfd.FileName = $"miyuki-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db";

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                File.Copy(src, sfd.FileName, overwrite: true);
                MessageBox.Show("✔ Yedek alındı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}