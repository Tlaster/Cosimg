using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase.RT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CosImg.ExHentai.Common
{

    public class ReadingViewSelector:DataTemplateSelector
    {
        public DataTemplate CommonView { get; set; }
        public DataTemplate FlipBookView { get; set; }

        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item)
        {
            return SettingHelpers.GetSetting<bool>("isFlipBookView") ? FlipBookView : CommonView;
        }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return this.SelectTemplateCore(item);
        }
    }
}
