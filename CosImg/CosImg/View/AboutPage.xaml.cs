using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TBase.RT;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace CosImg.CosImg.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private int goint = 0;

        public int Goint
        {
            get 
            { 
                return goint;
            }
            set 
            { 
                goint = value;
                if (goint==4)
                {
                    MessageDialog dialog = new MessageDialog("什么！？你还想吃鸡肋！？");
                    dialog.Commands.Add(new UICommand("不吃了",wowGoGoGO));
                    dialog.Commands.Add(new UICommand("不吃了"));
                    dialog.ShowAsync();
                }
            }
        }

        private async void wowGoGoGO(IUICommand command)
        {
            await Task.Delay(3000);
            try
            {
                SettingHelpers.GetSetting<string>("cookie", true);
                App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
            }
            catch (SettingException)
            {
                App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
            }
        }

        public AboutPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var rectangle = sender as Rectangle;
            if (rectangle.Tag.ToString()==goint.ToString())
            {
                Goint++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingHelpers.GetSetting<string>("cookie", true);
                App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
            }
            catch (SettingException)
            {
                App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
            }
        }

    }
}
