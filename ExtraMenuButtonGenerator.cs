using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wbooru;
using Wbooru.UI.Controls;
using Wbooru.UI.Controls.PluginExtension;
using Wbooru.UI.Dialogs;
using Wbooru.UI.Pages;
using Wbooru.Utils;
using WbooruPlugin.WindowsStyleCustomizer.Dialogs;

namespace WbooruPlugin.WindowsStyleCustomizer
{
    [Export(typeof(IExtraDetailImageMenuItem))]
    public class ExtraMenuButtonGenerator : IExtraDetailImageMenuItem
    {
        public UIElement Create()
        {
            var button = new MenuItem()
            {
                Header = "设置为桌面壁纸"
            };
            button.Click += SetDesktopBackground;

            return button;
        }

        private async void SetDesktopBackground(object sender, RoutedEventArgs e)
        {
            var picture_page = (sender as FrameworkElement)?.DataContext as PictureDetailViewPage;

            if (!(ViusalTreeHelperEx.Find(a=>a is ImageViewer,picture_page) is ImageViewer viewer))
            {
                Toast.ShowMessage($"找不到依赖的ImageViewer控件");
                return;
            }

            if (!(viewer.ImageSource is BitmapSource source))
            {
                Toast.ShowMessage($"无法从ImageViewer控件提取图片");
                return;
            }

            await Dialog.ShowDialog(new SetDesktopBackgroundDetail(source));
        }
    }
}
