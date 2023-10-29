using Framework.Utilities;
using Framework.ViewModel;
using System.Linq;
using System.Windows;

namespace Framework.View
{
    public partial class ColorLevelsWindow : Window
    {
        private readonly ColorLevelsVM _colorLevelsVM;
        private readonly CLevelsType _cLevelsType;

        public ColorLevelsWindow(MainVM mainVM, CLevelsType type)
        {
            InitializeComponent();

            _colorLevelsVM = ColorLevelsFactory.Produce(_cLevelsType = type);
            _colorLevelsVM.Theme = mainVM.Theme;
            DataContext = _colorLevelsVM;

            Application.Current.Windows.OfType<MainWindow>().First().Update();
            Update();

            if (DataProvider.ColorInitialImage != null || DataProvider.ColorProcessedImage != null)
            {
                checkBoxBlue.Visibility = Visibility.Visible;
                checkBoxGreen.Visibility = Visibility.Visible;
                checkBoxRed.Visibility = Visibility.Visible;
            }
        }

        private Point LastPoint { get; set; }

        public void Update()
        {
            if (LastPoint != DataProvider.LastPosition)
            {
                _colorLevelsVM.XPos = "X: " + ((int)DataProvider.LastPosition.X).ToString();
                _colorLevelsVM.YPos = "Y: " + ((int)DataProvider.LastPosition.Y).ToString();

                DisplayGray();
                DisplayColor();

                LastPoint = DataProvider.LastPosition;
            }

            bool showBlue = (bool)checkBoxBlue.IsChecked;
            bool showGreen = (bool)checkBoxGreen.IsChecked;
            bool showRed = (bool)checkBoxRed.IsChecked;

            SetVisibility(0, showBlue);
            SetVisibility(1, showGreen);
            SetVisibility(2, showRed);
        }

        private void DisplayGray()
        {
            if (DataProvider.GrayInitialImage != null)
                originalImageView.Model = _colorLevelsVM.PlotImage(DataProvider.GrayInitialImage);
            if (DataProvider.GrayProcessedImage != null)
                processedImageView.Model = _colorLevelsVM.PlotImage(DataProvider.GrayProcessedImage);
        }

        private void DisplayColor()
        {
            if (DataProvider.ColorInitialImage != null)
                originalImageView.Model = _colorLevelsVM.PlotImage(DataProvider.ColorInitialImage);
            if (DataProvider.ColorProcessedImage != null)
                processedImageView.Model = _colorLevelsVM.PlotImage(DataProvider.ColorProcessedImage);
        }

        private void SetVisibility(int indexSeries, bool isVisible)
        {
            if (originalImageView.Model != null)
            {
                if (DataProvider.ColorInitialImage != null || (DataProvider.GrayInitialImage != null && indexSeries == 0))
                    originalImageView.Model.Series[indexSeries].IsVisible = isVisible;

                originalImageView.Model.InvalidatePlot(true);
            }

            if (processedImageView.Model != null)
            {
                if (DataProvider.ColorProcessedImage != null || (DataProvider.GrayProcessedImage != null && indexSeries == 0))
                    processedImageView.Model.Series[indexSeries].IsVisible = isVisible;

                processedImageView.Model.InvalidatePlot(true);
            }
        }

        private void UpdateSeriesVisibility(object sender, RoutedEventArgs e)
        {
            bool showSeries;
            if (sender == checkBoxBlue)
            {
                showSeries = (bool)checkBoxBlue.IsChecked;
                SetVisibility(0, showSeries);
            }
            else if (sender == checkBoxGreen)
            {
                showSeries = (bool)checkBoxGreen.IsChecked;
                SetVisibility(1, showSeries);
            }
            else if (sender == checkBoxRed)
            {
                showSeries = (bool)checkBoxRed.IsChecked;
                SetVisibility(2, showSeries);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (_cLevelsType)
            {
                case CLevelsType.Row:
                    DataProvider.RowColorLevelsOn = false;
                    break;

                case CLevelsType.Column:
                    DataProvider.ColumnColorLevelsOn = false;
                    break;
            }

            Application.Current.Windows.OfType<MainWindow>().First().Update();
        }
    }
}