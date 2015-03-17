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

//“设置浮出控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=273769 上有介绍

namespace CosImg.ExHentai.View
{
    public sealed partial class ExSettingsFlyout : SettingsFlyout
    {
        public ExSettingsFlyout()
        {
            this.InitializeComponent();
            this.RequestedTheme = ElementTheme.Dark;
            PageSwitch.IsOn = SettingHelpers.GetSetting<bool>("ExDefault");
        }

        private void PageSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            SettingHelpers.SetSetting<bool>("ExDefault", PageSwitch.IsOn);
        }
    }
}
