using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace TBase.RT
{
    public sealed partial class ToastPrompt : UserControl
    {        /// <summary>
        /// 当ToastPrompt完成显示时的事件
        /// </summary>
        public event EventHandler<Object> Completed;

        /// <summary>
        /// 新建提示Toast，并使用ProgressBar
        /// </summary>
        /// <param name="title">Toast内容</param>
        /// <param name="IsIndeterminate">设置ProgressBar的IsIndeterminate值</param>
        public ToastPrompt(string title, bool IsIndeterminate = false)
        {
            this.InitializeComponent();
            this.innerText.Text = title;
            this.probar.IsIndeterminate = IsIndeterminate;
            this.border.Width = (Window.Current.Content as Frame).ActualWidth;
        }


        public void ShowWithProgressBar()
        {
            popup.IsOpen = true;
            this.probar.Visibility = Visibility.Visible;
            this.SlideInAnimation.Begin();
        }

        public void HideWithProgressBar()
        {
            this.SlideOutAnimation.Begin();
            this.SlideOutAnimation.Completed += (sender, e) =>
            {
                this.probar.IsIndeterminate = false;
            };
        }

        /// <summary>
        /// 显示ToastPrompt
        /// </summary>
        public void Show()
        {
            popup.IsOpen = true;
            this.SlideAnimation.Begin();
        }

        private void SlideAnimation_Completed(object sender, object e)
        {
            popup.IsOpen = false;
            if (this.Completed != null)
            {
                this.Completed(sender, e);
            }
        }

    }
}
