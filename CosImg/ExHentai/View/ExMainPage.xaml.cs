using CosImg.ExHentai.ViewModel;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TBase.RT;
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
    public sealed partial class ExMainPage : Page
    {
        public ExMainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.DataContext = new ExHentai.ViewModel.ExMainPageViewModel();
        } 

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                try
                {
                    App.rootFrame.BackStack.RemoveAt(0);
                    App.rootFrame.BackStack.RemoveAt(0);
                    App.rootFrame.BackStack.RemoveAt(0);
                }
                catch (Exception)
                {
                }
                this.RequestedTheme = ElementTheme.Dark;
            }
            UmengSDK.UmengAnalytics.TrackEvent("HentaiModeExChanged");
            UmengSDK.UmengAnalytics.TrackPageStart("ExHnetaiMainPage");

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UmengSDK.UmengAnalytics.TrackPageEnd("ExHnetaiMainPage");
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.pivot.SelectedIndex = 1;
        }
    }
}
