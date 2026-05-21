using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Guna.UI2.WinForms;
using Barcoded_Warehouse_Stock_Tracking.Business;
using Barcoded_Warehouse_Stock_Tracking.DataAccess;
using Barcoded_Warehouse_Stock_Tracking.Entities;   
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;

namespace Barcoded_Warehouse_Stock_Tracking
{
    public partial class Form1 : Form
    {
        private DashboardService _dashboardService;
        private ProductService _productService;
        private WarehouseContext _context;
        
        // Guna Dashboard Cards
        private Guna2Panel pnlTotalProducts;
        private Label lblTotalProductsVal;
        
        private Guna2Panel pnlDailySales;
        private Label lblDailySalesVal;

        private Guna2Panel pnlLowStock;
        private Label lblLowStockVal;

        private Guna2Panel pnlPendingBalance;
        private Label lblPendingBalanceVal;

        private Chart chartTopSellers;

        private Guna2Panel _pnlSidebar;
        private readonly List<Guna2Button> _navButtons = new List<Guna2Button>();
        private Guna2Button _navReports;

        public Form1()
        {
            InitializeComponent();
            _context = new WarehouseContext();
            _dashboardService = new DashboardService(_context);
            _productService = new ProductService(_context);

            this.FormClosed += (s, e) => _context?.Dispose();
            this.FormClosing += (s, e) => Database.BackupDatabase();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                var login = new LoginForm();
                if (login.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
            }

            SetupRoleAccess();
            InitializeDashboardCards();
            SetupModernShell();
            InitializeContextMenu();

            // Veritabanı değiştikçe tüm UI'ı yenile (Satış, İade vb.)
            Database.DataChanged += () => {
                if (this.InvokeRequired) this.Invoke(new Action(RefreshAll));
                else RefreshAll();
            };

            if (cmbType.Items.Count > 0)
                cmbType.SelectedIndex = 0;

            // Events
            txtBarcode.TextChanged += TxtBarcode_TextChanged;
            dgvProducts.RowPrePaint += DgvProducts_RowPrePaint;
            btnReports.Click += BtnReports_Click;
            btnPos.Click += BtnPos_Click;
            btnReturns.Click += BtnReturns_Click;
            btnCustomers.Click += BtnCustomers_Click;

            btnPos.Visible = false;
            btnReturns.Visible = false;
            btnCustomers.Visible = false;
            btnReports.Visible = false;

            // Global Grid Styling
            StyleModernGrid(dgvProducts);
            StyleModernGrid(dgvMovements);

            // Yeni merkezi silme butonu
            var btnDeleteProduct = new Guna2Button
            {
                Text = "🗑  Ürünü Sil",
                Size = new Size(200, 38),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = UiTheme.Danger,
                Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                HoverState = { FillColor = ControlPaint.Dark(UiTheme.Danger, 0.08f) }
            };
            btnDeleteProduct.Click += (s, ev) =>
            {
                if (dgvProducts.CurrentRow == null) return;
                if (!Session.IsAdmin)
                {
                    MessageBox.Show("Bu işlem için yönetici yetkisi gereklidir.", "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var row = dgvProducts.CurrentRow;
                var barcode = row.Cells["Barkod"].Value.ToString();
                var name = row.Cells["Urun"].Value.ToString();

                if (MessageBox.Show($"'{name}' isimli ürünü silmek istediğinize emin misiniz?", "Ürün Sil", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var product = _productService.GetProductByBarcode(barcode);
                    if (product != null)
                    {
                        _productService.DeleteProduct(product.Id);
                        RefreshAll();
                    }
                }
            };
            tabProducts.Controls.Add(btnDeleteProduct);
            void placeDeleteBtn(object __, EventArgs ___) =>
                btnDeleteProduct.Left = tabProducts.ClientSize.Width - btnDeleteProduct.Width - 18;
            tabProducts.Resize += placeDeleteBtn;
            placeDeleteBtn(null, null);

            ApplyLightChromeToDataTabs();

            RefreshAll();
        }

        private void ApplyLightChromeToDataTabs()
        {
            tabProducts.BackColor = UiTheme.MainBackground;
            tabMovements.BackColor = UiTheme.MainBackground;

            foreach (var tb in new[] { txtBarcode, txtName, txtPrice })
            {
                tb.FillColor = UiTheme.InputFill;
                tb.ForeColor = UiTheme.TextPrimary;
                tb.BorderColor = UiTheme.InputBorder;
                tb.PlaceholderForeColor = UiTheme.TextMuted;
            }
            lblBarcode.ForeColor = lblName.ForeColor = lblPrice.ForeColor = UiTheme.TextMuted;
            btnAdd.FillColor = UiTheme.Success;
            btnAdd.HoverState.FillColor = ControlPaint.Dark(UiTheme.Success, 0.08f);

            txtBarcodeMovement.FillColor = UiTheme.InputFill;
            txtBarcodeMovement.ForeColor = UiTheme.TextPrimary;
            txtBarcodeMovement.BorderColor = UiTheme.InputBorder;
            txtBarcodeMovement.PlaceholderForeColor = UiTheme.TextMuted;

            cmbType.FillColor = UiTheme.InputFill;
            cmbType.ForeColor = UiTheme.TextPrimary;
            cmbType.BorderColor = UiTheme.InputBorder;

            nudQuantity.BackColor = UiTheme.InputFill;
            nudQuantity.ForeColor = UiTheme.TextPrimary;
            lblBarcodeMovement.ForeColor = lblQuantity.ForeColor = lblType.ForeColor = UiTheme.TextMuted;
            btnAddMovement.FillColor = UiTheme.Success;
            btnAddMovement.HoverState.FillColor = ControlPaint.Dark(UiTheme.Success, 0.08f);
        }

        private void SetupModernShell()
        {
            BackColor = UiTheme.MainBackground;
            Text = "Poseidon — Depo & Stok";

            var pnlFill = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.MainBackground
            };
            Controls.Remove(tabControl);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Margin = new Padding(0);
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.Multiline = true;
            pnlFill.Controls.Add(tabControl);

            _pnlSidebar = new Guna2Panel
            {
                Dock = DockStyle.Left,
                Width = 236,
                FillColor = UiTheme.Sidebar,
                BorderRadius = 0
            };

            var lblHead = new Label
            {
                Text = "Yönetim Paneli",
                ForeColor = Color.FromArgb(210, 200, 245),
                Font = new System.Drawing.Font("Segoe UI", 8.5f),
                AutoSize = true,
                Location = new Point(20, 18),
                BackColor = Color.Transparent
            };
            _pnlSidebar.Controls.Add(lblHead);

            int y = 52;
            Guna2Button nav(string caption, Action act, bool transientAction = false)
            {
                var b = new Guna2Button
                {
                    Text = caption,
                    Location = new Point(12, y),
                    Size = new Size(212, 44),
                    BorderRadius = 12,
                    FillColor = UiTheme.Sidebar, // Match sidebar color exactly
                    ForeColor = Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10f, FontStyle.Bold),
                    TextAlign = HorizontalAlignment.Left,
                    TextOffset = new Point(14, 0),
                    Cursor = Cursors.Hand,
                    BorderThickness = 0, // Ensure no borders
                    BackColor = Color.Transparent,
                    UseTransparentBackground = true,
                    Animated = true
                };
                b.HoverState.FillColor = UiTheme.SidebarHover;
                b.HoverState.ForeColor = Color.White;
                b.Click += (_, __) =>
                {
                    if (!transientAction)
                        SetNavActive(b);
                    act();
                    if (transientAction && _navButtons.Count > 0)
                    {
                        tabControl.SelectedIndex = 0;
                        SetNavActive(_navButtons[0]);
                    }
                };
                _pnlSidebar.Controls.Add(b);
                _navButtons.Add(b);
                y += 48;
                return b;
            }

            nav("  🏠  Özet", () => { tabControl.SelectedIndex = 0; });
            nav("  🛒  Satış / POS", () => { OpenChildPage("Satış / POS", new FrmPos()); });
            nav("  ↩  İade / İptal", () => { OpenChildPage("İade / İptal", new FrmReturns()); });
            nav("  📦  Ürünler", () => { tabControl.SelectedIndex = 1; });
            nav("  📋  Stok Hareketleri", () => { tabControl.SelectedIndex = 2; });
            nav("  👤  Müşteriler", () => { OpenChildPage("Müşteriler", new FrmCustomers()); });
            _navReports = nav("  📊  Raporlar", () => { OpenChildPage("Raporlar", new FrmReports()); });
            nav("  ⚙  Ayarlar", () =>
            {
                OpenChildPage("Ayarlar", new FrmSettings());
            });

            var lblFoot = new Label
            {
                Text = Session.Username ?? "",
                ForeColor = Color.FromArgb(190, 180, 230),
                Font = new System.Drawing.Font("Segoe UI", 8f),
                AutoSize = false,
                Size = new Size(212, 36),
                Location = new Point(12, _pnlSidebar.Height - 48),
                TextAlign = ContentAlignment.BottomLeft,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.Transparent
            };
            _pnlSidebar.Controls.Add(lblFoot);
            _pnlSidebar.Resize += (_, __) => lblFoot.Top = _pnlSidebar.ClientSize.Height - lblFoot.Height - 12;

            Controls.Add(pnlFill);
            Controls.Add(_pnlSidebar);

            if (_navButtons.Count > 0)
                SetNavActive(_navButtons[0]);

            if (_navReports != null)
                _navReports.Enabled = Session.IsAdmin;

            tabControl.SelectedIndexChanged += (_, __) =>
            {
                if (_navButtons == null || _navButtons.Count < 5) return;
                int i = tabControl.SelectedIndex;
                if (i == 0) SetNavActive(_navButtons[0]);
                else if (i == 1) SetNavActive(_navButtons[3]);
                else if (i == 2) SetNavActive(_navButtons[4]);
            };
        }

        private void SetNavActive(Guna2Button btn)
        {
            foreach (var b in _navButtons)
            {
                b.FillColor = UiTheme.Sidebar;
                b.ForeColor = Color.White;
            }
            if (btn != null)
            {
                btn.FillColor = UiTheme.SidebarSelected;
                btn.ForeColor = Color.White;
            }
        }

        private void OpenChildPage(string title, Form childForm)
        {
            // Eğer admin değilse ve raporlara erişmeye çalışıyorsa engelle
            if (title == "Raporlar" && !Session.IsAdmin)
            {
                MessageBox.Show("Bu işlem için yetkiniz yok.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Önce bu formun daha önce açılıp açılmadığını kontrol et
            foreach (TabPage tp in tabControl.TabPages)
            {
                if (tp.Text == title)
                {
                    tabControl.SelectedTab = tp;
                    return;
                }
            }

            // Yeni bir TabPage oluştur
            var newTab = new TabPage(title) { BackColor = UiTheme.MainBackground };
            
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            
            newTab.Controls.Add(childForm);
            tabControl.TabPages.Add(newTab);
            tabControl.SelectedTab = newTab;
            
            childForm.Show();
            
            // Form kapandığında (eğer form içinde Close() çağrılırsa) tab'ı da kapatabiliriz
            childForm.FormClosed += (s, e) => {
                tabControl.TabPages.Remove(newTab);
                RefreshAll();
            };
        }

        private void StyleModernGrid(DataGridView g)
        {
            g.BackgroundColor = UiTheme.Surface;
            g.BorderStyle = BorderStyle.None;
            g.RowTemplate.Height = 36;
            g.AllowUserToResizeRows = false;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = UiTheme.GridLine;

            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersHeight = 40;
            g.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.GridHeaderBg;
            g.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Primary;
            g.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiTheme.GridHeaderBg;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiTheme.Primary;

            g.DefaultCellStyle.BackColor = UiTheme.Surface;
            g.DefaultCellStyle.ForeColor = UiTheme.TextPrimary;
            g.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10f);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254); // Light Sapphire Selection
            g.DefaultCellStyle.SelectionForeColor = UiTheme.TextPrimary;

            g.AlternatingRowsDefaultCellStyle.BackColor = UiTheme.GridRowAlt;
            g.AlternatingRowsDefaultCellStyle.ForeColor = UiTheme.TextPrimary;
        }

        private void SetupRoleAccess()
        {
            if (!Session.IsAdmin)   
            {
                // Personel yetkileri sınırlama
                btnReports.Enabled = false;
            }
        }

        private void InitializeDashboardCards()
        {
            pnlTotalProducts  = CreateCard("📦 Toplam Ürün", UiTheme.CardBlue, UiTheme.TextMuted, UiTheme.Primary, 20, 20, out lblTotalProductsVal);
            pnlDailySales     = CreateCard("💰 Bugünkü Satış", UiTheme.SuccessSoft, UiTheme.TextMuted, UiTheme.Success, 240, 20, out lblDailySalesVal);
            pnlLowStock       = CreateCard("⚠ Kritik Stok (<5)", UiTheme.DangerSoft, UiTheme.TextMuted, UiTheme.Danger, 460, 20, out lblLowStockVal);
            pnlPendingBalance = CreateCard("📋 Toplam Alacak", Color.FromArgb(255, 247, 237), UiTheme.TextMuted, UiTheme.Warning, 680, 20, out lblPendingBalanceVal);

            // ── TOP SELLERS CHART ────────────────────────────────────────────────
            chartTopSellers = new Chart
            {
                BackColor = UiTheme.Surface,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 20, 10, 10)
            };

            var area = new ChartArea("MainArea") { BackColor = Color.FromArgb(248, 249, 252) };
            area.AxisX.LabelStyle.ForeColor = UiTheme.TextPrimary;
            area.AxisY.LabelStyle.ForeColor = UiTheme.TextMuted;
            area.AxisX.MajorGrid.LineColor = UiTheme.GridLine;
            area.AxisY.MajorGrid.LineColor = UiTheme.GridLine;
            chartTopSellers.ChartAreas.Add(area);

            var series = new Series("Sales")
            {
                ChartType = SeriesChartType.Bar, // Horizontal looks more elegant
                IsValueShownAsLabel = true,
                LabelForeColor = UiTheme.TextMuted,
                Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Regular),
                Palette = ChartColorPalette.None,
                Color = UiTheme.Primary,
                ["BarLabelStyle"] = "Outside"
            };
            chartTopSellers.Series.Add(series);

