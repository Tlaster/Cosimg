using CosImg.CosImg.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TBase.RT;
using TBase;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Net;
using Windows.Web.Http;
using CosImg.Common;
using HtmlAgilityPack;
using Windows.ApplicationModel.DataTransfer;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace CosImg.CosImg.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        private ListModel Item;
        private byte[] ImageByte;
        private string link;
        public ImagePage()
        {
            this.InitializeComponent();
            this.imageGrid.Width = (Window.Current.Content as Frame).ActualWidth;
            this.imageGrid.Height = (Window.Current.Content as Frame).ActualHeight;
            this.BigImage.Width = (Window.Current.Content as Frame).ActualWidth;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            proRing.IsActive = true;
            Item = e.Parameter as ListModel;
            var htmlStr = await HttpHelper.GetString("http://worldcosplay.net" + Item.url);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlStr);
            HtmlNode node;
            if (Item.url.Contains("instants"))
            {
                node = doc.DocumentNode.GetNodebyClassName("photo");
            }
            else
            {
                node = doc.DocumentNode.GetNodebyClassName("img");
            }
            this.link = node.Attributes["src"].Value;
            BitmapImage img = new BitmapImage(new Uri(link));
            ImageByte = await HttpHelper.GetByteArray(node.Attributes["src"].Value);
            this.BigImage.Source = await ImageHelper.ByteArrayToBitmapImage(ImageByte);
            RegisterForShare();
            proRing.IsActive = false;
        }


        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.BigImage.Source == null)
            {
                new ToastPrompt("Please Wait While The Image Is Loading").Show();
                return;
            } 
            DataTransferManager.ShowShareUI();
        }
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareHandler;
        }

        private async void ShareHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (ImageByte != null)
            {
                DataRequestDeferral deferral = e.Request.GetDeferral();
                DataRequest request = e.Request;
                request.Data.Properties.Title = "分享图片";
                InMemoryRandomAccessStream randonAcc = new InMemoryRandomAccessStream();
                IOutputStream outputStream = randonAcc.GetOutputStreamAt(0);
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteBytes(this.ImageByte);
                await writer.StoreAsync();
                await writer.FlushAsync();
                RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromStream(randonAcc);
                request.Data.SetBitmap(streamRef);
                deferral.Complete();
            }
        }


        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImageByte == null)
            {
                new ToastPrompt("Please Wait While The Image Is Loading").Show();
                return;
            }
            var name = Path.GetFileName(link);
            await ImageHelper.SaveImage(name,ImageByte);
        }

    }
}
