using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Utilities;
using Framework.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Framework.View
{
    /// <summary>
    /// Interaction logic for SplineToolWindow.xaml
    /// </summary>
    public partial class SplineToolWindow : Window
    {
        private readonly SplineToolVM _splinetoolVM;

        public SplineToolWindow()
        {
            InitializeComponent();

            DataProvider.VectorOfMousePosition.Clear();

            _splinetoolVM = new SplineToolVM();
            DataContext = _splinetoolVM;
        }

        public void Update()
        {

            UiHelper.RemoveUiElements(canvasOriginalImage, null);
            UiHelper.DrawUiElements(canvasOriginalImage, null, _splinetoolVM.ScaleValue);
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
            SetUiValues(DataProvider.GrayProcessedImage, DataProvider.ColorProcessedImage, (int)position.X, (int)position.Y);
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataProvider.MousePosition = e.GetPosition(MyGraph);

            if (DataProvider.LastPosition != DataProvider.MousePosition)
            {
                DataProvider.VectorOfMousePosition.Add(DataProvider.MousePosition);

                DataProvider.LastPosition = DataProvider.MousePosition;
                UiHelper.DrawSplineToolGraphUI(canvasOriginalImage, _splinetoolVM.ScaleValue, DataProvider.VectorOfMousePosition);
            }
        }

        private void ImageMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataProvider.VectorOfMousePosition.Clear();
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Update();

            if (DataProvider.MagnifierOn == true)
                Application.Current.Windows.OfType<MagnifierWindow>().First().Update();
            if (DataProvider.RowColorLevelsOn == true || DataProvider.ColumnColorLevelsOn == true)
                Application.Current.Windows.OfType<ColorLevelsWindow>().All(window => { window.Update(); return true; });
        }

        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scaleValue = sliderZoom.Value;

            DrawingHelper.UpdateShapesProperties(canvasOriginalImage, scaleValue);

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
            DataProvider.CloseWindows();
        }
    }
}
