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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace CosImg.ExHentai.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownLoadPage : Page
    {
        DownLoadViewModel VM;
        public DownLoadPage()
        {
            this.InitializeComponent();
            VM = new DownLoadViewModel();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Dark;
            UmengSDK.UmengAnalytics.TrackPageStart("DownLoadPage");
            this.DataContext = VM;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageEnd("DownLoadPage");
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            downloadGridView.SelectionMode = ListViewSelectionMode.Multiple;
            DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SelectButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            VM.RemoveList = downloadGridView.SelectedItems.ToList();
            downloadGridView.SelectionMode = ListViewSelectionMode.None;
            DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            SelectButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    this.BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;
                default:
                    this.BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }
        }


    }
}
