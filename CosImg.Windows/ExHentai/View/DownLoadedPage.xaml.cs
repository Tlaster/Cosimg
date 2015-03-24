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
    public sealed partial class DownLoadedPage : Page
    {
        public DownLoadedPage()
        {
            this.InitializeComponent();
            this.RequestedTheme = ElementTheme.Dark;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.gridView.ItemsSource = await DownLoadDBHelpers.GetList(true);
        }

        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.rootFrame.Navigate(typeof(ExDetailPage), new ExDetailViewModel((e.ClickedItem as DownLoadInfo).PageUri));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.rootFrame.GoBack();
        }
    }
}
