using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Wbooru;

namespace WbooruPlugin.WindowsStyleCustomizer
{
    //copy and modify from https://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
    public static class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Tile,
            Fit,
            Span,
            Fill,
            Stretch,
            Center
        }

        public static Dictionary<Style, (int WallpaperStyle, int TileWallpaper)> value_map = new Dictionary<Style, (int WallpaperStyle, int TileWallpaper)>()
        {
            { Style.Fill,(10,0) },
            { Style.Fit,(6,0) },
            { Style.Span,(22,0) },
            { Style.Stretch,(2,0) },
            { Style.Tile,(0,1) },
            { Style.Center,(0,0) },
        };

        public static bool SetWallpaper(Bitmap img, Style style)
        {
            var path = Path.GetTempFileName();
            img.Save(path);

            Log.Info($"Saved bitmap to temp file {path}");

            return SetWallpaper(path, style);
        }

        public static bool SetWallpaper(string path, Style style)
        {
            Log.Info($"Style = {style}");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            var value = value_map[style];
            key.SetValue(@"WallpaperStyle", value.WallpaperStyle.ToString());
            key.SetValue(@"TileWallpaper", value.TileWallpaper.ToString());

            return 0 != SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static (string,Style) GetCurrentWallpaper()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
            var path = key.GetValue("WallPaper").ToString();

            var WallpaperStyle = int.Parse(key.GetValue(@"WallpaperStyle").ToString());
            var TileWallpaper = int.Parse(key.GetValue(@"TileWallpaper").ToString());

            var style = value_map.Where(x => x.Value == (WallpaperStyle, TileWallpaper)).FirstOrDefault().Key;

            return (path, style);
        }
    }
}
