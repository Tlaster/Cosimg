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
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ToastPrompt toast = new ToastPrompt("Now login,please wait", true);
            try
            {
                toast.ShowWithProgressBar();
                var loginCookie = await LogInHelper.GetLoginCookie(userName.Text, password.Password);
                toast.HideWithProgressBar();
                SettingHelpers.SetSetting<string>("cookie",loginCookie);
                new ToastPrompt("Login Succeed").Show();
                App.rootFrame.Navigate(typeof(ExMainPage));
                App.rootFrame.BackStack.RemoveAt(0);
            }
            catch (LoginException)
            {
                new ToastPrompt("Login Failed,Please check your account and try again").Show();
                toast.HideWithProgressBar();
            }
            catch (Exception)
            {
                new ToastPrompt("Login Failed,Please try again").Show();
                toast.HideWithProgressBar();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.rootFrame.GoBack();
        }
    }
}
