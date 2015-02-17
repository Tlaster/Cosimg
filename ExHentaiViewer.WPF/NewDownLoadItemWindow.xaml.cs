using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExHentaiViewer.WPF
{
    /// <summary>
    /// NewDownLoadItemWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewDownLoadItemWindow : Window
    {
        private string uri;
        private ExHentaiLib.Prop.DetailProp Detail;
        public string ItemName { get; set; }
        public string DownLoadPath { get; set; }
        public NewDownLoadItemWindow()
        {
            InitializeComponent();
        }

        public NewDownLoadItemWindow(string uri, ExHentaiLib.Prop.DetailProp Detail)
        {
            InitializeComponent();
            this.uri = uri;
            this.Detail = Detail;
            DownLoadPath = AppDomain.CurrentDomain.BaseDirectory;
            this.DownloadPathButton.Content = DownLoadPath;
            ItemNameTB.Text = Detail.HeaderInfo.TitleEn;
            ItemUriTB.Text = uri;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dilog = new System.Windows.Forms.FolderBrowserDialog();
            dilog.Description = "Choose a folder";
            dilog.ShowNewFolderButton = true;
            if (dilog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DownLoadPath = dilog.SelectedPath;
                this.DownloadPathButton.Content = DownLoadPath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            ItemName = ItemNameTB.Text;
            uri = ItemUriTB.Text;
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
