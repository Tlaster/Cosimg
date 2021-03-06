﻿using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TBase.RT;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
        private bool _isbusy;
        public LoginPage()
        {
            this.InitializeComponent();
#if WINDOWS_PHONE_APP
            WP_BUTTON.Visibility = Windows.UI.Xaml.Visibility.Visible;
#else
            WIN8_BUTTON.Visibility = Windows.UI.Xaml.Visibility.Visible;
#endif
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
#if WINDOWS_PHONE_APP
            await StatusBar.GetForCurrentView().HideAsync();
#endif
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_isbusy)
            {
                _isbusy = true;
                ToastPrompt toast = new ToastPrompt("Now login,please wait", true);
                MessageDialog dialog;
                try
                {
                    toast.ShowWithProgressBar();
                    var loginCookie = await LogInHelper.GetLoginCookie(userName.Text, password.Password);
                    toast.HideWithProgressBar();
                    SettingHelpers.SetSetting<string>("cookie", loginCookie);
                    new ToastPrompt("Login Succeed").Show();
                    App.rootFrame.Navigate(typeof(ExMainPage));
                    while (App.rootFrame.BackStack.Count != 0)
                    {
                        App.rootFrame.BackStack.RemoveAt(0);
                    }
                }
                catch (LoginException)
                {
                    dialog = new MessageDialog("Please check your account and try again", "Login Failed");
                    dialog.ShowAsync();
                    toast.HideWithProgressBar();
                }
                catch (LogAccessException)
                {
                    dialog = new MessageDialog("Maybe your account have no access to exhentai", "Login Failed");
                    dialog.ShowAsync();
                    toast.HideWithProgressBar();
                }
                catch (Exception)
                {
                    new ToastPrompt("Login Failed,Please try again").Show();
                    toast.HideWithProgressBar();
                }
                _isbusy = false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.rootFrame.GoBack();
        }

    }
}
