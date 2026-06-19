using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IconGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string pngPath = @"c:\Users\Halil Kocaoğlu\OneDrive\Masaüstü\Visual Projeler\Barcoded Warehouse Stock Tracking\Barcoded Warehouse Stock Tracking\Resources\poseidon_logo.png";
                string icoPath = @"c:\Users\Halil Kocaoğlu\OneDrive\Masaüstü\Visual Projeler\Barcoded Warehouse Stock Tracking\Barcoded Warehouse Stock Tracking\app_logo.ico";

                using (Bitmap bmp = new Bitmap(pngPath))
                {
                    // Standart Windows form ikonu için 32x32 veya 48x48 boyutlarında yeniden oluşturuyoruz
                    using (Bitmap resizedBmp = new Bitmap(bmp, new Size(48, 48)))
                    {
                        using (FileStream fs = new FileStream(icoPath, FileMode.Create))
                        {
                            // ICO dosya başlığı (Header)
                            fs.WriteByte(0); fs.WriteByte(0); // Reserved. Must always be 0.
                            fs.WriteByte(1); fs.WriteByte(0); // Image type: 1 for icon (.ICO)
                            fs.WriteByte(1); fs.WriteByte(0); // Number of images in file: 1

                            // Image Entry
                            fs.WriteByte(48); // Width
                            fs.WriteByte(48); // Height
                            fs.WriteByte(0);  // Color count (0 if >= 8bpp)
                            fs.WriteByte(0);  // Reserved
                            fs.WriteByte(1); fs.WriteByte(0); // Color planes (1)
                            fs.WriteByte(32); fs.WriteByte(0); // Bits per pixel (32 bits for transparency support)

                            // PNG formatında resmi bir belleğe kaydedip boyutunu okuyacağız
                            using (MemoryStream ms = new MemoryStream())
                            {
                                resizedBmp.Save(ms, ImageFormat.Png);
                                byte[] pngBytes = ms.ToArray();

                                int size = pngBytes.Length;
                                fs.Write(BitConverter.GetBytes(size), 0, 4); // Size of image data
                                fs.Write(BitConverter.GetBytes(22), 0, 4);   // Offset of image data (Header + Entry = 6 + 16 = 22 bytes)

                                fs.Write(pngBytes, 0, size); // PNG veri akışı
                            }
                        }
                    }
                }
                Console.WriteLine("SUCCESS");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
