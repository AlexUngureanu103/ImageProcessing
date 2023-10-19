using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Framework.Utilities
{
    public class UiHelper
    {
        private static Rectangle _initialSquare;
        private static Rectangle _processedSquare;

        private static Line _initialRowLine;
        private static Line _processedRowLine;

        private static Line _initialColumnLine;
        private static Line _processedColumnLine;

        private static List<Line> _splineToolRowLines = new List<Line>();
        private static List<Line> _splineToolColumnLines = new List<Line>();
        private static List<Ellipse> _splineToolPointsElipses = new List<Ellipse>();

        public static void DrawUiElements(Canvas initialCanvas, Canvas processedCanvas, double scaleValue)
        {
            if (DataProvider.MagnifierOn == true)
            {
                DrawInitialSquare(initialCanvas, scaleValue);
                DrawProcessedSquare(processedCanvas, scaleValue);
            }

            if (DataProvider.RowColorLevelsOn == true || DataProvider.MagnifierOn == true)
            {
                DrawInitialRowLine(initialCanvas, scaleValue);
                DrawProcessedRowLine(processedCanvas, scaleValue);
            }

            if (DataProvider.ColumnColorLevelsOn == true || DataProvider.MagnifierOn == true)
            {
                DrawInitialColumnLine(initialCanvas, scaleValue);
                DrawProcessedColumnLine(processedCanvas, scaleValue);
            }
        }

        public static void DrawSplineToolGraphUI(Canvas canvas, double scaleValue, System.Windows.Media.PointCollection vectorOfMousePosition)
        {
            RemoveSplineToolUI(canvas);

            DrawSplineToolUIInteractivePoints(canvas, scaleValue, vectorOfMousePosition);
        }

        private static void DrawSplineToolUIInteractivePoints(Canvas canvas, double scaleValue, System.Windows.Media.PointCollection vectorOfMousePosition)
        {
            int imgWidth = 500;
            int imgHeigth = 300;

            _splineToolColumnLines.Add(DrawingHelper.DrawLine(canvas, new Point(0, imgHeigth), new Point(0, 0), 3, Brushes.Black, scaleValue));
            _splineToolColumnLines.Add(DrawingHelper.DrawLine(canvas, new Point(0, imgHeigth), new Point(imgWidth, imgHeigth), 3, Brushes.Black, scaleValue));
            
            _splineToolPointsElipses.Add(DrawingHelper.DrawEllipse(canvas, new Point(0, imgHeigth), 10, 10, 5, Brushes.Black, scaleValue));
            _splineToolPointsElipses.Add(DrawingHelper.DrawEllipse(canvas, new Point(imgWidth, 0), 10, 10, 5, Brushes.Black, scaleValue));
            
            foreach (var point in vectorOfMousePosition.Skip(Math.Max(0, vectorOfMousePosition.Count() - 5)))
            {
                var startR = new Point(0, point.Y);
                var endR = new Point(imgWidth, point.Y);
                _splineToolRowLines.Add(DrawingHelper.DrawDottedLine(canvas, startR, point, 1, Brushes.Black, scaleValue, 7));

                var startC = new Point(point.X, imgHeigth);
                var endC = new Point(point.X, 0);
                _splineToolColumnLines.Add(DrawingHelper.DrawDottedLine(canvas, startC, point, 1, Brushes.Black, scaleValue, 7));

                _splineToolPointsElipses.Add(DrawingHelper.DrawEllipse(canvas, point, 10, 10, 5, Brushes.Black, scaleValue));
            }
        }

        private static void RemoveSplineToolUI(Canvas canvas)
        {
            foreach (var el in _splineToolColumnLines)
            {
                DrawingHelper.RemoveUiElement(canvas, el);
            }
            _splineToolColumnLines.Clear();
            foreach (var el in _splineToolPointsElipses)
            {
                DrawingHelper.RemoveUiElement(canvas, el);
            }
            _splineToolPointsElipses.Clear();
            foreach (var el in _splineToolRowLines)
            {
                DrawingHelper.RemoveUiElement(canvas, el);
            }
            _splineToolRowLines.Clear();
        }

        public static void RemoveUiElements(Canvas initialCanvas, Canvas processedCanvas)
        {
            DrawingHelper.RemoveUiElement(initialCanvas, _initialSquare);
            DrawingHelper.RemoveUiElement(processedCanvas, _processedSquare);

            DrawingHelper.RemoveUiElement(initialCanvas, _initialRowLine);
            DrawingHelper.RemoveUiElement(processedCanvas, _processedRowLine);

            DrawingHelper.RemoveUiElement(initialCanvas, _initialColumnLine);
            DrawingHelper.RemoveUiElement(processedCanvas, _processedColumnLine);
        }

        private static Rectangle GetSquare(Canvas canvas, Point point, double scaleValue)
        {
            var leftTop = new Point(point.X - 5, point.Y - 5);
            var rightBottom = new Point(point.X + 5, point.Y + 5);

            return DrawingHelper.DrawRectangle(canvas, leftTop, rightBottom, 1, Brushes.Red, scaleValue);
        }

        private static Line GetRowLine(Canvas canvas, IImage image, Point point, double scaleValue)
        {
            var start = new Point(0, point.Y);
            var end = new Point(image.Size.Width, point.Y);

            return DrawingHelper.DrawLine(canvas, start, end, 1, Brushes.Red, scaleValue);
        }

        private static Line GetColumnLine(Canvas canvas, IImage image, Point point, double scaleValue)
        {
            var start = new Point(point.X, 0);
            var end = new Point(point.X, image.Size.Height);

            return DrawingHelper.DrawLine(canvas, start, end, 1, Brushes.Red, scaleValue);
        }

        private static void DrawInitialRowLine(Canvas canvas, double scaleValue)
        {
            if (DataProvider.GrayInitialImage != null)
                _initialRowLine = GetRowLine(canvas, DataProvider.GrayInitialImage, DataProvider.LastPosition, scaleValue);
            else if (DataProvider.ColorInitialImage != null)
                _initialRowLine = GetRowLine(canvas, DataProvider.ColorInitialImage, DataProvider.LastPosition, scaleValue);
        }

        private static void DrawProcessedRowLine(Canvas canvas, double scaleValue)
        {
            if (DataProvider.GrayProcessedImage != null)
                _processedRowLine = GetRowLine(canvas, DataProvider.GrayProcessedImage, DataProvider.LastPosition, scaleValue);
            else if (DataProvider.ColorProcessedImage != null)
                _processedRowLine = GetRowLine(canvas, DataProvider.ColorProcessedImage, DataProvider.LastPosition, scaleValue);
        }

        private static void DrawInitialColumnLine(Canvas canvas, double scaleValue)
        {
            if (DataProvider.GrayInitialImage != null)
                _initialColumnLine = GetColumnLine(canvas, DataProvider.GrayInitialImage, DataProvider.LastPosition, scaleValue);
            else if (DataProvider.ColorInitialImage != null)
                _initialColumnLine = GetColumnLine(canvas, DataProvider.ColorInitialImage, DataProvider.LastPosition, scaleValue);
        }

        private static void DrawProcessedColumnLine(Canvas canvas, double scaleValue)
        {
            if (DataProvider.GrayProcessedImage != null)
                _processedColumnLine = GetColumnLine(canvas, DataProvider.GrayProcessedImage, DataProvider.LastPosition, scaleValue);
            else if (DataProvider.ColorProcessedImage != null)
                _processedColumnLine = GetColumnLine(canvas, DataProvider.ColorProcessedImage, DataProvider.LastPosition, scaleValue);
        }

        private static void DrawInitialSquare(Canvas canvas, double scaleValue)
        {
            if (DataProvider.GrayInitialImage != null || DataProvider.ColorInitialImage != null)
                _initialSquare = GetSquare(canvas, DataProvider.LastPosition, scaleValue);
        }

        private static void DrawProcessedSquare(Canvas canvas, double scaleValue)
        {
            if (DataProvider.GrayProcessedImage != null || DataProvider.ColorProcessedImage != null)
                _processedSquare = GetSquare(canvas, DataProvider.LastPosition, scaleValue);
        }
    }
}