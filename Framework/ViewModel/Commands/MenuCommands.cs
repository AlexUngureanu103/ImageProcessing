using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Converters;
using Framework.Utilities;
using Framework.View;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;

        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayImageCommand;
        public RelayCommand LoadGrayImageCommand
        {
            get
            {
                if (_loadGrayImageCommand == null)
                    _loadGrayImageCommand = new RelayCommand(LoadGrayImage);
                return _loadGrayImageCommand;
            }
        }

        private void LoadGrayImage(object parameter)
        {
            Clear(parameter);

            string fileName = FileHelper.LoadFileDialog("Select a gray picture");
            if (fileName != null)
            {
                DataProvider.GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = ImageConverter.Convert(DataProvider.GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = FileHelper.LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                DataProvider.ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = ImageConverter.Convert(DataProvider.ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (DataProvider.GrayProcessedImage == null && DataProvider.ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = FileHelper.SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                DataProvider.GrayProcessedImage?.Bitmap.Save(imagePath, FileHelper.GetJpegCodec("image/jpeg"), FileHelper.GetEncoderParameter(Encoder.Quality, 100));
                DataProvider.ColorProcessedImage?.Bitmap.Save(imagePath, FileHelper.GetJpegCodec("image/jpeg"), FileHelper.GetEncoderParameter(Encoder.Quality, 100));
                FileHelper.OpenImage(imagePath);
            }
        }
        #endregion

        #region Save both images
        private ICommand _saveImagesCommand;
        public ICommand SaveImagesCommand
        {
            get
            {
                if (_saveImagesCommand == null)
                    _saveImagesCommand = new RelayCommand(SaveImages);
                return _saveImagesCommand;
            }
        }

        private void SaveImages(object parameter)
        {
            if (DataProvider.GrayInitialImage == null && DataProvider.ColorInitialImage == null)
            {
                MessageBox.Show("If you want to save both images, " +
                    "please load and process an image first!");
                return;
            }

            if (DataProvider.GrayProcessedImage == null && DataProvider.ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save both images, " +
                    "please process your image first!");
                return;
            }

            string imagePath = FileHelper.SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                IImage processedImage = null;
                if (DataProvider.GrayInitialImage != null && DataProvider.GrayProcessedImage != null)
                    processedImage = Utils.Combine(DataProvider.GrayInitialImage, DataProvider.GrayProcessedImage);

                if (DataProvider.GrayInitialImage != null && DataProvider.ColorProcessedImage != null)
                    processedImage = Utils.Combine(DataProvider.GrayInitialImage, DataProvider.ColorProcessedImage);

                if (DataProvider.ColorInitialImage != null && DataProvider.GrayProcessedImage != null)
                    processedImage = Utils.Combine(DataProvider.ColorInitialImage, DataProvider.GrayProcessedImage);

                if (DataProvider.ColorInitialImage != null && DataProvider.ColorProcessedImage != null)
                    processedImage = Utils.Combine(DataProvider.ColorInitialImage, DataProvider.ColorProcessedImage);

                processedImage?.Bitmap.Save(imagePath, FileHelper.GetJpegCodec("image/jpeg"), FileHelper.GetEncoderParameter(Encoder.Quality, 100));
                FileHelper.OpenImage(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            DataProvider.CloseWindows();
            System.Environment.Exit(0);
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            DrawingHelper.RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            DrawingHelper.RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            DrawingHelper.RemoveUiElements(canvases[0] as Canvas);
            DrawingHelper.RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            DrawingHelper.RemoveUiElements(parameter as Canvas);

            DataProvider.GrayInitialImage = null;
            DataProvider.ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            DrawingHelper.RemoveUiElements(parameter as Canvas);

            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            DataProvider.CloseWindows();

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (DataProvider.MagnifierOn == true) return;
            if (DataProvider.VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }
        #endregion

        #region Display Gray/Color levels

        #region On row
        private ICommand _displayLevelsOnRowCommand;
        public ICommand DisplayLevelsOnRowCommand
        {
            get
            {
                if (_displayLevelsOnRowCommand == null)
                    _displayLevelsOnRowCommand = new RelayCommand(DisplayLevelsOnRow);
                return _displayLevelsOnRowCommand;
            }
        }

        private void DisplayLevelsOnRow(object parameter)
        {
            if (DataProvider.RowColorLevelsOn == true) return;
            if (DataProvider.VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region On column
        private ICommand _displayLevelsOnColumnCommand;
        public ICommand DisplayLevelsOnColumnCommand
        {
            get
            {
                if (_displayLevelsOnColumnCommand == null)
                    _displayLevelsOnColumnCommand = new RelayCommand(DisplayLevelsOnColumn);
                return _displayLevelsOnColumnCommand;
            }
        }

        private void DisplayLevelsOnColumn(object parameter)
        {
            if (DataProvider.ColumnColorLevelsOn == true) return;
            if (DataProvider.VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (DataProvider.InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            HistogramWindow window = null;

            if (DataProvider.ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (DataProvider.GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (DataProvider.ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image !");
                return;
            }

            HistogramWindow window = null;

            if (DataProvider.ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (DataProvider.GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion


        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.Copy(DataProvider.ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
            else if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.GrayProcessedImage = Tools.Copy(DataProvider.GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.GrayProcessedImage = Tools.Invert(DataProvider.GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.Invert(DataProvider.ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }
        #endregion

        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.GrayProcessedImage = Tools.Convert(DataProvider.ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images !");
            }
        }
        #endregion

        #endregion

        #region Pointwise operations
        private ICommand _splineToolCommand;
        public ICommand SplineToolCommand
        {
            get
            {
                if (_splineToolCommand == null)
                {
                    _splineToolCommand = new RelayCommand(SplineTool);
                }
                return _splineToolCommand;
            }
        }

        private void SplineTool(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            SplineToolWindow splineToolWindow = new SplineToolWindow(_mainVM);
            splineToolWindow.Show();
        }
        #endregion

        #region Thresholding

        #region Basic Thresholding
        private ICommand _thresholdingCommand;
        public ICommand ThresholdingCommand
        {
            get
            {
                if (_thresholdingCommand == null)
                {
                    _thresholdingCommand = new RelayCommand(ThresholdingImage);
                }
                return _thresholdingCommand;
            }
        }

        private void ThresholdingImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> parameters = new List<string>();
            parameters.Add("Threshold");

            DialogBox box = new DialogBox(_mainVM, parameters);
            box.ShowDialog();

            List<double> values = box.GetValues();
            if (values[0] < 10 || values[0] > 154)
            {
                MessageBox.Show("Please add an thresholding value between 10 and 154 !");
                return;
            }
            if (values != null)
            {
                byte threshhold = (byte)(values[0] + 0.5);
                if (DataProvider.GrayInitialImage != null)
                {
                    DataProvider.GrayProcessedImage = Tools.Thresholding(DataProvider.GrayInitialImage, threshhold);
                    ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
                }
                else if (DataProvider.ColorInitialImage != null)
                {
                    DataProvider.GrayProcessedImage = Tools.Convert(DataProvider.ColorInitialImage);
                    DataProvider.GrayProcessedImage = Tools.Thresholding(DataProvider.GrayProcessedImage, threshhold);
                    ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
                }
            }
        }

        #endregion

        #region Triangle Thresholding

        private ICommand _triangleThresholdingCommand;
        public ICommand TriangleThresholdingCommand
        {
            get
            {
                if (_triangleThresholdingCommand == null)
                {
                    _triangleThresholdingCommand = new RelayCommand(TriangleThresholdingImage);
                }
                return _triangleThresholdingCommand;
            }
        }

        private void TriangleThresholdingImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }


            ClearProcessedCanvas(parameter);

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.GrayProcessedImage = Thresholding.TriangleThresholding(DataProvider.GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.GrayProcessedImage = Tools.Convert(DataProvider.ColorInitialImage);
                DataProvider.GrayProcessedImage = Thresholding.TriangleThresholding(DataProvider.GrayProcessedImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
        }

        #endregion

        #endregion

        #region Crop Image

        private ICommand _cropCommand;
        public ICommand CropCommand
        {
            get
            {
                if (_cropCommand == null)
                {
                    _cropCommand = new RelayCommand(CropImage);
                }
                return _cropCommand;
            }
        }

        private void CropImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            var canvases = (object[])parameter;
            ClearProcessedCanvas(canvases[1]);
            RemoveInitialDrawnShapes(canvases[0]);

            if (DataProvider.VectorOfMousePosition.Count >= 2)
            {
                Point firstPoint = DataProvider.VectorOfMousePosition.Last(c => c != DataProvider.LastPosition);
                Point secondPoint = DataProvider.LastPosition;

                if (firstPoint.X.Equals(secondPoint.X) || firstPoint.Y.Equals(secondPoint.Y))
                {
                    MessageBox.Show("Points must be different", "Warning");
                    return;
                }
                if (DataProvider.GrayInitialImage != null)
                {
                    DataProvider.ColorProcessedImage = Tools.Convert(DataProvider.GrayInitialImage);
                    DataProvider.ColorProcessedImage = Tools.Crop(DataProvider.ColorProcessedImage, firstPoint, secondPoint);
                    ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);

                    DrawSelectedZone(canvases[0]);
                }
                else if (DataProvider.ColorInitialImage != null)
                {
                    DataProvider.ColorProcessedImage = Tools.Crop(DataProvider.ColorInitialImage, firstPoint, secondPoint);
                    ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);

                    DrawSelectedZone(canvases[0]);
                }

                Bgr avegare;
                MCvScalar standardDeviation;

                DataProvider.ColorProcessedImage.AvgSdv(out avegare, out standardDeviation);

                MessageBox.Show($"Valoarea medie of selected Zone: B:{avegare.Blue}, G:{avegare.Green}, R:{avegare.Red}\r\nAbaterea medie patratica: B:{standardDeviation.V0 * standardDeviation.V0}, G:{standardDeviation.V1 * standardDeviation.V1}, R:{standardDeviation.V2 * standardDeviation.V2}");
            }
            else
            {
                MessageBox.Show("Please select at least 2 points !");
                return;
            }

        }

        private void DrawSelectedZone(object canvas)
        {
            var topLeftPoint = Tools.GetTopLeftPoint(DataProvider.VectorOfMousePosition.Last(c => c != DataProvider.LastPosition), DataProvider.LastPosition);
            var bottomRightPoint = Tools.GetBottomRightPoint(DataProvider.VectorOfMousePosition.Last(c => c != DataProvider.LastPosition), DataProvider.LastPosition);
            DrawingHelper.DrawRectangle(canvas as Canvas, topLeftPoint, bottomRightPoint, 1, Brushes.Red, ScaleValue);
        }

        #endregion

        #region Mirror Image

        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                {
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                }
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.Convert(DataProvider.GrayInitialImage);
                DataProvider.ColorProcessedImage = Tools.Mirror(DataProvider.ColorProcessedImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);

            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.Mirror(DataProvider.ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #region Rotate Image

        #region Clockwise
        private ICommand _rotateClockwiseCommand;
        public ICommand RotateClockwiseCommand
        {
            get
            {
                if (_rotateClockwiseCommand == null)
                {
                    _rotateClockwiseCommand = new RelayCommand(RotateImageClockwise);
                }
                return _rotateClockwiseCommand;
            }
        }

        private void RotateImageClockwise(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.Convert(DataProvider.GrayInitialImage);
                DataProvider.ColorProcessedImage = Tools.RotateImage(DataProvider.ColorProcessedImage, 90);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);

            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.RotateImage(DataProvider.ColorInitialImage, 90);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }
        #endregion

        #region Anti-Clockwise
        private ICommand _rotateAntiClockwiseCommand;
        public ICommand RotateAntiClockwiseCommand
        {
            get
            {
                if (_rotateAntiClockwiseCommand == null)
                {
                    _rotateAntiClockwiseCommand = new RelayCommand(RotateImageAntiClockwise);
                }
                return _rotateAntiClockwiseCommand;
            }
        }

        private void RotateImageAntiClockwise(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.Convert(DataProvider.GrayInitialImage);
                DataProvider.ColorProcessedImage = Tools.RotateImage(DataProvider.ColorProcessedImage, -90);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);

            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = Tools.RotateImage(DataProvider.ColorInitialImage, -90);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }
        #endregion
        #endregion

        #region Filters

        #region Median vectorial filter

        private ICommand _filtrulMedianVectorialCommand;
        public ICommand FiltrulMedianVectorialCommand
        {
            get
            {
                if (_filtrulMedianVectorialCommand == null)
                {
                    _filtrulMedianVectorialCommand = new RelayCommand(FiltrulMedianVectorial);
                }
                return _filtrulMedianVectorialCommand;
            }
        }

        private void FiltrulMedianVectorial(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image !");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Choose filter size:\n Yes - 3x3\n No - 5x5", "Filter size", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly);
            int filterSize = 0;

            if (result == MessageBoxResult.Yes)
            {
                filterSize = 1;
            }
            else if (result == MessageBoxResult.No)
            {
                filterSize = 2;
            }
            else
            {
                return;
            }

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.GrayProcessedImage = new Image<Gray, byte>(DataProvider.GrayInitialImage.Bitmap);
                DataProvider.GrayProcessedImage = Filters.FiltrulMedianVectorial(DataProvider.GrayInitialImage, 2 * filterSize + 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = new Image<Bgr, byte>(DataProvider.ColorInitialImage.Bitmap);
                DataProvider.ColorProcessedImage = Filters.FiltrulMedianVectorial(DataProvider.ColorInitialImage, 2 * filterSize + 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #region Gauss filter

        private ICommand _gaussFilterCommand;
        public ICommand GaussFilterCommand
        {
            get
            {
                if (_gaussFilterCommand == null)
                {
                    _gaussFilterCommand = new RelayCommand(GaussFilter);
                }
                return _gaussFilterCommand;
            }
        }

        private void GaussFilter(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image ! Gauss Filter");
                return;
            }

            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;

            if (DataProvider.GrayInitialImage != null)
            {
                DataProvider.GrayProcessedImage = DataProvider.GrayInitialImage.SmoothGaussian(5, 5, 1, 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                DataProvider.ColorProcessedImage = DataProvider.ColorInitialImage.SmoothGaussian(5, 5, 1, 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #region Gradient magnitude image

        private ICommand _gradientMagnitudeImageCommand;
        public ICommand GradientMagnitudeImageCommand
        {
            get
            {
                if (_gradientMagnitudeImageCommand == null)
                {
                    _gradientMagnitudeImageCommand = new RelayCommand(GradientMagnitudeImage);
                }
                return _gradientMagnitudeImageCommand;
            }
        }

        private void GradientMagnitudeImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image GradientMagnitudeImage!");
                return;
            }
            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;

            var sliderDialogBox = new SliderDialogBox(_mainVM, new List<string> { "T1" });
            sliderDialogBox.ShowDialog();
            var values = sliderDialogBox.GetValues();

            if (DataProvider.GrayInitialImage != null)
            {
                var img = DataProvider.GrayInitialImage.SmoothGaussian(5, 5, 1, 1);
                var imgs = Filters.Sobel(img, (byte)values[0]);

                DataProvider.GrayProcessedImage = imgs.Item1;

                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                MessageBox.Show("Please convert image to grayscale first!");
                return;
                DataProvider.ColorProcessedImage = DataProvider.ColorInitialImage.SmoothGaussian(5, 5, 1, 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #region Angle Image

        private ICommand _angleImageCommand;
        public ICommand AngleImageCommand
        {
            get
            {
                if (_angleImageCommand == null)
                {
                    _angleImageCommand = new RelayCommand(AngleImage);
                }
                return _angleImageCommand;
            }
        }

        private void AngleImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image Angle Image!");
                return;
            }

            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;

            var sliderDialogBox = new SliderDialogBox(_mainVM, new List<string> { "T1" });
            sliderDialogBox.ShowDialog();
            var values = sliderDialogBox.GetValues();

            if (DataProvider.GrayInitialImage != null)
            {
                var img = DataProvider.GrayInitialImage.SmoothGaussian(5, 5, 1, 1);
                var imgs = Filters.Sobel(img, (byte)values[0]);

                DataProvider.ColorProcessedImage = imgs.Item2;

                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                MessageBox.Show("Please convert image to grayscale first!");
                return;
                DataProvider.ColorProcessedImage = DataProvider.ColorInitialImage.SmoothGaussian(5, 5, 1, 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #region NonmaximumSuppression

        private ICommand _nonmaximumSuppressionCommand;
        public ICommand NonmaximumSuppressionCommand
        {
            get
            {
                if (_nonmaximumSuppressionCommand == null)
                {
                    _nonmaximumSuppressionCommand = new RelayCommand(NonmaximumSuppression);
                }
                return _nonmaximumSuppressionCommand;
            }
        }

        private void NonmaximumSuppression(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image NonmaximumSuppression!");
                return;
            }

            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;

            var sliderDialogBox = new SliderDialogBox(_mainVM, new List<string> { "T1" });
            sliderDialogBox.ShowDialog();
            var values = sliderDialogBox.GetValues();

            if (DataProvider.GrayInitialImage != null)
            {
                var img = DataProvider.GrayInitialImage.SmoothGaussian(5, 5, 1, 1);
                var imgs = Filters.Sobel(img, (byte)values[0]);

                var firstImg = imgs.Item1;
                var secondImg = Filters.NonMaximaSupression(firstImg, imgs.Item2);
                //var cnt = 0;
                //while (!firstImg.Equals(secondImg))
                //{
                //    cnt++;
                //    imgs = Filters.Sobel(secondImg);
                //    firstImg = imgs.Item1;
                //    secondImg = Filters.NonMaximaSupression(firstImg, imgs.Item2);
                //}
                DataProvider.GrayProcessedImage = secondImg;

                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                MessageBox.Show("Please convert image to grayscale first!");
                return;
                DataProvider.ColorProcessedImage = DataProvider.ColorInitialImage.SmoothGaussian(5, 5, 1, 1);
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #region HysteresisThresholding

        private ICommand _hysteresisThresholdingCommand;
        public ICommand HysteresisThresholdingCommand
        {
            get
            {
                if (_hysteresisThresholdingCommand == null)
                {
                    _hysteresisThresholdingCommand = new RelayCommand(HysteresisThresholding);
                }
                return _hysteresisThresholdingCommand;
            }
        }

        private void HysteresisThresholding(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image HysteresisThresholding!");
                return;
            }

            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;

            var sliderDialogBox = new SliderDialogBox(_mainVM, new List<string> { "T1", "T2" });
            sliderDialogBox.ShowDialog();
            var values = sliderDialogBox.GetValues();
            if (values[0] > values[1])
            {
                MessageBox.Show("T1 must be smaller than T2");
                return;
            }

            if (DataProvider.GrayInitialImage != null)
            {
                var img = DataProvider.GrayInitialImage.SmoothGaussian(5, 5, 1, 1);
                var imgs = Filters.Sobel(img, (byte)values[0]);

                var firstImg = imgs.Item1;
                var secondImg = Filters.NonMaximaSupression(firstImg, imgs.Item2);
                //var cnt = 0;
                //while (!firstImg.Equals(secondImg))
                //{
                //    cnt++;
                //    imgs = Filters.Sobel(secondImg);
                //    firstImg = imgs.Item1;
                //    secondImg = Filters.NonMaximaSupression(firstImg, imgs.Item2);
                //}

                DataProvider.GrayProcessedImage = Thresholding.HysteresisThresholding(secondImg, (byte)values[0], (byte)values[1]);

                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                return;
                var img = DataProvider.GrayInitialImage.SmoothGaussian(5, 5, 1, 1);
                DataProvider.GrayProcessedImage = Tools.Convert(DataProvider.ColorInitialImage);
                DataProvider.GrayProcessedImage = Thresholding.HysteresisThresholding(DataProvider.GrayProcessedImage, (byte)values[0], (byte)values[1]);
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
        }
        #endregion

        #region CannyRGB

        private ICommand _cannyRGBCommand;
        public ICommand CannyRGBCommand
        {
            get
            {
                if (_cannyRGBCommand == null)
                {
                    _cannyRGBCommand = new RelayCommand(CannyRGB);
                }
                return _cannyRGBCommand;
            }
        }

        private void CannyRGB(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image  CannyRGB!");
                return;
            }

            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;

            var sliderDialogBox = new SliderDialogBox(_mainVM, new List<string> { "T1", "T2" });
            sliderDialogBox.ShowDialog();
            var values = sliderDialogBox.GetValues();
            if (values[0] > values[1])
            {
                MessageBox.Show("T1 must be smaller than T2");
                return;
            }

            if (DataProvider.GrayInitialImage != null)
            {
                var image = new Image<Gray, byte>(DataProvider.GrayInitialImage.Bitmap);
                CvInvoke.Canny(DataProvider.GrayInitialImage, image, values[0], values[1]);
                DataProvider.GrayProcessedImage = image;
                ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
            }
            else if (DataProvider.ColorInitialImage != null)
            {
                return;
                var image = new Image<Bgr, byte>(DataProvider.ColorInitialImage.Bitmap);
                CvInvoke.Canny(DataProvider.ColorInitialImage, image, values[0], values[1]);
                DataProvider.ColorProcessedImage = image;
                ProcessedImage = ImageConverter.Convert(DataProvider.ColorProcessedImage);
            }
        }

        #endregion

        #endregion

        #region Morphological operations


        private ICommand _closingGrayCommand;
        public ICommand ClosingGrayCommand
        {
            get
            {
                if (_closingGrayCommand == null)
                {
                    _closingGrayCommand = new RelayCommand(ClosingGray);
                }
                return _closingGrayCommand;
            }
        }

        private void ClosingGray(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image ! CLosing Gray.");
                return;
            }

            if (DataProvider.GrayInitialImage == null)
            {
                MessageBox.Show("Please add a gray image ! Closing Gray.");
                return;
            }

            var dialogBox = new DialogBox(_mainVM, new List<string> { "Kernel size" });
            dialogBox.ShowDialog();

            var value = dialogBox.GetValues()[0];
            if (value < 3)
            {
                value = 3;
            }
            if (value % 2 == 0)
            {
                value += 1;
            }

            DataProvider.GrayProcessedImage = MorphologicalOperations.ClosingGray(DataProvider.GrayInitialImage, (int)value);
            ProcessedImage = ImageConverter.Convert(DataProvider.GrayProcessedImage);
        }

        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation
        #endregion

        #region Save processed image as original image
        private ICommand _saveAsOriginalImageCommand;
        public ICommand SaveAsOriginalImageCommand
        {
            get
            {
                if (_saveAsOriginalImageCommand == null)
                    _saveAsOriginalImageCommand = new RelayCommand(SaveAsOriginalImage);
                return _saveAsOriginalImageCommand;
            }
        }

        private void SaveAsOriginalImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first.");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (DataProvider.GrayProcessedImage != null)
            {
                DataProvider.GrayInitialImage = DataProvider.GrayProcessedImage;
                InitialImage = ImageConverter.Convert(DataProvider.GrayInitialImage);
            }
            else if (DataProvider.ColorProcessedImage != null)
            {
                DataProvider.ColorInitialImage = DataProvider.ColorProcessedImage;
                InitialImage = ImageConverter.Convert(DataProvider.ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion
    }
}