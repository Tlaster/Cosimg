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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExHentaiViewer.WPF.Toolkit
{
    /// <summary>
    /// SearchBox.xaml 的交互逻辑
    /// </summary>
    public partial class SearchBox : UserControl
    {
        Brush brush = (Brush)(new BrushConverter()).ConvertFromString("#FF007ACC");

        public double SearchBoxFontSize
        {
            get { return (double)GetValue(SearchBoxFontSizeProperty); }
            set { SetValue(SearchBoxFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchBoxFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchBoxFontSizeProperty =
            DependencyProperty.Register("SearchBoxFontSize", typeof(double), typeof(SearchBox), new PropertyMetadata(FontSizeChange));


        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SearchBox));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }



        public SearchBox()
        {
            InitializeComponent();
        }

        private static void FontSizeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SearchBox).FontSize = (double)e.NewValue;
        }

        void SearchTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if ((sender as TextBox).Text == "Search...")
                {
                    (sender as TextBox).Text = string.Empty;
                }
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.SearchTextBox.Text == string.Empty)
            {
                SearchTextBox.Text = "Search...";
            }
            this.BorderBrush = Brushes.Transparent;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.BorderBrush = brush;
        }


        public event EventHandler<RoutedEventArgs> SearchClick;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SearchClick != null)
            {
                SearchClick(sender, e);
            }
        }
    }
}
