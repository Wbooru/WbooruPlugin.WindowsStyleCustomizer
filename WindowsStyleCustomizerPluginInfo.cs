using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wbooru.PluginExt;

namespace WbooruPlugin.WindowsStyleCustomizer
{
    [Export(typeof(PluginInfo))]
    public class WindowsStyleCustomizerPluginInfo : PluginInfo
    {
        public override string PluginName => "Windows Style Customizer";

        public override string PluginProjectWebsite => "https://github.com/Wbooru/WbooruPlugin.WindowsStyleCustomizer";

        public override string PluginAuthor => "MikiraSora";

        public override string PluginDescription => "能够提供将图片设置成桌面壁纸或者锁屏壁纸的功能";
    }
}
