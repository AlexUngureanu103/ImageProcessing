﻿using Algorithms.Sections;
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
            get => _splineToolVM.SplineToolScaleValue;
            set => _splineToolVM.SplineToolScaleValue = value;
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

            List<Point> curvePoints = PointwiseOperations.GetSplineHermitCurvePoints(DataProvider.VectorOfMousePosition.ToList(), _splineToolVM.Graph.Width, _splineToolVM.Graph.Height);

            DataProvider.SplineToolCurvePoints = new System.Windows.Media.PointCollection(curvePoints);

            UiHelper.DrawSplineToolCurve(canvases[0] as Canvas, canvases[1] as Canvas, curvePoints, ScaleValue, Brushes.Blue);
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

        private void UpdateGrayValues(object parameter)
        {
            if (DataProvider.SplineToolCurvePoints.Count == 0)
            {
                MessageBox.Show("Please draw a curve first.");
                return;
            }
            try
            {
                var lutValues = PointwiseOperations.GetLUTValues(DataProvider.SplineToolCurvePoints.ToList(), _splineToolVM.Graph.Width, _splineToolVM.Graph.Height);

                if (DataProvider.ColorInitialImage != null)
                {
                    var image = PointwiseOperations.UpdateGrayValues(DataProvider.ColorInitialImage, lutValues);
                    ClearProcessedImage();

                    DataProvider.ColorProcessedImage = image;
                    _splineToolVM.MainVM.ProcessedImage = ImageConverter.Convert(image);
                }
                else if (DataProvider.GrayInitialImage != null)
                {
                    var image = PointwiseOperations.UpdateGrayValues(DataProvider.GrayInitialImage, lutValues);
                    ClearProcessedImage();

                    DataProvider.GrayProcessedImage = image;
                    _splineToolVM.MainVM.ProcessedImage = ImageConverter.Convert(image);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                //MessageBox.Show(lutValues.Count.ToString());
            }
        }

        private void ClearProcessedImage()
        {
            DataProvider.GrayProcessedImage = null;
            DataProvider.ColorProcessedImage = null;
            _splineToolVM.MainVM.ProcessedImage = null;
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
