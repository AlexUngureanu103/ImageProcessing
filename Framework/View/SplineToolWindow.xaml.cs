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

            UiHelper.RemoveUiElements(splineToolCanvasOriginalImage, splineToolCanvasProcessedImage);
            UiHelper.DrawUiElements(splineToolCanvasOriginalImage, splineToolCanvasProcessedImage, _splinetoolVM.SplineToolScaleValue);
        }

        private void SetUiValues(int x, int y)
        {
            _splinetoolVM.XPos = x >= 0 ? "X: " + (x / _splinetoolVM.Graph.Width * 255).ToString() : "";
            _splinetoolVM.YPos = y >= 0 ? "Y: " + ((_splinetoolVM.Graph.Height - y) / _splinetoolVM.Graph.Height * 255).ToString() : "";
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            if (sender == MyGraph)
            {
                var position = e.GetPosition(MyGraph);
                SetUiValues((int)position.X, (int)position.Y);
            }
            else if (sender == ProcessedGraph)
            {
                var position = e.GetPosition(ProcessedGraph);
                SetUiValues((int)(position.X / _splinetoolVM.SplineToolScaleValue), (int)(position.Y / _splinetoolVM.SplineToolScaleValue));
            }
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == MyGraph)
            {
                DataProvider.MousePosition = e.GetPosition(MyGraph);
                if (DataProvider.LastPosition != DataProvider.MousePosition)
                {
                    DataProvider.VectorOfMousePosition.Add(DataProvider.MousePosition);

                    DataProvider.LastPosition = DataProvider.MousePosition;
                    UiHelper.DrawSplineToolGraphUI(splineToolCanvasOriginalImage, _splinetoolVM.SplineToolScaleValue, DataProvider.VectorOfMousePosition);
                }
            }
            else if (sender == ProcessedGraph)
            {
                DataProvider.MousePosition = e.GetPosition(ProcessedGraph);

            }

        }

        private void ImageMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == MyGraph)
            {
                DataProvider.VectorOfMousePosition.Clear();
                DrawingHelper.RemoveUiElements(splineToolCanvasOriginalImage);
            }
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Update();

            if (DataProvider.MagnifierOn == true)
                Application.Current.Windows.OfType<MagnifierWindow>().First().Update();
            if (DataProvider.RowColorLevelsOn == true || DataProvider.ColumnColorLevelsOn == true)
                Application.Current.Windows.OfType<ColorLevelsWindow>().All(window => { window.Update(); return true; });
        }

        private void SplineToolSliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scaleValue = splinetoolsliderZoom.Value;

            DrawingHelper.UpdateShapesProperties(splineToolCanvasOriginalImage, scaleValue);
            DrawingHelper.UpdateShapesProperties(splineToolCanvasProcessedImage, scaleValue);

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
