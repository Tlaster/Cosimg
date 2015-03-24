using CosImg.ExHentai.Common;
using CosImg.ExHentai.Model;
using CosImg.ExHentai.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace CosImg.ExHentai.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ExMainPage : Page
    {
        public ExMainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.RequestedTheme = ElementTheme.Dark;
            Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView().CommandsRequested += ExMainPage_CommandsRequested;
            this.DataContext = new ExMainPageViewModel();
        }

        void ExMainPage_CommandsRequested(Windows.UI.ApplicationSettings.SettingsPane sender, Windows.UI.ApplicationSettings.SettingsPaneCommandsRequestedEventArgs args)
        {
            Windows.UI.ApplicationSettings.SettingsCommand settings = new Windows.UI.ApplicationSettings.SettingsCommand("Settings", "Settings", (a) =>
            {
                var exSettings = new ExSettingsFlyout();
                exSettings.Show();
            });
            args.Request.ApplicationCommands.Add(settings);

            Windows.UI.ApplicationSettings.SettingsCommand download = new Windows.UI.ApplicationSettings.SettingsCommand("Download", "Download", (a) =>
            {
                var exDownload = new DownLoadFlyout();
                exDownload.Show();
            });
            args.Request.ApplicationCommands.Add(download);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.NavigationMode == NavigationMode.New)
            {
                while (App.rootFrame.BackStack.Count != 0)
                {
                    App.rootFrame.BackStack.RemoveAt(0);
                }
                var downloadlist = await DownLoadDBHelpers.GetList();
                if (downloadlist != null && downloadlist.Count != 0)
                {
                    for (int i = 0; i < downloadlist.Count; i++)
                    {
                        if (App.DownLoadList == null)
                        {
                            App.DownLoadList = new List<DownLoadModel>();
                        }
                        App.DownLoadList.Add(new DownLoadModel(downloadlist[i]));
                    }
                }
            }
            var list = await FavorDBHelpers.Query();
            list.Reverse();
            favorGridView.ItemsSource = list;
        }
        

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {

        }
          
    }
}
