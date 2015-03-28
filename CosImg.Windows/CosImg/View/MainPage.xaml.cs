using CosImg.CosImg.Model;
using CosImg.CosImg.View;
using CosImg.CosImg.ViewModel;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace CosImg
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void listBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ListModel;
            new ImagePopUp(item).Show();
        }

        private async void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
#if DEBUG
            if (args.QueryText == "go")
#else
            if (args.QueryText == "I AM HENTAI")
#endif
            {
                MessageDialog dialog = new MessageDialog("Sure?");
                dialog.Commands.Add(new UICommand("Yes", (a) =>
                {
                    try
                    {
                        LogInHelper.LogCookieCheck(SettingHelpers.GetSetting<string>("cookie", true));
                        App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
                    }
                    catch (Exception)
                    {
                        App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
                    }
                }));
                dialog.Commands.Add(new UICommand("No"));
                await dialog.ShowAsync();
            }
            else
            {
                App.rootFrame.Navigate(typeof(SearchPage), args.QueryText);
            }
        }

    }
}
