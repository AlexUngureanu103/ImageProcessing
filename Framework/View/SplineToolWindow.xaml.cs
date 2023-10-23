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

        public SplineToolWindow(MainVM mainVM)
        {
            InitializeComponent();

            DataProvider.VectorOfMousePosition.Clear();

            //var initialImage = DataProvider.GrayInitialImage != null ? ImageConverter.Convert( DataProvider.GrayInitialImage) : ImageConverter.Convert(DataProvider.ColorInitialImage);

            _splinetoolVM = new SplineToolVM();
            _splinetoolVM.MainVM = mainVM;

            DataContext = _splinetoolVM;
        }

        public void Update()
        {

            UiHelper.RemoveUiElements(canvasOriginalImage, canvasProcessedImage);
            UiHelper.DrawUiElements(canvasOriginalImage, canvasProcessedImage, _splinetoolVM.ScaleValue);
        }

        private void SetUiValues(Image<Gray, byte> grayImage, Image<Bgr, byte> colorImage, int x, int y)
        {
            _splinetoolVM.XPos = x >= 0 ? "X: " + (x / _splinetoolVM.OriginalCanvasWidth * 255).ToString() : "";
            _splinetoolVM.YPos = y >= 0 ? "Y: " + ((_splinetoolVM.OriginalCanvasHeight - y) / _splinetoolVM.OriginalCanvasHeight * 255).ToString() : "";

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
            if (sender == MyGraph)
            {
                SetUiValues(DataProvider.GrayProcessedImage, DataProvider.ColorProcessedImage, (int)position.X, (int)position.Y);
            }
            else if (sender == ProcessedGraph)
            {
                SetUiValues(DataProvider.GrayProcessedImage, DataProvider.ColorProcessedImage, (int)(position.X / _splinetoolVM.ScaleValue), (int)(position.Y / _splinetoolVM.ScaleValue));
            }
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == MyGraph)
            {
                DataProvider.MousePosition = e.GetPosition(MyGraph);
            }
            else if (sender == ProcessedGraph)
            {
                DataProvider.MousePosition = e.GetPosition(ProcessedGraph);
            }

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
            DrawingHelper.UpdateShapesProperties(canvasProcessedImage, scaleValue);

            if (_splinetoolVM != null)
            {
                _splinetoolVM.OriginalCanvasWidth = MyGraph.ActualWidth * scaleValue;
                _splinetoolVM.OriginalCanvasHeight = MyGraph.ActualHeight * scaleValue;

                _splinetoolVM.ProcessedCanvasWidth = ProcessedGraph.ActualWidth * scaleValue;
                _splinetoolVM.ProcessedCanvasHeight = ProcessedGraph.ActualHeight * scaleValue;
            }
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == scrollViewerInitial)
            {
                scrollViewerInitial.ScrollToVerticalOffset(e.VerticalOffset);
                scrollViewerInitial.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
            else
            {
                scrollViewerProcessed.ScrollToVerticalOffset(e.VerticalOffset);
                scrollViewerProcessed.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataProvider.CloseWindows();
        }
    }
}
