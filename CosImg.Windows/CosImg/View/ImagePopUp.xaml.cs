using CosImg.Common;
using CosImg.CosImg.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TBase;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace CosImg.CosImg.View
{
    public sealed partial class ImagePopUp : UserControl
    {
        public EventHandler<RoutedEventArgs> SaveClick;
        private string link;
        private byte[] ImageByte;
        public ImagePopUp(ListModel item)
        {
            this.InitializeComponent();
            grid.Width = (Window.Current.Content as Frame).ActualWidth;
            grid.Height = (Window.Current.Content as Frame).ActualHeight;
            OnLoaded(item);
        }

        private async void OnLoaded(ListModel item)
        {
            proRin.IsActive = true;
            var htmlStr = await HttpHelper.GetReadString("http://worldcosplay.net" + item.url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);
            HtmlNode node;
            if (item.url.Contains("instants"))
            {
                node = doc.DocumentNode.GetNodebyClassName("photo");
            }

            else
            {
                node = doc.DocumentNode.GetNodebyClassName("img");
            }
            this.link = node.Attributes["src"].Value;
            ImageByte = await HttpHelper.GetByteArray(node.Attributes["src"].Value);
            this.image.Source = await ImageHelper.ByteArrayToBitmapImage(ImageByte);
            proRin.IsActive = false;
        }

        



        public void Show()
        {
            this.popup.IsOpen = true;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await ImageHelper.SaveImage(Path.GetFileName(link), ImageByte);
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.popup.IsOpen = false;
        }
    }
}
