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
using ExHentaiLib.Prop;
using ExHentaiViewer.WPF.ViewModel;

namespace ExHentaiViewer.WPF
{
    /// <summary>
    /// ReadingPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReadingPage : Window
    {
        ReadingViewModel RDM;
        public ReadingPage(string uri)
        {
            InitializeComponent();
            RDM = new ReadingViewModel(uri);
            this.DataContext = RDM;
        }
        public ReadingPage(string uri, ImageListInfo info)
        {
            InitializeComponent();
            RDM = new ReadingViewModel(uri, info);
            this.DataContext = RDM;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void MiniButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
