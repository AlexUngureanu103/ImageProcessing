﻿using Framework.Utilities;
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

        #region Magnifier

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

        private void DrawHermitCurve(object parameter)
        {
            if (DataProvider.VectorOfMousePosition.Count < 3)
            {
                MessageBox.Show("Please select at least 3 first.");
                return;
            }
            DataProvider.SplineToolVectorOfMousePosition.Clear();
            DataProvider.SplineToolVectorOfMousePosition.Add(new Point(0, _splineToolVM.Graph.Height));
            for (int i = Math.Max(DataProvider.VectorOfMousePosition.Count - 5, 0); i < DataProvider.VectorOfMousePosition.Count; i++)
            {
                DataProvider.SplineToolVectorOfMousePosition.Add(DataProvider.VectorOfMousePosition[i]);
            }
            DataProvider.SplineToolVectorOfMousePosition.Add(new Point(_splineToolVM.Graph.Width, 0));
            var points = DataProvider.SplineToolVectorOfMousePosition.OrderBy(point => point.X).Select(point => new Point(point.X, _splineToolVM.Graph.Height - point.Y)).ToList();
            points.Add(new Point(points[points.Count - 1].X - (points[points.Count - 2].X - points[points.Count - 1].X), points[points.Count - 2].Y));
            points.Add(new Point(-points[1].X, points[1].Y));
            points = points.OrderBy(point => point.X).ToList();

            List<Point> curve = GenerateSpline_V2(points);

            curve = curve.Select(point => new Point(point.X, _splineToolVM.Graph.Height - point.Y)).ToList();

            UiHelper.DrawSplineToolCurve(parameter as Canvas, curve, ScaleValue, Brushes.Blue);

            Canvas canvas = parameter as Canvas;
        }

        public List<Point> GenerateSpline_V2( List<Point> points)
        {
            List<Point> result = new List<Point>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point p0 = i == 0 ? points[i] : points[i - 1];
                Point p1 = points[i];
                Point p2 = points[i + 1];
                Point p3 = i == points.Count - 2 ? points[i + 1] : points[i + 2];

                for (double t = 0; t <= 1; t += 0.01f)
                {
                    double tPow2 = Math.Pow(t, 2);
                    double tPow3 = Math.Pow(t, 3);
                    double h00 = 2 * tPow3 - 3 * tPow2 + 1;
                    double h01 = -2 * tPow3 + 3 * tPow2;
                    double h10 = tPow3 - 2 * tPow2 + t;
                    double h11 = tPow3 - tPow2;

                    double x = h00 * p1.X + h01 * p2.X + h10 * (p2.X - p0.X) + h11 * (p3.X - p1.X);
                    double y = h00 * p1.Y + h01 * p2.Y + h10 * (p2.Y - p0.Y) + h11 * (p3.Y - p1.Y);

                    result.Add(new Point(x, y));
                }
            }

            return result;
        }

        #endregion
        #region Magnifier

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

        private void Reset(object parameter)
        {
            DrawingHelper.RemoveUiElements(parameter as Canvas);
            DataProvider.VectorOfMousePosition.Clear();
        }
        #endregion

    }
}