using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CosImg.ExHentai.Common
{
    public class ReadingViewSelector : DataTemplateSelector 
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            return base.SelectTemplateCore(item, container);
        }
    }
}
