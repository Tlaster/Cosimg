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

            if (LogState())
            {
                new MainWindow().Show();
            }
            else
            {
                new LogInWindow().Show();
            }
        }
        private bool LogState()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory+"cookie.cookie"))
            {
                var logcookie = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie");
                string memberidRegex = @"ipb_member_id=([^;]*)";
                string passhashRegex = @"ipb_pass_hash=([^;]*)";
                var memberidStr = Regex.Match(logcookie, memberidRegex);
                var passhashStr = Regex.Match(logcookie, passhashRegex);
                if (memberidStr.Success && passhashStr.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