            // Sleek Axis Styling
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 9);
            area.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 9);
            area.AxisX.LineColor = Color.Transparent;
            area.AxisY.LineColor = Color.Transparent;

            var title = new Title("🔥 En Çok Satan 5 Ürün", Docking.Top, new System.Drawing.Font("Segoe UI", 14, FontStyle.Bold), UiTheme.Primary);
            chartTopSellers.Titles.Add(title);

            // ── RESPONSIVE LAYOUT ENGINE ─────────────────────────────────────────
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(10)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140)); // Fixed height for cards
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Chart fills remaining

            var cardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            for (int i = 0; i < 4; i++) cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            // Adjust card sizes for fluid layout
            foreach (var p in new[] { pnlTotalProducts, pnlDailySales, pnlLowStock, pnlPendingBalance })
            {
                p.Dock = DockStyle.Fill;
                p.Margin = new Padding(10);
            }

            cardLayout.Controls.Add(pnlTotalProducts, 0, 0);
            cardLayout.Controls.Add(pnlDailySales, 1, 0);
            cardLayout.Controls.Add(pnlLowStock, 2, 0);
            cardLayout.Controls.Add(pnlPendingBalance, 3, 0);

            mainLayout.Controls.Add(cardLayout, 0, 0);
            mainLayout.Controls.Add(chartTopSellers, 0, 1);

            var dashTab = new TabPage("🏠  Dashboard");
            dashTab.BackColor = UiTheme.MainBackground;
            dashTab.Controls.Add(mainLayout);

            tabControl.TabPages.Insert(0, dashTab);
            tabControl.SelectedIndex = 0;
        }

        private Guna.UI2.WinForms.Guna2Panel CreateCard(string title, Color bg, Color titleColor, Color valueColor, int x, int y, out Label valLabel)
        {
            var pnl = new Guna.UI2.WinForms.Guna2Panel
            {
                Location = new Point(x, y),
                Size = new Size(200, 110),
                BorderRadius = 20,
                FillColor = bg,
                ShadowDecoration = { Enabled = true, Color = Color.FromArgb(30, UiTheme.Primary.R, UiTheme.Primary.G, UiTheme.Primary.B), Depth = 12, BorderRadius = 20 }
            };

            var lblTitle = new Label
            {
                Text = title,
                ForeColor = titleColor,
                BackColor = Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(14, 12),
                AutoSize = true
            };

            valLabel = new Label
            {
                Text = "0",
                ForeColor = valueColor,
                BackColor = Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 22, FontStyle.Bold),
                Location = new Point(14, 44),
                AutoSize = true
            };

            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(valLabel);
            return pnl;
        }

        private void InitializeContextMenu()
        {
            var menu = new ContextMenuStrip();
            var mnuEdit = new ToolStripMenuItem("Düzenle");
            var mnuDelete = new ToolStripMenuItem("Sil");
            var mnuDetails = new ToolStripMenuItem("Detay Gör");

            mnuEdit.Click += (s, e) => MessageBox.Show("Düzenleme özelliği yakında!");

            mnuDelete.Click += (s, e) =>
            {
                if (!Session.IsAdmin)
                {
                    MessageBox.Show("Bu işlem için yetkiniz yok (Personel yetkisi).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                if (dgvProducts.CurrentRow != null)
                {
                    long id = Convert.ToInt64(dgvProducts.CurrentRow.Cells["No"].Value);
                    _productService.DeleteProduct(id);
                    RefreshAll();
                }
            };

            mnuDetails.Click += (s, e) => MessageBox.Show("Detay özelliği yakında!");

            menu.Items.Add(mnuEdit);
            menu.Items.Add(mnuDelete);
            menu.Items.Add(mnuDetails);

            dgvProducts.ContextMenuStrip = menu;
        }

        private void TxtBarcode_TextChanged(object sender, EventArgs e)
        {
            var term = txtBarcode.Text.Trim();
            if (string.IsNullOrWhiteSpace(term))
            {
                dgvProducts.DataSource = _productService.GetAllActiveProducts()
                    .OrderBy(p => p.Id)
                    .Select(p => new { No = p.Id, Barkod = p.Barcode, Urun = p.Name, Fiyat = p.UnitPrice, Stok = p.StockQty })
                    .ToList();
            }
            else
            {
                var filtered = _productService.SearchProducts(term);
                dgvProducts.DataSource = filtered
                    .OrderBy(p => p.Id)
                    .Select(p => new { No = p.Id, Barkod = p.Barcode, Urun = p.Name, Fiyat = p.UnitPrice, Stok = p.StockQty })
                    .ToList();

                var exact = _productService.GetProductByBarcode(term);
                if (exact != null)
                {
                    txtName.Text = exact.Name;
                    txtPrice.Text = exact.UnitPrice.ToString("0.00");
                }
            }
        }

        private void DgvProducts_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (dgvProducts.Rows[e.RowIndex].Cells["Stok"].Value != null)
            {
                int stock = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["Stok"].Value);
                if (stock < 5)
                {
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.BackColor = UiTheme.DangerSoft;
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.ForeColor = UiTheme.Danger;
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = UiTheme.TextPrimary;
                }
                else
                {
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? UiTheme.Surface : UiTheme.GridRowAlt;
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.ForeColor = UiTheme.TextPrimary;
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = UiTheme.CardLavender;
                    dgvProducts.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = UiTheme.TextPrimary;
                }
            }
        }

        private void RefreshAll()
        {
            // EF DbContext cache'ini temizleyip veritabanından en güncel (raw sql ile güncellenmiş) verileri çekmesi için yeniden yükle
            _context?.Dispose();
            _context = new WarehouseContext();
            _productService = new ProductService(_context);
            _dashboardService = new DashboardService(_context);

            // DataGridView güncellemesi
            dgvProducts.DataSource = _productService.GetAllActiveProducts()
                .OrderBy(p => p.Id)
                .Select(p => new { No = p.Id, Barkod = p.Barcode, Urun = p.Name, Fiyat = p.UnitPrice, Stok = p.StockQty })
                .ToList();
            
            dgvMovements.DataSource = _context.StockMovements
                .OrderByDescending(m => m.Id)
                .Take(100)
                .Select(m => new
                {
                    Barkod     = m.BarcodeSnapshot,
                    İşlemTipi  = m.Type,
                    Miktar     = m.Quantity,
                    Neden      = m.Reason,
                    Tarih      = m.CreatedAt
                })
                .ToList();

            // Sütun genişliklerinin tüm boşlukları kaplayacak şekilde esnemesi
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMovements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Kart güncellemesi
            if (lblTotalProductsVal != null) lblTotalProductsVal.Text = _dashboardService.GetTotalActiveProducts().ToString();
            if (lblDailySalesVal != null) lblDailySalesVal.Text = _dashboardService.GetTodaySalesTotal().ToString("C2");
            if (lblPendingBalanceVal != null) lblPendingBalanceVal.Text = _dashboardService.GetTotalPendingBalance().ToString("C2");
            
            if (lblLowStockVal != null) 
            {
                var lowStockProducts = _productService.GetAllActiveProducts().Where(p => p.StockQty < 5).ToList();
                int lStock = lowStockProducts.Count;
                if (lStock > 0)
                {
                    var names = string.Join(", ", lowStockProducts.Select(p => p.Name).Take(3));
                    if (lStock > 3) names += "...";
                    lblLowStockVal.Text = $"{lStock} Adet\n({names})";
                    lblLowStockVal.Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold);
                }
                else
                {
                    lblLowStockVal.Text = "0";
                    lblLowStockVal.Font = new System.Drawing.Font("Segoe UI", 24, FontStyle.Bold);
                }
            }

            // Grafik Güncellemesi
            if (chartTopSellers != null)
            {
                var dt = _dashboardService.GetTopSellingProducts(5);
                chartTopSellers.Series["Sales"].Points.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int index = chartTopSellers.Series["Sales"].Points.AddXY(row["Name"], row["TotalQty"]);
                    chartTopSellers.Series["Sales"].Points[index].Color = UiTheme.Primary;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var barcode = txtBarcode.Text.Trim();
            var name = txtName.Text.Trim();
            var priceText = txtPrice.Text.Trim();

            if (string.IsNullOrWhiteSpace(barcode) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Barkod ve ürün adı zorunludur.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out var price) || price < 0)
            {
                MessageBox.Show("Geçerli bir birim fiyat girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existingProduct = _productService.GetProductByBarcode(barcode);
            if (existingProduct != null)
            {
                existingProduct.Name = name;
                existingProduct.UnitPrice = (double)price;
                _productService.UpdateProduct(existingProduct);
                MessageBox.Show("Mevcut ürün sistemde bulundu ve isim/fiyat bilgileri başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
                txtBarcode.Clear();
                txtName.Clear();
                txtPrice.Clear();
                txtBarcode.Focus();
                return;
            }

            var p = new Product { Barcode = barcode, Name = name, UnitPrice = (double)price };
            _productService.AddProduct(p);
            
            // Zen.Barcode Yazdırma Modülü
            try
            {
                Zen.Barcode.Code128BarcodeDraw br = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                System.Drawing.Image img = br.Draw(barcode, 60);

                var f = new Form { Text = "Otomatik Barkod Etiketi: " + barcode, Size = new Size(300, 200), StartPosition = FormStartPosition.CenterParent };
                var pb = new PictureBox { Image = img, SizeMode = PictureBoxSizeMode.CenterImage, Dock = DockStyle.Fill };
                var lbl = new Label { Text = name + " - " + priceText + " TL", Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold) };
                f.Controls.Add(pb);
                f.Controls.Add(lbl);
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Barkod gösteriminde hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RefreshAll();

            txtBarcode.Clear();
            txtName.Clear();
            txtPrice.Clear();
            txtBarcode.Focus();
        }

        private void btnAddMovement_Click(object sender, EventArgs e)
        {
            var barcode = txtBarcodeMovement.Text.Trim();
            var prod = _productService.GetProductByBarcode(barcode);
            if (prod == null)
            {
                MessageBox.Show("Bu barkoda sahip bir ürün bulunamadı. Önce ürün kartını oluşturun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qty = (int)nudQuantity.Value;
            string type = cmbType.SelectedItem?.ToString() ?? "Giriş";

            if (type == "Çıkış")
            {
                if (prod.StockQty < qty)
                {
                    MessageBox.Show("Yetersiz stok.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                prod.StockQty -= qty;
            }
            else
            {
                prod.StockQty += qty;
            }

            _productService.UpdateProduct(prod);

            // Çıkış hareketlerinde miktar negatif saklanır (POS satışlarıyla tutarlı)
            int storedQty = (type == "Çıkış") ? -qty : qty;

            _context.StockMovements.Add(new StockMovement
            {
                ProductId        = prod.Id,
                BarcodeSnapshot  = barcode,
                Quantity         = storedQty,
                Type             = type,
                Reason           = "Manuel " + type,
                RefType          = "Manual",
                CreatedAt        = DateTime.Now,
                CreatedByUserId  = Session.UserId
            });
            _context.SaveChanges();

            // Log ekleme
            var ls = new LogService(_context);
            ls.Info("Stok Hareketi: " + type, $"Barkod: {barcode}, Miktar: {qty}", Session.UserId);

            RefreshAll();
            txtBarcodeMovement.Clear();
            nudQuantity.Value = 1;
            txtBarcodeMovement.Focus();
        }

        private void BtnPos_Click(object sender, EventArgs e) => OpenChildPage("Satış / POS", new FrmPos());
        private void BtnReturns_Click(object sender, EventArgs e) => OpenChildPage("İade / İptal", new FrmReturns());
        private void BtnCustomers_Click(object sender, EventArgs e) => OpenChildPage("Müşteriler", new FrmCustomers());
        private void BtnReports_Click(object sender, EventArgs e) => OpenChildPage("Raporlar", new FrmReports());
    }
}
