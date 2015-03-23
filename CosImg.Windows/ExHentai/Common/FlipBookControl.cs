using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CosImg.ExHentai.Common
{   
    [TemplatePart(Name = A_PARTNAME, Type = typeof(Border))]
    [TemplatePart(Name = B_PARTNAME, Type = typeof(Border))]
    [TemplatePart(Name = C_PARTNAME, Type = typeof(Border))]
    [TemplatePart(Name = D_PARTNAME, Type = typeof(Border))]
    [TemplatePart(Name = E_PARTNAME, Type = typeof(Border))]
    [TemplatePart(Name = F_PARTNAME, Type = typeof(Border))]
    [TemplatePart(Name = A_RECT_PARTNAME, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = B_RECT_PARTNAME, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = C_RECT_PARTNAME, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = D_RECT_PARTNAME, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = E_RECT_PARTNAME, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = F_RECT_PARTNAME, Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = A_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = B_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = C_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = D_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = E_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = F_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = A_PRESENTER_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = B_PRESENTER_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = C_PRESENTER_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = D_PRESENTER_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = E_PRESENTER_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = F_PRESENTER_PARTNAME, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = INNER_LEFT_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = OUTER_LEFT_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = INNER_RIGHT_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = OUTER_RIGHT_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = MARGIN_LEFT_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = MARGIN_RIGHT_TRANS_PARTNAME, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = ST_SHADOW_SPLIT_INNER_LEFT_PARTNAME, Type = typeof(StackPanel))]
    [TemplatePart(Name = ST_SHADOW_SPLIT_OUTER_LEFT_PARTNAME, Type = typeof(StackPanel))]
    [TemplatePart(Name = ST_SHADOW_SPLIT_INNER_RIGHT_PARTNAME, Type = typeof(StackPanel))]
    [TemplatePart(Name = ST_SHADOW_SPLIT_OUTER_RIGHT_PARTNAME, Type = typeof(StackPanel))]
    [TemplatePart(Name = ST_SHADOW_MARGIN_LEFT_PARTNAME, Type = typeof(StackPanel))]
    [TemplatePart(Name = ST_SHADOW_MARGIN_RIGHT_PARTNAME, Type = typeof(StackPanel))]
    [TemplatePart(Name = BOOK_CONTAINER_PARTNAME, Type = typeof(Image))]
    [TemplatePart(Name = GRD_CONTENT_PARTNAME, Type = typeof(Grid))]

    /// <summary>
    /// it is from http://blog.csdn.net/wangrenzhu2011/article/details/10207413
    /// </summary>
    public sealed class FlipBookControl : ItemsControl
    {
        #region Member Variables
        /// <summary>
        /// 0 初始 翻页状态1 翻页状态2
        /// </summary>
        private int Status = 0;
        /// <summary>
        /// 是否翻下一页
        /// true 下一页 false 上一页
        /// </summary>
        private bool isNext = false;
        /// <summary>
        /// 触发向左翻页动画
        /// true 发生偏移  false 停止偏移
        /// </summary>
        private bool turnLeft = false;
        /// <summary>
        /// 触发向右翻页动画
        /// true 发生偏移  false 停止偏移
        /// </summary>
        private bool turnRight = false;
        /// <summary>
        /// 触发向右翻页还原动画
        /// true 发生偏移  false 停止偏移
        /// </summary>
        private bool rightRestore = false;
        /// <summary>
        /// 触发向左翻页还原动画
        /// true 发生偏移  false 停止偏移
        /// </summary>
        private bool leftRestore = false;
        /// <summary>
        /// 是否多点操作中
        /// true 是 false 否
        /// </summary>
        private bool isManipulating = false;
        /// <summary>
        /// 最近一次偏移量
        /// </summary>
        private double lastDeltaOffset = 0.0;
        /// <summary>
        /// 横向偏移量
        /// </summary>
        private double offsetWidth = 0.0;
        /// <summary>
        /// 竖向偏移量
        /// </summary>
        private double offsetHeight = 0.0;
        /// <summary>
        /// 是否加载
        /// </summary>
        private bool isLoaded = false;
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool isInit = false;

        /// <summary>
        /// 控制是否翻页
        /// </summary>
        private bool isFlip = true;

        /// <summary>
        /// 是否释放
        /// </summary>
        private bool isRelease = true;
        Border nextPage;
        Border prevPage;
        Border leftPage;
        Border rightPage;
        Border leftTopPage;
        Border rightTopPage;
        CompositeTransform nextTrans;
        CompositeTransform prevTrans;
        Border A;
        Border B;
        Border C;
        Border D;
        Border E;
        Border F;
        ContentPresenter APresenter;
        ContentPresenter BPresenter;
        ContentPresenter CPresenter;
        ContentPresenter DPresenter;
        ContentPresenter EPresenter;
        ContentPresenter FPresenter;
        RectangleGeometry ARect;
        RectangleGeometry BRect;
        RectangleGeometry CRect;
        RectangleGeometry DRect;
        RectangleGeometry ERect;
        RectangleGeometry FRect;
        CompositeTransform transA;
        CompositeTransform transB;
        CompositeTransform transC;
        CompositeTransform transD;
        CompositeTransform transE;
        CompositeTransform transF;
        CompositeTransform innerLeftTrans;
        CompositeTransform outerLeftTrans;
        CompositeTransform innerRightTrans;
        CompositeTransform outerRightTrans;
        CompositeTransform marginLeftTrans;
        CompositeTransform marginRightTrans;
        StackPanel stShadowSplitOuterLeft;
        StackPanel stShadowSplitInnerLeft;
        StackPanel stShadowSplitOuterRight;
        StackPanel stShadowSplitInnerRight;
        StackPanel stShadowMarginLeft;
        StackPanel stShadowMarginRight;
        Grid grdContent;
        Image bookContainer;
        ImageBrush leftBrush;
        ImageBrush rightBrush;
        private TransformGroup _transformGroup;
        private MatrixTransform _previousTransform;
        private CompositeTransform _compositeTransform;
        #endregion

        #region Template Part
        /// <summary>
        /// 矩形
        /// </summary>
        const string A_PARTNAME = "A";
        /// <summary>
        /// 矩形遮掩
        /// </summary>
        const string A_RECT_PARTNAME = "ARect";
        /// <summary>
        /// 矩形偏移
        /// </summary>
        const string A_TRANS_PARTNAME = "transA";
        const string A_PRESENTER_PARTNAME = "APresenter";
        const string B_PARTNAME = "B";
        const string B_RECT_PARTNAME = "BRect";
        const string B_TRANS_PARTNAME = "transB";
        const string B_PRESENTER_PARTNAME = "BPresenter";
        const string C_PARTNAME = "C";
        const string C_RECT_PARTNAME = "CRect";
        const string C_TRANS_PARTNAME = "transC";
        const string C_PRESENTER_PARTNAME = "CPresenter";
        const string D_PARTNAME = "D";
        const string D_RECT_PARTNAME = "DRect";
        const string D_TRANS_PARTNAME = "transD";
        const string D_PRESENTER_PARTNAME = "DPresenter";
        const string E_PARTNAME = "E";
        const string E_RECT_PARTNAME = "ERect";
        const string E_TRANS_PARTNAME = "transE";
        const string E_PRESENTER_PARTNAME = "EPresenter";
        const string F_PARTNAME = "F";
        const string F_RECT_PARTNAME = "FRect";
        const string F_TRANS_PARTNAME = "transF";
        const string F_PRESENTER_PARTNAME = "FPresenter";
        const string ST_SHADOW_SPLIT_OUTER_RIGHT_PARTNAME = "stShadowSplitOuterRight";
        const string ST_SHADOW_SPLIT_INNER_RIGHT_PARTNAME = "stShadowSplitInnerRight";
        const string ST_SHADOW_SPLIT_OUTER_LEFT_PARTNAME = "stShadowSplitOuterLeft";
        const string ST_SHADOW_SPLIT_INNER_LEFT_PARTNAME = "stShadowSplitInnerLeft";
        const string ST_SHADOW_MARGIN_LEFT_PARTNAME = "stShadowMarginLeft";
        const string ST_SHADOW_MARGIN_RIGHT_PARTNAME = "stShadowMarginRight";
        const string OUTER_LEFT_TRANS_PARTNAME = "outerLeftTrans";
        const string INNER_LEFT_TRANS_PARTNAME = "innerLeftTrans";
        const string OUTER_RIGHT_TRANS_PARTNAME = "outerRightTrans";
        const string INNER_RIGHT_TRANS_PARTNAME = "innerRightTrans";
        const string MARGIN_LEFT_TRANS_PARTNAME = "marginLeftTrans";
        const string MARGIN_RIGHT_TRANS_PARTNAME = "marginRightTrans";
        /// <summary>
        /// 书壳
        /// </summary>
        const string BOOK_CONTAINER_PARTNAME = "bookContainer";
        const string GRD_CONTENT_PARTNAME = "grdContent";
        #endregion

        #region DependencyProperties
        #region DelayLoad
        public TimeSpan DelayLoad
        {
            get { return (TimeSpan)GetValue(DelayLoadProperty); }
            set { SetValue(DelayLoadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DelayLoad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayLoadProperty =
            DependencyProperty.Register("DelayLoad", typeof(TimeSpan), typeof(FlipBookControl), new PropertyMetadata(TimeSpan.FromSeconds(0)));
        #endregion

        #region BookBackgroundBrush
        public ImageBrush BookBackgroundBrush
        {
            get { return (ImageBrush)GetValue(BookBackgroundBrushProperty); }
            set { SetValue(BookBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BookBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookBackgroundBrushProperty =
            DependencyProperty.Register("BookBackgroundBrush", typeof(ImageBrush), typeof(FlipBookControl), new PropertyMetadata(null, OnBookBackgroundBrushChangedCallBack));

        private static async void OnBookBackgroundBrushChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var ctrl = (d as FlipBookControl);
            if (ctrl.isLoaded)
                await ctrl.GetCropBrush();
        }
        #endregion

        #region Speed
        /// <summary>
        /// 动画速度 默认为30pixel
        /// </summary>
        public int Speed
        {
            get { return (int)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Speed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(int), typeof(FlipBookControl), new PropertyMetadata(30));
        #endregion

        #region BookContainerSource
        public ImageSource BookContainerSource
        {
            get { return (ImageSource)GetValue(BookContainerSourceProperty); }
            set { SetValue(BookContainerSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BookContainerSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BookContainerSourceProperty =
            DependencyProperty.Register("BookContainerSource", typeof(ImageSource), typeof(FlipBookControl), new PropertyMetadata(null, OnBookContainerSourceChangedCallBack));

        private static void OnBookContainerSourceChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (d as FlipBookControl);
            if (null != ctrl.BookContainerSource && ctrl.bookContainer != null && ctrl.isLoaded)
                ctrl.bookContainer.Source = ctrl.BookContainerSource;
        }

        #endregion

        #region PageIndex
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(FlipBookControl), new PropertyMetadata(0, OnPageIndexChangedCallBack));

        private static void OnPageIndexChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (d as FlipBookControl);
            if (ctrl.isLoaded)
            {
                var isNext = Convert.ToInt32(e.NewValue) > Convert.ToInt32(e.OldValue);
                var presenters = ctrl.GetPresentersByPageIndex(isNext);
                if (null != presenters)
                    ctrl.LoadPageContentByPageIndex(Convert.ToInt32(e.NewValue), isNext, presenters[0], presenters[1]);
            }
        }
        #endregion

        #region DisposeAction
        public Action<object> DisposeAction
        {
            get { return (Action<object>)GetValue(DisposeActionProperty); }
            set { SetValue(DisposeActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisposeAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisposeActionProperty =
            DependencyProperty.Register("DisposeAction", typeof(Action<object>), typeof(FlipBookControl), new PropertyMetadata(null));
        #endregion

        #region RestoreItemAction
        public Action<object> RestoreItemAction
        {
            get { return (Action<object>)GetValue(RestoreItemActionProperty); }
            set { SetValue(RestoreItemActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RestoreItemAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RestoreItemActionProperty =
            DependencyProperty.Register("RestoreItemAction", typeof(Action<object>), typeof(FlipBookControl), new PropertyMetadata(null));
        #endregion

        #region CanScale
        public bool CanScale
        {
            get { return (bool)GetValue(CanScaleProperty); }
            set { SetValue(CanScaleProperty, value); }
        }

        // 能否进行放大缩小
        public static readonly DependencyProperty CanScaleProperty =
            DependencyProperty.Register("CanScale", typeof(bool), typeof(FlipBookControl), new PropertyMetadata(false));
        #endregion
        #endregion

        #region Event
        /// <summary>
        /// 翻书结束事件
        /// </summary>
        private delegate void Fliped(object sender, FlipEventArgs args);

        private event Fliped Fliping;
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void NeedLoadItems(object sender, FlipLoadArgs args);

        public event NeedLoadItems NeedLoadingItem;
        #endregion

        #region Constructor
        public FlipBookControl()
        {
            this.DefaultStyleKey = typeof(FlipBookControl);
            this.Loaded += FlipBookControlLoaded;
            this.Unloaded += FlipBookControlUnLoaded;
            CompositionTarget.Rendering += RenderAnimation;
            this.Fliping += FlipEnded;
        }

        /// <summary>
        /// 初始化完毕开始载入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FlipBookControlLoaded(object sender, RoutedEventArgs e)
        {
            grdContent.ManipulationMode =
              ManipulationModes.TranslateInertia |
              ManipulationModes.TranslateX |
              ManipulationModes.Scale |
              ManipulationModes.ScaleInertia |
              ManipulationModes.TranslateY;
            grdContent.DoubleTapped += grdContent_DoubleTapped;
            grdContent.ManipulationDelta += FlipManipulationDelta;
            grdContent.ManipulationCompleted += FlipManipulationCompleted;
            grdContent.ManipulationStarting += FlipManipulationStarting;
            grdContent.ManipulationInertiaStarting += FlipManipulationInertiaStarting;
            A.PointerPressed += PointerPressed;
            B.PointerPressed += PointerPressed;
            C.PointerPressed += PointerPressed;
            D.PointerPressed += PointerPressed;
            E.PointerPressed += PointerPressed;
            F.PointerPressed += PointerPressed;
            offsetWidth = A.ActualWidth;
            offsetHeight = A.ActualHeight;
            await GetCropBrush();
            bookContainer.Source = BookContainerSource;
            RefreshPageByStatus();
            InitPages();
            isLoaded = true;
        }

        void grdContent_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _compositeTransform.CenterX = 0;
            _compositeTransform.CenterY = 0;
            _compositeTransform.ScaleX = 1;
            _compositeTransform.ScaleY = 1;
            _compositeTransform.TranslateX = 0;
            _compositeTransform.TranslateY = 0;
            RefreshPageByStatus();
        }

        private void FlipBookControlUnLoaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= RenderAnimation;
            grdContent.ManipulationDelta -= FlipManipulationDelta;
            grdContent.ManipulationCompleted -= FlipManipulationCompleted;
            grdContent.ManipulationStarting -= FlipManipulationStarting;
            grdContent.ManipulationInertiaStarting -= FlipManipulationInertiaStarting;
            this.Fliping -= FlipEnded;
            A.PointerPressed -= PointerPressed;
            B.PointerPressed -= PointerPressed;
            C.PointerPressed -= PointerPressed;
            D.PointerPressed -= PointerPressed;
            E.PointerPressed -= PointerPressed;
            F.PointerPressed -= PointerPressed;
        }

        public void InitPosition()
        {
            _compositeTransform = new CompositeTransform();
            _previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };
            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_previousTransform);
            _transformGroup.Children.Add(_compositeTransform);
            this.RenderTransform = _transformGroup;
        }

        public async void InitPages()
        {
            if (!isInit && PageIndex == 0 && this.Items.Count > 0)
            {
                await Task.Delay(DelayLoad);
                List<object> needLoadItems = new List<object>();
                //第一次加载 载入4页
                CPresenter.DataContext = this.Items[0];
                needLoadItems.Add(Items[0]);
                if (this.Items.Count > 1)
                {
                    DPresenter.DataContext = this.Items[1];
                    needLoadItems.Add(Items[1]);
                }
                if (this.Items.Count > 2)
                {
                    EPresenter.DataContext = this.Items[2];
                    needLoadItems.Add(Items[2]);
                }
                if (this.Items.Count > 3)
                {
                    FPresenter.DataContext = this.Items[3];
                    needLoadItems.Add(Items[3]);
                }
                if (null != NeedLoadingItem)
                    NeedLoadingItem(this, new FlipLoadArgs(needLoadItems, false));
                isInit = true;
            }
            else
            {
                Status = 0;
                this.PageIndex = 0;
                if (null != APresenter)
                {
                    APresenter.DataContext = null;
                    BPresenter.DataContext = null;
                    CPresenter.DataContext = null;
                    DPresenter.DataContext = null;
                    EPresenter.DataContext = null;
                    FPresenter.DataContext = null;
                }
            }
            InitPosition();
        }
        #endregion

        #region EventMethod
        #region OnApplyTemplate
        protected override void OnApplyTemplate()
        {
            bookContainer = GetTemplateChild(BOOK_CONTAINER_PARTNAME) as Image;
            A = GetTemplateChild(A_PARTNAME) as Border;
            B = GetTemplateChild(B_PARTNAME) as Border;
            C = GetTemplateChild(C_PARTNAME) as Border;
            D = GetTemplateChild(D_PARTNAME) as Border;
            E = GetTemplateChild(E_PARTNAME) as Border;
            F = GetTemplateChild(F_PARTNAME) as Border;
            APresenter = GetTemplateChild(A_PRESENTER_PARTNAME) as ContentPresenter;
            BPresenter = GetTemplateChild(B_PRESENTER_PARTNAME) as ContentPresenter;
            CPresenter = GetTemplateChild(C_PRESENTER_PARTNAME) as ContentPresenter;
            DPresenter = GetTemplateChild(D_PRESENTER_PARTNAME) as ContentPresenter;
            EPresenter = GetTemplateChild(E_PRESENTER_PARTNAME) as ContentPresenter;
            FPresenter = GetTemplateChild(F_PRESENTER_PARTNAME) as ContentPresenter;
            ARect = GetTemplateChild(A_RECT_PARTNAME) as RectangleGeometry;
            BRect = GetTemplateChild(B_RECT_PARTNAME) as RectangleGeometry;
            CRect = GetTemplateChild(C_RECT_PARTNAME) as RectangleGeometry;
            DRect = GetTemplateChild(D_RECT_PARTNAME) as RectangleGeometry;
            ERect = GetTemplateChild(E_RECT_PARTNAME) as RectangleGeometry;
            FRect = GetTemplateChild(F_RECT_PARTNAME) as RectangleGeometry;
            transA = GetTemplateChild(A_TRANS_PARTNAME) as CompositeTransform;
            transB = GetTemplateChild(B_TRANS_PARTNAME) as CompositeTransform;
            transC = GetTemplateChild(C_TRANS_PARTNAME) as CompositeTransform;
            transD = GetTemplateChild(D_TRANS_PARTNAME) as CompositeTransform;
            transE = GetTemplateChild(E_TRANS_PARTNAME) as CompositeTransform;
            transF = GetTemplateChild(F_TRANS_PARTNAME) as CompositeTransform;
            grdContent = GetTemplateChild(GRD_CONTENT_PARTNAME) as Grid;
            innerLeftTrans = GetTemplateChild(INNER_LEFT_TRANS_PARTNAME) as CompositeTransform;
            outerLeftTrans = GetTemplateChild(OUTER_LEFT_TRANS_PARTNAME) as CompositeTransform;
            innerRightTrans = GetTemplateChild(INNER_RIGHT_TRANS_PARTNAME) as CompositeTransform;
            outerRightTrans = GetTemplateChild(OUTER_RIGHT_TRANS_PARTNAME) as CompositeTransform;
            marginLeftTrans = GetTemplateChild(MARGIN_LEFT_TRANS_PARTNAME) as CompositeTransform;
            marginRightTrans = GetTemplateChild(MARGIN_RIGHT_TRANS_PARTNAME) as CompositeTransform;
            stShadowSplitInnerLeft = GetTemplateChild(ST_SHADOW_SPLIT_INNER_LEFT_PARTNAME) as StackPanel;
            stShadowSplitOuterLeft = GetTemplateChild(ST_SHADOW_SPLIT_OUTER_LEFT_PARTNAME) as StackPanel;
            stShadowSplitInnerRight = GetTemplateChild(ST_SHADOW_SPLIT_INNER_RIGHT_PARTNAME) as StackPanel;
            stShadowSplitOuterRight = GetTemplateChild(ST_SHADOW_SPLIT_OUTER_RIGHT_PARTNAME) as StackPanel;
            stShadowMarginLeft = GetTemplateChild(ST_SHADOW_MARGIN_LEFT_PARTNAME) as StackPanel;
            stShadowMarginRight = GetTemplateChild(ST_SHADOW_MARGIN_RIGHT_PARTNAME) as StackPanel;
            base.OnApplyTemplate();
        }
        #endregion

        #region PointerPressed
        /// <summary>
        /// 确定翻页方向
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private new void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!isManipulating)
            {
                if (sender.Equals(leftPage))
                    isNext = false;
                else if (sender.Equals(rightPage))
                    isNext = true;
                else
                    RefreshPageByStatus();
            }
        }
        #endregion

        #region OnPointerReleased
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
        }
        #endregion

        #region FlipEnded
        /// <summary>
        /// 翻页完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void FlipEnded(object sender, FlipEventArgs args)
        {
            if (args.isNext) PageIndex += 2;
            else PageIndex -= 2;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            RefreshPageByStatus());
        }
        #endregion

        #region Manipulation  多点触控操作翻书特效
        #region FlipManipulationStarting
        private void FlipManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            isManipulating = true;
            e.Handled = true;
            if (isNext)
            {
                if (PageIndex >= this.Items.Count - 2)
                {
                    isFlip = false;
                    isManipulating = false;
                }
            }
            else
            {
                if (this.PageIndex == 0)
                {
                    isFlip = false;
                    isManipulating = false;
                }
            }
        }
        #endregion

        #region FlipManipulationInertiaStarting
        private void FlipManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 5 * 96.0 / (1000.0 * 1000.0);
            e.ExpansionBehavior.DesiredDeceleration = 100 * 96 / 1000.0 * 1000.0;
        }
        #endregion

        #region FlipManipulationCompleted
        private async void FlipManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            isManipulating = false;
            if (isFlip)
            {
                IsHitVisible(false);
                var leftTopOffset = leftTopPage.Clip.Rect.X;
                var rightTopOffset = rightTopPage.Clip.Rect.X;
                await Task.Run(() =>
                {
                    if (isNext)
                    {
                        if (lastDeltaOffset < 0)
                        {
                            Status = Status < 2 ? Status + 1 : 0;
                            turnRight = true;
                        }
                        else if (rightTopOffset != 0.0)
                            rightRestore = true;
                        else IsHitVisible(true);
                    }
                    else
                    {
                        if (lastDeltaOffset > 0)
                        {
                            Status = Status > 0 ? Status - 1 : 2;
                            turnLeft = true;
                        }
                        else if (leftTopOffset != 0.0)
                            leftRestore = true;
                        else IsHitVisible(true);
                    }
                });
            }
            isFlip = true;
            CanScale = false;
        }
        #endregion

        #region FlipManipulationDelta
        private void FlipManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var scale = e.Delta.Scale;
            var translateX = e.Delta.Translation.X;
            var translateY = e.Delta.Translation.Y;
            var nTtranX = nextTrans.TranslateX;
            var nTtranY = nextTrans.TranslateY;
            var ctranlateX = e.Cumulative.Translation.X;
            var ctranlateY = e.Cumulative.Translation.Y;
            CanScale = Task.Run(() =>
            {
                if (scale != 1.0 || (Math.Abs(ctranlateX * 1.15) < Math.Abs(ctranlateY))) return true;
                else return false;
            }).Result;
            if (isManipulating && !CanScale)
            {
                if (isNext)
                {
                    #region 下一页
                    var rightTopNect = rightTopPage.Clip.Rect;
                    var nextRect = nextPage.Clip.Rect;
                    var nextTransOffset = nTtranX + translateX * 2;

                    SetShadowOperacity(Math.Abs(nextTransOffset), offsetWidth, false);
                    var nextRectXOffset = nextRect.X - e.Delta.Translation.X;
                    lastDeltaOffset = e.Delta.Translation.X;
                    if (nextRectXOffset < 0 && nextRectXOffset > -offsetWidth)
                    {
                        outerRightTrans.TranslateX += e.Delta.Translation.X;
                        innerRightTrans.TranslateX += e.Delta.Translation.X;
                        marginRightTrans.TranslateX += e.Delta.Translation.X * 2;

                        nextTrans.TranslateX = nextTransOffset;
                        if (nextRectXOffset < 0)
                            nextRect.X = nextRectXOffset;
                        rightTopNect.X += e.Delta.Translation.X;
                        nextPage.Clip.Rect = nextRect;
                        rightTopPage.Clip.Rect = rightTopNect;
                    }
                    else
                    {
                        e.Complete();
                        if (nextTransOffset < 0)
                        {
                            nextTrans.TranslateX = -offsetWidth;
                            nextRect.X = 0;
                            rightTopNect.X = 0;
                            nextPage.Clip.Rect = nextRect;
                            rightTopPage.Clip.Rect = rightTopNect;
                        }
                        else
                        {
                            nextTrans.TranslateX = offsetWidth;
                            nextRect.X = -offsetWidth;
                            rightTopNect.X = offsetWidth;
                            nextPage.Clip.Rect = nextRect;
                            rightTopPage.Clip.Rect = rightTopNect;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 上一页
                    var leftTopNect = leftTopPage.Clip.Rect;
                    var prevRect = prevPage.Clip.Rect;
                    var prevTransOffset = prevTrans.TranslateX + e.Delta.Translation.X * 2;
                    var prevRectXOffset = prevRect.X - e.Delta.Translation.X;
                    SetShadowOperacity(Math.Abs(prevTransOffset), offsetWidth, true);
                    lastDeltaOffset = e.Delta.Translation.X;
                    if (prevRectXOffset > 0 && prevRectXOffset < offsetWidth)
                    {
                        innerLeftTrans.TranslateX += translateX;
                        outerLeftTrans.TranslateX += translateX;
                        marginLeftTrans.TranslateX += translateX * 2;

                        prevTrans.TranslateX = prevTransOffset;
                        if (prevRectXOffset > 0)
                            prevRect.X = prevRectXOffset;
                        leftTopNect.X += e.Delta.Translation.X;
                        prevPage.Clip.Rect = prevRect;
                        leftTopPage.Clip.Rect = leftTopNect;
                    }
                    else
                    {
                        e.Complete();
                        if (prevTransOffset < 0)
                        {
                            prevTrans.TranslateX = -offsetWidth;
                            prevRect.X = offsetWidth;
                            leftTopNect.X = -offsetWidth;
                            prevPage.Clip.Rect = prevRect;
                            leftTopPage.Clip.Rect = leftTopNect;

                        }
                        else
                        {
                            prevTrans.TranslateX = offsetWidth;
                            prevRect.X = 0;
                            leftTopNect.X = 0;
                            prevPage.Clip.Rect = prevRect;
                            leftTopPage.Clip.Rect = leftTopNect;
                        }
                    }
                    #endregion
                }
            }
            if (CanScale)
            {
                _previousTransform.Matrix = _transformGroup.Value;
                Point center = _previousTransform.TransformPoint(new Point(e.Position.X, e.Position.Y));
                _compositeTransform.CenterX = center.X;
                _compositeTransform.CenterY = center.Y;
                _compositeTransform.ScaleX = e.Delta.Scale;
                _compositeTransform.ScaleY = e.Delta.Scale;
                _compositeTransform.TranslateX = e.Delta.Translation.X;
                _compositeTransform.TranslateY = e.Delta.Translation.Y;
            }
        }
        #endregion
        #endregion

        #region RenderAnimation 绘制翻页动画
        private void RenderAnimation(object sender, object e)
        {
            if (turnLeft)
            {
                rightRestore = false;
                turnRight = false;
                var prevRect = prevPage.Clip.Rect;
                var leftTopRect = leftTopPage.Clip.Rect;
                var prevOffset = prevRect.X - Speed / 2;
                if (prevOffset > 0)
                {
                    prevRect.X = prevOffset;
                    prevTrans.TranslateX += Speed;
                    leftTopRect.X += Speed / 2;
                    innerLeftTrans.TranslateX += Speed / 2;
                    outerLeftTrans.TranslateX += Speed / 2;
                    marginLeftTrans.TranslateX += Speed;
                    SetShadowOperacity(Math.Abs(prevTrans.TranslateX), offsetWidth, true);
                }
                else
                {
                    prevRect.X = 0;
                    leftTopRect.X = 0;
                    turnLeft = false;
                    prevTrans.TranslateX = offsetWidth;
                    Fliping(sender, new FlipEventArgs(false));

                }
                prevPage.Clip.Rect = prevRect;
                leftTopPage.Clip.Rect = leftTopRect;
                IsHitVisible(true);
            }
            else if (leftRestore)
            {
                turnLeft = false;
                turnRight = false;
                rightRestore = false;
                var prevRect = prevPage.Clip.Rect;
                var leftTopRect = leftTopPage.Clip.Rect;
                var prevOffset = prevRect.X + Speed / 2;
                if (prevOffset < offsetWidth)
                {
                    prevRect.X = prevOffset;
                    prevTrans.TranslateX -= Speed;
                    leftTopRect.X -= Speed / 2;
                    innerLeftTrans.TranslateX -= Speed / 2;
                    outerLeftTrans.TranslateX -= Speed / 2;
                    marginLeftTrans.TranslateX -= Speed;
                    SetShadowOperacity(Math.Abs(prevTrans.TranslateX), offsetWidth, true);
                }
                else
                {
                    prevRect.X = offsetWidth;
                    leftTopRect.X = -offsetWidth;
                    prevTrans.TranslateX = -offsetWidth;
                    innerLeftTrans.TranslateX = 0;
                    outerLeftTrans.TranslateX = 0;
                    marginLeftTrans.TranslateX = 0;
                    leftRestore = false;

                }
                prevPage.Clip.Rect = prevRect;
                leftTopPage.Clip.Rect = leftTopRect;
                IsHitVisible(true);
            }
            else if (turnRight)
            {

                rightRestore = false;
                turnLeft = false;
                var nextRect = nextPage.Clip.Rect;
                var rightTopRect = rightTopPage.Clip.Rect;
                var nextOffset = nextRect.X + Speed / 2;
                if (nextOffset < 0)
                {
                    nextRect.X = nextOffset;
                    nextTrans.TranslateX -= Speed;
                    rightTopRect.X -= Speed / 2;
                    innerRightTrans.TranslateX -= Speed / 2;
                    outerRightTrans.TranslateX -= Speed / 2;
                    marginRightTrans.TranslateX -= Speed;
                    SetShadowOperacity(Math.Abs(nextTrans.TranslateX), offsetWidth, false);
                }
                else
                {
                    nextRect.X = 0;
                    nextTrans.TranslateX = -offsetWidth;
                    turnRight = false;
                    rightTopRect.X = 0;
                    Fliping(sender, new FlipEventArgs(true));
                }
                nextPage.Clip.Rect = nextRect;
                rightTopPage.Clip.Rect = rightTopRect;
                IsHitVisible(true);
            }
            else if (rightRestore)
            {

                turnRight = false;
                turnLeft = false;
                leftRestore = false;
                var nextRect = nextPage.Clip.Rect;
                var rightTopRect = rightTopPage.Clip.Rect;
                var nextOffset = nextRect.X - Speed / 2;
                if (nextRect.X - Speed / 2 > -offsetWidth)
                {
                    nextRect.X = nextOffset;
                    nextTrans.TranslateX += Speed;
                    rightTopRect.X += Speed / 2;
                    innerRightTrans.TranslateX += Speed / 2;
                    outerRightTrans.TranslateX += Speed / 2;
                    marginRightTrans.TranslateX += Speed;
                    SetShadowOperacity(Math.Abs(nextTrans.TranslateX), offsetWidth, false);
                }
                else
                {
                    nextRect.X = -offsetWidth;
                    rightTopRect.X = offsetWidth;
                    nextTrans.TranslateX = offsetWidth;
                    innerRightTrans.TranslateX = 0;
                    outerRightTrans.TranslateX = 0;
                    marginRightTrans.TranslateX = 0;
                    rightRestore = false;
                }
                rightTopPage.Clip.Rect = rightTopRect;
                nextPage.Clip.Rect = nextRect;
                IsHitVisible(true);
            }
        }
        #endregion
        #endregion

        #region Method
        #region LoadPageContentByPageIndex
        private void LoadPageContentByPageIndex(int PageIndex, bool isNextOrPrev, ContentPresenter firstPresenter, ContentPresenter secondPresenter)
        {
            List<object> needLoadItems = new List<object>();
            if (isNextOrPrev)
            {
                //加载下一页模板
                if (PageIndex + 2 < this.Items.Count)
                {
                    firstPresenter.Content = null;
                    firstPresenter.DataContext = null;
                    object item = null;
                    if (this.Items.Count > PageIndex + 2)
                    {
                        item = this.Items[PageIndex + 2];
                        needLoadItems.Add(item);
                        firstPresenter.DataContext = item;
                    }
                }
                else firstPresenter.DataContext = null;
                if (PageIndex + 3 < this.Items.Count)
                {
                    object item = null;
                    secondPresenter.Content = null;
                    secondPresenter.DataContext = null;
                    if (this.Items.Count > PageIndex + 3)
                    {
                        item = this.Items[PageIndex + 3];
                        needLoadItems.Add(item);
                        secondPresenter.DataContext = item;
                    }
                }
                else secondPresenter.DataContext = null;
                if (null != NeedLoadingItem)
                    NeedLoadingItem(this, new FlipLoadArgs(needLoadItems, true));
                RecycleData(true, needLoadItems);
            }
            else
            {
                if (PageIndex - 2 >= 0 && Items.Count > PageIndex - 2)
                {
                    needLoadItems.Add(this.Items[PageIndex - 2]);
                    secondPresenter.Content = null;
                    secondPresenter.DataContext = null;
                    secondPresenter.DataContext = this.Items[PageIndex - 2];
                }
                if (PageIndex - 1 >= 0 && Items.Count > PageIndex - 1)
                {
                    firstPresenter.Content = null;
                    firstPresenter.DataContext = null;
                    firstPresenter.DataContext = this.Items[PageIndex - 1];
                    needLoadItems.Add(this.Items[PageIndex - 1]);
                }
                //加载上一页模板
                if (null != NeedLoadingItem)
                    NeedLoadingItem(this, new FlipLoadArgs(needLoadItems, false));
                RecycleData(false, needLoadItems);
            }
        }
        #endregion

        #region RecycleData
        private async void RecycleData(bool isNext, List<object> needItems)
        {
            await Task.Run(async () =>
            {
                foreach (var o in needItems)
                {
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (null != this.RestoreItemAction)
                            this.RestoreItemAction.Invoke(o);
                    });
                }
                if (isNext)
                {
                    var index = -1;
                    try
                    {
                        index = this.Items.IndexOf(needItems[0]);

                    }
                    catch
                    {
                        index = -1;
                    }
                    if (index != -1 && index - 8 > 0)
                    {
                        for (int i = index - 8; i < index - 6; i++)
                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                            {
                                if (null != this.DisposeAction)
                                    DisposeAction.Invoke(this.Items[i]);
                            });
                    }
                }
                else
                {
                    var index = -1;
                    try
                    {
                        index = this.Items.IndexOf(needItems.Last());

                    }
                    catch (Exception ex)
                    {
                        index = -1;
                    }
                    if (index != -1 && this.Items.Count > index + 7)
                    {
                        for (int i = index + 5; i < index + 7; i++)
                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                            {
                                if (null != this.DisposeAction)
                                    DisposeAction.Invoke(this.Items[i]);
                            });
                    }
                }
            });
        }
        #endregion

        #region OnItemsChanged
        /// <summary>
        /// 刷新数据 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            isInit = false;
            InitPages();
            base.OnItemsChanged(e);
        }
        #endregion

        #region GetPresentersByPageIndex
        private List<ContentPresenter> GetPresentersByPageIndex(bool isNext)
        {
            List<ContentPresenter> presenters = new List<ContentPresenter>();
            if (isNext)
            {
                presenters.Add(leftTopPage.Child as ContentPresenter);
                presenters.Add(prevPage.Child as ContentPresenter);
            }
            else
            {
                presenters.Add(rightTopPage.Child as ContentPresenter);
                presenters.Add(nextPage.Child as ContentPresenter);
            }
            Debug.WriteLine("presenter0Name:" + presenters[0].Name);
            Debug.WriteLine("presenter1Name:" + presenters[1].Name);
            return presenters;
        }
        #endregion

        #region Crop
        /// <summary>
        /// 图形切割
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public WriteableBitmap Crop(WriteableBitmap source, int x1, int y1, int x2, int y2)
        {
            if (x1 >= x2 ||
                y1 >= y2 ||
                x1 < 0 ||
                y1 < 0 ||
                x2 < 0 ||
                y2 < 0 ||
                x1 > source.PixelWidth ||
                y1 > source.PixelHeight ||
                x2 > source.PixelWidth ||
                y2 > source.PixelHeight)
            {
                throw new ArgumentException();
            }

            //var buffer = source.PixelBuffer.GetPixels();
            var cw = x2 - x1;
            var ch = y2 - y1;
            var target = new WriteableBitmap(cw, ch);

            var croppedBytes =
                new byte[4 * cw * ch];
            var inputStream = source.PixelBuffer.AsStream();
            inputStream.Seek(4 * (source.PixelWidth * y1 + x1), SeekOrigin.Current);
            for (int i = 0; i < ch; i++)
            {
                inputStream.Read(croppedBytes, 4 * cw * i, 4 * cw);
                inputStream.Seek(4 * (source.PixelWidth - cw), SeekOrigin.Current);
            }

            var outputStream = target.PixelBuffer.AsStream();
            outputStream.Seek(0, SeekOrigin.Begin);
            outputStream.Write(croppedBytes, 0, croppedBytes.Length);
            target.Invalidate();

            return target;
        }
        #endregion

        #region IsHitVisible
        /// <summary>
        /// 禁止点击
        /// </summary>
        /// <param name="o"></param>
        private async void IsHitVisible(bool o)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                this.grdContent.IsHitTestVisible = o;
                this.leftPage.IsHitTestVisible = o;
                this.rightPage.IsHitTestVisible = o;
                this.nextPage.IsHitTestVisible = o;
                this.prevPage.IsHitTestVisible = o;
                this.leftTopPage.IsHitTestVisible = o;
                this.rightTopPage.IsHitTestVisible = o;
            });
        }
        #endregion

        #region RefreshPageByStatus
        /// <summary>
        /// 翻页成功后刷新控件状态
        /// </summary>
        private void RefreshPageByStatus()
        {

            switch (Status)
            {
                case 0:
                    Canvas.SetZIndex(A, 2);
                    Canvas.SetZIndex(B, 2);
                    Canvas.SetZIndex(C, 0);
                    Canvas.SetZIndex(D, 0);
                    Canvas.SetZIndex(E, 2);
                    Canvas.SetZIndex(F, 2);
                    Grid.SetColumn(A, 1);
                    Grid.SetColumn(B, 1);
                    Grid.SetColumn(C, 1);
                    Grid.SetColumn(D, 2);
                    Grid.SetColumn(E, 2);
                    Grid.SetColumn(F, 2);
                    transA.TranslateX = 0;
                    transB.TranslateX = -offsetWidth;
                    transC.TranslateX = 0;
                    transD.TranslateX = 0;
                    transE.TranslateX = offsetWidth;
                    transF.TranslateX = 0;
                    ARect.Rect = new Rect(-this.A.ActualWidth, 0, this.A.ActualWidth, this.A.ActualHeight);
                    CRect.Rect = new Rect(0, 0, this.C.ActualWidth, this.C.ActualHeight);
                    DRect.Rect = new Rect(0, 0, this.D.ActualWidth, this.D.ActualHeight);
                    FRect.Rect = new Rect(this.F.ActualWidth, 0, this.F.ActualWidth, this.F.ActualHeight);
                    BRect.Rect = new Rect(this.B.ActualWidth, 0, this.B.ActualWidth, this.B.ActualHeight);
                    ERect.Rect = new Rect(-this.E.ActualWidth, 0, this.E.ActualWidth, this.E.ActualHeight);
                    nextPage = E;
                    prevPage = B;
                    leftPage = C;
                    rightPage = D;
                    leftTopPage = A;
                    rightTopPage = F;
                    nextTrans = transE;
                    prevTrans = transB;
                    //A.PointerPressed -= this.PointerPressed;
                    //B.PointerPressed -= this.PointerPressed;
                    //C.PointerPressed += this.PointerPressed;
                    //D.PointerPressed += this.PointerPressed;
                    break;
                case 1:
                    Canvas.SetZIndex(A, 2);
                    Canvas.SetZIndex(B, 2);
                    Canvas.SetZIndex(C, 2);
                    Canvas.SetZIndex(D, 2);
                    Canvas.SetZIndex(E, 0);
                    Canvas.SetZIndex(F, 0);
                    Grid.SetColumn(A, 2);
                    Grid.SetColumn(B, 2);
                    Grid.SetColumn(C, 1);
                    Grid.SetColumn(D, 1);
                    Grid.SetColumn(E, 1);
                    Grid.SetColumn(F, 2);
                    transA.TranslateX = offsetWidth;
                    transB.TranslateX = 0;
                    transC.TranslateX = 0;
                    transD.TranslateX = -offsetWidth;
                    transE.TranslateX = 0;
                    transF.TranslateX = 0;
                    ARect.Rect = new Rect(-this.A.ActualWidth, 0, this.A.ActualWidth, this.A.ActualHeight);
                    CRect.Rect = new Rect(-this.C.ActualWidth, 0, this.C.ActualWidth, this.C.ActualHeight);
                    DRect.Rect = new Rect(this.D.ActualWidth, 0, this.D.ActualWidth, this.D.ActualHeight);
                    FRect.Rect = new Rect(0, 0, this.F.ActualWidth, this.F.ActualHeight);
                    BRect.Rect = new Rect(this.B.ActualWidth, 0, this.B.ActualWidth, this.B.ActualHeight);
                    ERect.Rect = new Rect(0, 0, this.E.ActualWidth, this.E.ActualHeight);
                    nextPage = A;
                    prevPage = D;
                    leftPage = E;
                    rightPage = F;
                    leftTopPage = C;
                    rightTopPage = B;
                    nextTrans = transA;
                    prevTrans = transD;
                    //C.PointerPressed -= this.PointerPressed;
                    //D.PointerPressed -= this.PointerPressed;
                    //E.PointerPressed += this.PointerPressed;
                    //F.PointerPressed += this.PointerPressed;
                    break;
                case 2:
                    Canvas.SetZIndex(A, 0);
                    Canvas.SetZIndex(B, 0);
                    Canvas.SetZIndex(C, 2);
                    Canvas.SetZIndex(D, 2);
                    Canvas.SetZIndex(E, 2);
                    Canvas.SetZIndex(F, 2);
                    Grid.SetColumn(A, 1);
                    Grid.SetColumn(B, 2);
                    Grid.SetColumn(C, 2);
                    Grid.SetColumn(D, 2);
                    Grid.SetColumn(E, 1);
                    Grid.SetColumn(F, 1);
                    transA.TranslateX = 0;
                    transB.TranslateX = 0;
                    transC.TranslateX = offsetWidth;
                    transD.TranslateX = 0;
                    transE.TranslateX = 0;
                    transF.TranslateX = -offsetWidth;
                    ARect.Rect = new Rect(0, 0, this.A.ActualWidth, this.A.ActualHeight);
                    CRect.Rect = new Rect(-this.C.ActualWidth, 0, this.C.ActualWidth, this.C.ActualHeight);
                    DRect.Rect = new Rect(this.D.ActualWidth, 0, this.D.ActualWidth, this.D.ActualHeight);
                    FRect.Rect = new Rect(this.F.ActualWidth, 0, this.F.ActualWidth, this.F.ActualHeight);
                    BRect.Rect = new Rect(0, 0, this.B.ActualWidth, this.B.ActualHeight);
                    ERect.Rect = new Rect(-this.E.ActualWidth, 0, this.E.ActualWidth, this.E.ActualHeight);
                    nextPage = C;
                    prevPage = F;
                    leftPage = A;
                    rightPage = B;
                    leftTopPage = E;
                    rightTopPage = D;
                    nextTrans = transC;
                    prevTrans = transF;
                    //E.PointerPressed -= this.PointerPressed;
                    //F.PointerPressed -= this.PointerPressed;
                    //A.PointerPressed += this.PointerPressed;
                    //B.PointerPressed += this.PointerPressed;
                    break;
                default:
                    break;
            }
            stShadowSplitInnerLeft.Opacity = 0;
            stShadowSplitInnerRight.Opacity = 0;
            stShadowSplitOuterLeft.Opacity = 0;
            stShadowSplitOuterRight.Opacity = 0;
            outerRightTrans.TranslateX = 0;
            innerRightTrans.TranslateX = 0;
            outerLeftTrans.TranslateX = 0;
            innerLeftTrans.TranslateX = 0;
            marginLeftTrans.TranslateX = 0;
            marginRightTrans.TranslateX = 0;
            leftTopPage.Background = leftBrush;
            prevPage.Background = rightBrush;
            leftPage.Background = leftBrush;
            rightPage.Background = rightBrush;
            nextPage.Background = leftBrush;
            rightTopPage.Background = rightBrush;
        }
        #endregion

        #region GetCropBookBrush
        private async Task GetCropBrush()
        {
            if (null != this.BookBackgroundBrush)
            {
                var orginSource = this.BookBackgroundBrush.ImageSource as BitmapImage;
                if (!orginSource.UriSource.AbsolutePath.Equals(string.Empty))
                {
                    var uri = new Uri("ms-appx://" + orginSource.UriSource.AbsolutePath);
                    try
                    {
                        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                        WriteableBitmap leftSource = new WriteableBitmap(Convert.ToInt32(offsetWidth * 2), Convert.ToInt32(offsetHeight));
                        await LoadAsync(leftSource, file);
                        WriteableBitmap rightSource = new WriteableBitmap(Convert.ToInt32(offsetWidth * 2), Convert.ToInt32(offsetHeight));
                        await LoadAsync(rightSource, file);
                        leftBrush = new ImageBrush();
                        rightBrush = new ImageBrush();
                        rightBrush.Stretch = Stretch.Fill;
                        leftBrush.Stretch = Stretch.Fill;
                        leftSource = Crop(leftSource, 0, 0, Convert.ToInt32(offsetWidth), Convert.ToInt32(offsetHeight));
                        leftBrush.ImageSource = leftSource;
                        rightSource = Crop(rightSource, Convert.ToInt32(offsetWidth), 0, Convert.ToInt32(offsetWidth * 2), Convert.ToInt32(offsetHeight));
                        rightBrush.ImageSource = rightSource;
                    }
                    catch
                    {
                    }
                }
            }
        }
        #endregion

        #region LoadWriteableBitmap
        public async Task<WriteableBitmap> LoadAsync(
            WriteableBitmap writeableBitmap,
           StorageFile storageFile)
        {
            using (var stream = await storageFile.OpenReadAsync())
            {
                await writeableBitmap.SetSourceAsync(
                    stream);
            }
            return writeableBitmap;
        }

        #endregion

        #region SetShadowOperacity
        private async void SetShadowOperacity(double pos, double pageWidth, bool direction)
        {
            var opacity = await Task.Run(() =>
            {
                double num = (pageWidth - pos) / 2.0;
                double num2 = Math.Abs((double)((pageWidth / 2.0) - num));
                return (1.0 * (1.0 - (num2 / (pageWidth / 2.0))));
            });
            if (direction)
            {
                this.stShadowSplitOuterLeft.Opacity = opacity;
                this.stShadowSplitInnerLeft.Opacity = opacity;
                this.stShadowMarginLeft.Opacity = opacity;
            }
            else
            {
                this.stShadowSplitOuterRight.Opacity = opacity;
                this.stShadowSplitInnerRight.Opacity = opacity;
                this.stShadowMarginRight.Opacity = opacity;
            }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// 抛出需要加载的项数据
    /// </summary>
    public class FlipLoadArgs : EventArgs
    {
        public readonly List<object> needItems;
        public readonly bool isNext;

        public FlipLoadArgs(List<object> _needItems, bool _isNext)
        {
            this.needItems = _needItems;
            this.isNext = _isNext;
        }
    }

    public class FlipEventArgs : EventArgs
    {
        public readonly bool isNext;

        public FlipEventArgs(bool _isNext)
        {
            this.isNext = _isNext;
        }
    }
}
