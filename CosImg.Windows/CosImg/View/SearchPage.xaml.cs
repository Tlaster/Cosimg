using CosImg.CosImg.Common;
using CosImg.CosImg.Model;
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

namespace CosImg.CosImg.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public string Title { get; set; }
        public GeneratorIncrementalLoadingClass<ListModel> List { get; set; }
        public SearchPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            OnLoaded(e.Parameter as string);
        }

        private void OnLoaded(string p)
        {
            Title = p;
            List = new GeneratorIncrementalLoadingClass<ListModel>("http://worldcosplay.net/api/photo/search?q=" + p, (item) =>
            {
                return new ListModel()
                    {
                        image = item.GetObject()["photo"].GetObject()["sq300_url"].GetString(),
                        name = item.GetObject()["photo"].GetObject()["subject"].GetString(),
                        url = item.GetObject()["photo"].GetObject()["url"].GetString(),
                        id = item.GetObject()["photo"].GetObject()["id"].GetNumber(),
                    };
            });
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.rootFrame.GoBack();
        }
    }
}
