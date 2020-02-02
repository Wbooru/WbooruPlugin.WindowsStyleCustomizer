using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wbooru.UI.Controls;
using Wbooru.UI.Controls.PluginExtension;
using Wbooru.UI.Dialogs;
using Wbooru.UI.Pages;
using Wbooru.Utils;
using Windows.Storage;
using Windows.System.UserProfile;

namespace WbooruPlugin.WindowsStyleCustomizer
{
    [Export(typeof(IExtraDetailImageMenuItem))]
    public class LockScreenMenuButtonGenerator : IExtraDetailImageMenuItem
    {
        public UIElement Create()
        {
            var button = new MenuItem()
            {
                Header = "设置为锁屏壁纸"
            };
            button.Click += SetLockScreenBackground;

            return button;
        }

        private async void SetLockScreenBackground(object sender, RoutedEventArgs _)
        {
            var picture_page = (sender as FrameworkElement)?.DataContext as PictureDetailViewPage;

            if (!(ViusalTreeHelperEx.Find(a => a is ImageViewer, picture_page) is ImageViewer viewer))
            {
                Toast.ShowMessage($"找不到依赖的ImageViewer控件");
                return;
            }

            if (!(viewer.ImageSource is BitmapSource source))
            {
                Toast.ShowMessage($"无法从ImageViewer控件提取图片");
                return;
            }

            if (!await Dialog.ShowComfirmDialog("是否将此图片设置成锁屏壁纸?"))
                return;

            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    encoder.Save(ms);

                    /*
                    using (var bitmap = new Bitmap(System.Drawing.Image.FromStream(ms)))
                    {
                        var path = Path.GetTempFileName();
                        bitmap.Save(path);

                        var file = await StorageFile.GetFileFromPathAsync(path);
                        await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                    }
                    */

                    await LockScreen.SetImageStreamAsync(ms.AsRandomAccessStream());

                    Toast.ShowMessage("设置成功");
                }
            }
            catch (Exception e)
            {
                Toast.ShowMessage("设置失败:" + e.Message);
            }
        }
    }
}
