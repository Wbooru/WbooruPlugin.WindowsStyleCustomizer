using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wbooru;
using Wbooru.Settings;
using Wbooru.UI.Controls;
using Wbooru.UI.Dialogs;

namespace WbooruPlugin.WindowsStyleCustomizer.Dialogs
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SetDesktopBackgroundDetail : DialogContentBase
    {
        Wallpaper.Style current_style = Wallpaper.Style.Center,backup_style;

        BitmapSource source;

        string temporary_file_path, persistence_file_path,backup_file;

        public SetDesktopBackgroundDetail(BitmapSource source)
        {
            InitializeComponent();

            SelectScaleType.ItemsSource = Enum.GetValues(typeof(Wallpaper.Style));
            SelectScaleType.SelectedIndex = 0;

            BackupCurrentWallpaper();
            this.source = source;
        }

        private void BackupCurrentWallpaper()
        {
            (backup_file, backup_style) = Wallpaper.GetCurrentWallpaper();

            Log.Info($"Get current wallpaper {backup_file} , style = {backup_style}");
        }

        private void SelectScaleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current_style = (Wallpaper.Style)(sender as ComboBox).SelectedItem;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (await ApplyWallpaper(true))
                Toast.ShowMessage($"预览成功");
            else
                Toast.ShowMessage($"预览失败");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (await ApplyWallpaper(false))
            {
                Dialog.CloseDialog(this);
                Toast.ShowMessage($"设置成功");
            }
            else
                Toast.ShowMessage($"设置失败");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RestoreBackupedWallpaper();
            Dialog.CloseDialog(this);
        }

        private void RestoreBackupedWallpaper()
        {
            if (!string.IsNullOrWhiteSpace(backup_file))
                Wallpaper.SetWallpaper(backup_file, backup_style);
        }

        private async Task<bool> ApplyWallpaper(bool is_temporary)
        {
            if (!await CheckBitmap(is_temporary))
                return false;

            var path = is_temporary ? temporary_file_path : persistence_file_path;

            return Wallpaper.SetWallpaper(path, current_style);
        }

        private string GeneratePath(bool is_temporary)
        {
            if (is_temporary)
                return System.IO.Path.GetTempFileName() + ".png";

            int i = 0;

            while (true)
            {
                var save_dir = System.IO.Path.Combine(Setting<GlobalSetting>.Current.DownloadPath, "Wallpaper");
                Directory.CreateDirectory(save_dir);

                var save_path = System.IO.Path.Combine(save_dir, $"image{(i == 0 ? "" : $"-{i}")}.png");

                if (!File.Exists(save_path))
                    return System.IO.Path.GetFullPath(save_path);

                i++;
            }
        }

        private async Task<bool> CheckBitmap(bool is_temporary)
        {
            var path = is_temporary ? temporary_file_path : persistence_file_path;

            if (File.Exists(path))
                return true;

            await Task.Run(() =>
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BitmapEncoder encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(source));
                        encoder.Save(ms);

                        using (var bitmap = new Bitmap(System.Drawing.Image.FromStream(ms)))
                        {
                            path = GeneratePath(is_temporary);
                            bitmap.Save(path);
                        }
                    }

                    if (is_temporary)
                        temporary_file_path = path;
                    else
                        persistence_file_path = path;

                    Log.Info($"Created bitmap and save to {(is_temporary ? "temporary" : "persistence")} file : {path}");
                }
                catch (Exception e)
                {
                    Log.Error($"Created bitmap failed : {e.Message}");
                }
            });

            return File.Exists(path);
        }
    }
}
