using Framework.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.UiHelper;
using static Framework.Utilities.DrawingHelper;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Framework.View
{
    /// <summary>
    /// Interaction logic for SplineToolWindow.xaml
    /// </summary>
    public partial class SplineToolWindow : Window
    {
        private readonly SplineTollVM _splinetoolVM;

        public SplineToolWindow()
        {
            InitializeComponent();

            _splinetoolVM = new SplineTollVM();
            DataContext = _splinetoolVM;
        }
        
        public void Update()
        {

            RemoveUiElements(canvasOriginalImage, null);
            DrawUiElements(canvasOriginalImage, null, _splinetoolVM.ScaleValue);
        }

        private void SetUiValues(Image<Gray, byte> grayImage, Image<Bgr, byte> colorImage, int x, int y)
        {
            _splinetoolVM.XPos = x >= 0 ? "X: " + x.ToString() : "";
            _splinetoolVM.YPos = y >= 0 ? "Y: " + y.ToString() : "";

            _splinetoolVM.GrayValue = (grayImage != null && y >= 0 && y < grayImage.Height && x >= 0 && x < grayImage.Width) ?
                "Gray: " + grayImage.Data[y, x, 0] : "";
            _splinetoolVM.BlueValue = (colorImage != null && y >= 0 && y < colorImage.Height && x >= 0 && x < colorImage.Width) ?
                "B: " + colorImage.Data[y, x, 0] : "";
            _splinetoolVM.GreenValue = (colorImage != null && y >= 0 && y < colorImage.Height && x >= 0 && x < colorImage.Width) ?
                "G: " + colorImage.Data[y, x, 1] : "";
            _splinetoolVM.RedValue = (colorImage != null && y >= 0 && y < colorImage.Height && x >= 0 && x < colorImage.Width) ?
                "R: " + colorImage.Data[y, x, 2] : "";
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(MyGraph);
            SetUiValues(GrayProcessedImage, ColorProcessedImage, (int)position.X, (int)position.Y);
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MousePosition = e.GetPosition(MyGraph);

            if (LastPosition != MousePosition)
            {
                VectorOfMousePosition.Add(MousePosition);
                LastPosition = MousePosition;
            }
        }

        private void ImageMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            VectorOfMousePosition.Clear();
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Update();

            if (MagnifierOn == true)
                Application.Current.Windows.OfType<MagnifierWindow>().First().Update();
            if (RowColorLevelsOn == true || ColumnColorLevelsOn == true)
                Application.Current.Windows.OfType<ColorLevelsWindow>().All(window => { window.Update(); return true; });
        }

        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scaleValue = sliderZoom.Value;

            UpdateShapesProperties(canvasOriginalImage, scaleValue);

            if (_splinetoolVM != null)
            {
                _splinetoolVM.OriginalCanvasWidth = MyGraph.ActualWidth * scaleValue;
                _splinetoolVM.OriginalCanvasHeight = MyGraph.ActualHeight * scaleValue;
            }
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            scrollViewerInitial.ScrollToVerticalOffset(e.VerticalOffset);
            scrollViewerInitial.ScrollToHorizontalOffset(e.HorizontalOffset);
        }


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWindows();
        }
    }
}
