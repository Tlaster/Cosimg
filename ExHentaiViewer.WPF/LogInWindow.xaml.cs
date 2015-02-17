using System;
using System.Collections.Generic;
using System.IO;
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
using ExHentaiLib.Common;
using ExHentaiViewer.WPF.Toolkit;

namespace ExHentaiViewer.WPF
{
    /// <summary>
    /// LogInWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }
        private async Task TryLog()
        {
            try
            {
                OnLoging.Visibility = System.Windows.Visibility.Visible;
                var cookie = await LogInHelper.GetLoginCookie(UserNameIn.Text, PasswordIn.Password);
                SsveCookie(cookie);
                new MainWindow().Show();
                OnLoging.Visibility = System.Windows.Visibility.Collapsed;
                this.Close();
            }
            catch (LoginException)
            {
                OnLoging.Visibility = System.Windows.Visibility.Collapsed;
                new MessageWindow("Error", "Login Error,Please check your account").ShowDialog();
            }
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await TryLog();
        }
        private void SsveCookie(string cookie)
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie", cookie);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void PasswordIn_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await TryLog();
            }
        }
    }
}
