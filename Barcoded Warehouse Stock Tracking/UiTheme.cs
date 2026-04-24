using System.Drawing;

namespace Barcoded_Warehouse_Stock_Tracking
{
    /// <summary>Poseidon Sapphire: Derin gece mavisi ve safir tonlarından oluşan kurumsal palet.</summary>
    public static class UiTheme
    {
        // Sidebar & Navigation
        public static readonly Color Sidebar = Color.FromArgb(23, 37, 84);         // Deep Sapphire Midnight (Clearly Blue)
        public static readonly Color SidebarHover = Color.FromArgb(30, 58, 138);    // Lighter Blue for Hover
        public static readonly Color SidebarSelected = Color.FromArgb(2, 132, 199); // Sapphire Blue

        // Primary Colors
        public static readonly Color Primary = Color.FromArgb(14, 165, 233);       // Sky Blue
        public static readonly Color PrimaryDark = Color.FromArgb(3, 105, 161);    // Sapphire 700
        
        // Background & Surface
        public static readonly Color MainBackground = Color.FromArgb(248, 250, 252); // Slate 50
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceMuted = Color.FromArgb(241, 245, 249);  // Slate 100
        
        // Text Colors
        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);      // Slate 900
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);     // Slate 500
        
        // Status Colors
        public static readonly Color Success = Color.FromArgb(16, 185, 129);       // Emerald 500
        public static readonly Color SuccessSoft = Color.FromArgb(236, 253, 245);  // Emerald 50
        public static readonly Color Danger = Color.FromArgb(244, 63, 94);         // Rose 500
        public static readonly Color DangerSoft = Color.FromArgb(255, 241, 242);   // Rose 50
        public static readonly Color Warning = Color.FromArgb(245, 158, 11);        // Amber 500
        
        // Accents
        public static readonly Color AccentSapphire = Color.FromArgb(56, 189, 248); // Sky 400
        public static readonly Color CardBlue = Color.FromArgb(240, 249, 255);      // Sky 50
        
        // Input Controls
        public static readonly Color InputFill = Color.White;
        public static readonly Color InputBorder = Color.FromArgb(226, 232, 240);   // Slate 200
        
        // Grid & Tables
        public static readonly Color GridHeaderBg = Color.FromArgb(241, 245, 249);  // Slate 100
        public static readonly Color GridLine = Color.FromArgb(226, 232, 240);      // Slate 200
        public static readonly Color GridRowAlt = Color.FromArgb(248, 250, 252);    // Slate 50
        public static readonly Color Selection = Color.FromArgb(224, 242, 254);     // Light Blue Selection
        
        // Compatibility Aliases
        public static readonly Color CardLavender = Color.FromArgb(240, 249, 255);  // Redirected to Sapphire Blue
    }
}
