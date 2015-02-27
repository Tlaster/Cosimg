using ExHentaiLib.Common;
using ExHentaiViewer.WPF.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ExHentaiViewer.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static DownLoadWindow downLoadWindow { get; set; }
        public App()
        {
            downLoadWindow = new DownLoadWindow();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                LogInHelper.LogCookieCheck(CookieHelper.GetCookie());
                new MainWindow().Show();
            }
            catch (Exception)
            {
                new LogInWindow().Show();
            }
        }
    }
}
