using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Converters;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Framework.ViewModel.Commands
{
    public class SplineToolsMenuCommands : BaseVM
    {
        private readonly SplineToolVM _splineToolVM;

        public SplineToolsMenuCommands(SplineToolVM splineToolVM)
        {
            _splineToolVM = splineToolVM;
        }

        private ImageSource Graph
        {
            get => _splineToolVM.Graph;
            set => _splineToolVM.Graph = value;
        }

        private double ScaleValue
        {
            get => _splineToolVM.ScaleValue;
            set => _splineToolVM.ScaleValue = value;
        }

        #region HermitCurve

        private ICommand _drawHermitCurveCommand;
        public ICommand DrawHermitCurveCommand
        {
            get
            {
                if (_drawHermitCurveCommand == null)
                    _drawHermitCurveCommand = new RelayCommand(DrawHermitCurve);
                return _drawHermitCurveCommand;
            }
        }

        private void DrawHermitCurve(object parameters)
        {
            if (DataProvider.VectorOfMousePosition.Count < 3)
            {
                MessageBox.Show("Please select at least 3 first.");
                return;
            }

            var canvases = (object[])parameters;

            var controlPoints = new List<Point>
            {
                new Point(0, _splineToolVM.Graph.Height)
            };
            for (int i = Math.Max(DataProvider.VectorOfMousePosition.Count - 5, 0); i < DataProvider.VectorOfMousePosition.Count; i++)
            {
                controlPoints.Add(DataProvider.VectorOfMousePosition[i]);
            }
            controlPoints.Add(new Point(_splineToolVM.Graph.Width, 0));

            var points = controlPoints.OrderBy(point => point.X).Select(point => new Point(point.X, _splineToolVM.Graph.Height - point.Y)).ToList();
            points.Add(new Point(points[points.Count - 1].X - (points[points.Count - 2].X - points[points.Count - 1].X), points[points.Count - 2].Y));
            points.Add(new Point(-points[1].X, points[1].Y));
            points = points.OrderBy(point => point.X).ToList();

            List<Point> firstGen = GenerateCubicHermiteSplinePoints(points, 0.01d);

            List<Point> secondGen = GenerateCubicHermiteSplinePoints(firstGen, 0.3d)
                .Select(point => new Point(point.X, _splineToolVM.Graph.Height - point.Y))
                .Where(point => point.X >= 0 && point.X <= _splineToolVM.Graph.Width)
                .Select(point => new Point(point.X, point.Y < 0 ? 0 : point.Y))
                .ToList();

            DataProvider.SplineToolCurvePoints = new System.Windows.Media.PointCollection(secondGen);

            UiHelper.DrawSplineToolCurve(canvases[0] as Canvas, canvases[1] as Canvas, secondGen, ScaleValue, Brushes.Blue);
        }

        public List<Point> GenerateCubicHermiteSplinePoints(List<Point> points, double tStep)
        {
            List<Point> result = new List<Point>();
            double s = 2 * 0.85d;
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point p0 = i == 0 ? points[i] : points[i - 1];
                Point p1 = points[i];
                Point p2 = points[i + 1];
                Point p3 = i == points.Count - 2 ? points[i + 1] : points[i + 2];

                Point dv1 = new Point((p2.X - p0.X) / s, (p2.Y - p0.Y) / s);
                Point dv2 = new Point((p3.X - p1.X) / s, (p3.Y - p1.Y) / s);

                for (double t = 0; t <= 1; t += tStep)
                {
                    double tPow2 = Math.Pow(t, 2);
                    double tPow3 = Math.Pow(t, 3);
                    double h00 = 2 * tPow3 - 3 * tPow2 + 1;
                    double h01 = -2 * tPow3 + 3 * tPow2;
                    double h10 = tPow3 - 2 * tPow2 + t;
                    double h11 = tPow3 - tPow2;

                    double x = h00 * p1.X + h01 * p2.X + h10 * dv1.X + h11 * dv2.X;
                    double y = h00 * p1.Y + h01 * p2.Y + h10 * dv1.Y + h11 * dv2.Y;

                    result.Add(new Point(x, y));
                }
            }

            return result;
        }

        #endregion

        #region Update Gray Values

        private ICommand _updateGrayValuesCommand;
        public ICommand UpdateGrayValuesCommand
        {
            get
            {
                if (_updateGrayValuesCommand == null)
                    _updateGrayValuesCommand = new RelayCommand(UpdateGrayValues);
                return _updateGrayValuesCommand;
            }
        }

        private void UpdateGrayValues(object parameters)
        {
            var lutValues = new Dictionary<double, double>();
            try
            {
                lutValues = DataProvider.SplineToolCurvePoints
                   .GroupBy(point => NormalizeValue(point.X, _splineToolVM.Graph.Width))
                   .ToDictionary(group => group.Key,
                   group => NormalizeValue(_splineToolVM.Graph.Height - group.Sum(point => point.Y) / group.Count(), _splineToolVM.Graph.Height));

                if (DataProvider.ColorInitialImage != null)
                {
                    var image = new Image<Bgr, byte>(DataProvider.ColorInitialImage.Size);


                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            image.Data[y, x, 0] = (byte)lutValues[DataProvider.ColorInitialImage.Data[y, x, 0]];
                            image.Data[y, x, 1] = (byte)lutValues[DataProvider.ColorInitialImage.Data[y, x, 1]];
                            image.Data[y, x, 2] = (byte)lutValues[DataProvider.ColorInitialImage.Data[y, x, 2]];
                        }
                    }
                    DataProvider.ColorProcessedImage = image;
                    _splineToolVM.MainVM.ProcessedImage = ImageConverter.Convert(image);
                }
                else if (DataProvider.GrayInitialImage != null)
                {
                    var image = new Image<Gray, byte>(DataProvider.GrayInitialImage.Size);

                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            byte grayValue = DataProvider.GrayInitialImage.Data[y, x, 0];
                            byte lutRawValue = (byte)lutValues[grayValue];

                            image.Data[y, x, 0] = lutRawValue;
                        }
                    }
                    DataProvider.GrayProcessedImage = image;
                    _splineToolVM.MainVM.ProcessedImage = ImageConverter.Convert(image);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + $"{lutValues.Count}");
            }
            finally
            {
                //MessageBox.Show(lutValues.Count.ToString());
            }
        }

        private double NormalizeValue(double value, double size)
        {
            var normalizedValue = value / size * 255;
            return Math.Max(0, Math.Min(255, Math.Round(normalizedValue)));
        }

        #endregion

        #region Reset

        private ICommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                    _resetCommand = new RelayCommand(Reset);
                return _resetCommand;
            }
        }

        private void Reset(object parameters)
        {
            var canvases = (object[])parameters;

            Image<Bgr, byte> image = new Image<Bgr, byte>(800, 600, new Bgr(255, 255, 255));

            Graph = ImageConverter.Convert(image);

            DrawingHelper.RemoveUiElements(canvases[0] as Canvas);
            DrawingHelper.RemoveUiElements(canvases[1] as Canvas);
            DataProvider.VectorOfMousePosition.Clear();
            DataProvider.SplineToolCurvePoints.Clone();
        }
        #endregion
    }
}
