using Framework.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Framework.Utilities.DataProvider;

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

            //RemoveUiElements(canvasOriginalImage, null);
            //DrawUiElements(canvasOriginalImage, null, _splinetoolVM.ScaleValue);
        }
        
        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Update();

            if (MagnifierOn == true)
                Application.Current.Windows.OfType<MagnifierWindow>().First().Update();
            if (RowColorLevelsOn == true || ColumnColorLevelsOn == true)
                Application.Current.Windows.OfType<ColorLevelsWindow>().All(window => { window.Update(); return true; });
        }


        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(MyGraph);
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
