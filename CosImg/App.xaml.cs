using CosImg.Common;
using CosImg.ExHentai.Model;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using TBase.RT;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白应用程序”模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace CosImg
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {

        public static Frame rootFrame { get; private set; }
        public static List<DownLoadModel> DownLoadList;
        public static string ExitToastContent { get;set; }
        public static string CurrentPage { get; set; }


        /// <summary>
        /// 初始化单一实例应用程序对象。    这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            ExitToastContent = "再按一次返回键退出程序";
            UnhandledException += App_UnhandledException;
#if DEBUG
            UmengSDK.UmengAnalytics.IsDebug = true;
#endif
        }

        async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageDialog dialog = new MessageDialog("the app is crashed,send crash reports?","Oops");
            dialog.Commands.Add(new UICommand("Yes", async (a1) =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(@"mailto:CosImg@outlook.com?subject=CosImg Crash Report&body=" + e.Message + "%0d%0a" + e.Exception.StackTrace + "%0d%0a" + e.Exception.InnerException + "%0d%0a" + e.Exception.HResult + "%0d%0a" + e.Exception.Source + "%0d%0a" + CurrentPage));
            }));
            dialog.Commands.Add(new UICommand("No"));
            await dialog.ShowAsync();
        }

        bool isExit = false;
        async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (!rootFrame.CanGoBack)
            {
                if (DownLoadList != null && DownLoadList.Count != 0)
                {
                    MessageDialog dialog = new MessageDialog("Something is downloading,exit the application?", "Sure?");
                    dialog.Commands.Add(new UICommand("Exit", (a) =>
                    {
                        App.Current.Exit();
                    }));
                    dialog.Commands.Add(new UICommand("Cancel"));
                    e.Handled = true;
                    await dialog.ShowAsync();
                }
                else
                {
                    if (!isExit)
                    {
                        isExit = true;
                        ToastPrompt toast = new ToastPrompt(ExitToastContent);
                        toast.Completed += (s1, e1) => { isExit = false; };
                        toast.Show();
                        e.Handled = true;
                    }
                    else
                    {
                        App.Current.Exit();
                    }
                }
            }
            else
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }





        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以打开特定的文件或显示搜索结果等操作时，
        /// 将使用其他入口点。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
            UmengSDK.UmengAnalytics.IsDebug = true;
#endif

            //UmengSDK.UmengAnalytics.IsDebug = true;
            rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                // TODO: 将此值更改为适合您的应用程序的缓存大小
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: 从之前挂起的应用程序加载状态
                }
                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

#if DEBUG
            try
            {
                LogInHelper.LogCookieCheck(SettingHelpers.GetSetting<string>("cookie", true));
                App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
            }
            catch (Exception)
            {
                App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
            }
#else
            if (SettingHelpers.GetSetting<bool>("ExDefault"))
            {
                try
                {
                    LogInHelper.LogCookieCheck(SettingHelpers.GetSetting<string>("cookie", true));
                    App.rootFrame.Navigate(typeof(ExHentai.View.ExMainPage));
                }
                catch (Exception)
                {
                    App.rootFrame.Navigate(typeof(ExHentai.View.LoginPage));
                }
            }
            else
            {
                App.rootFrame.Navigate(typeof(MainPage));
            }
#endif






            // 确保当前窗口处于活动状态
            Window.Current.Activate();
            await UmengSDK.UmengAnalytics.StartTrackAsync("549eb88bfd98c51269000c35");

            var lastLaunchTime = SettingHelpers.GetSetting<string>("LastLaunch");
            var a = DateTime.Today.ToString();
            if (lastLaunchTime != DateTime.Today.ToString())
            {
                await ImageHelper.ClearCache();
            }
            SettingHelpers.SetSetting<string>("LastLaunch", DateTime.Today.ToString());

        }


        protected async override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            await UmengSDK.UmengAnalytics.StartTrackAsync("549eb88bfd98c51269000c35");
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。    在不知道应用程序
        /// 将被终止还是恢复的情况下保存应用程序状态，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: 保存应用程序状态并停止任何后台活动
            await UmengSDK.UmengAnalytics.EndTrackAsync();
            deferral.Complete();
        }
    }
}