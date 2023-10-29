using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        public static List<Point> GetSplineHermitCurvePoints(List<Point> entryPoints, double imageWidth, double imageHeight)
        {
            var controlPoints = new List<Point>
            {
                new Point(0, imageHeight)
            };
            for (int i = Math.Max(entryPoints.Count - 5, 0); i < entryPoints.Count; i++)
            {
                controlPoints.Add(entryPoints[i]);
            }
            controlPoints.Add(new Point(imageWidth, 0));

            var points = controlPoints.OrderBy(point => point.X).Select(point => new Point(point.X, imageHeight - point.Y)).ToList();
            points.Add(new Point(points[points.Count - 1].X - (points[points.Count - 2].X - points[points.Count - 1].X), points[points.Count - 2].Y));
            points.Add(new Point(-points[1].X, points[1].Y));
            points = points.OrderBy(point => point.X).ToList();

            List<Point> firstGen = GenerateCubicHermiteSplinePoints(points, 0.01d);

            List<Point> secondGen = GenerateCubicHermiteSplinePoints(firstGen, 0.3d)
                .Select(point => new Point(point.X, imageHeight - point.Y))
                .Where(point => point.X >= 0 && point.X <= imageWidth)
                .Select(point => new Point(point.X, point.Y < 0 ? 0 : point.Y))
                .ToList();

            return secondGen;
        }

        public static double NormalizeValue(double value, double size)
        {
            var normalizedValue = value / size * 255;
            return Math.Max(0, Math.Min(255, Math.Round(normalizedValue)));
        }

        public static Image<Gray, byte> UpdateGrayValues(Image<Gray, byte> initialImage, Dictionary<double, double> LUT)
        {
            var image = new Image<Gray, byte>(initialImage.Size);


            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    image.Data[y, x, 0] = (byte)LUT[initialImage.Data[y, x, 0]];
                }
            }

            return image;
        }

        public static Image<Bgr, byte> UpdateGrayValues(Image<Bgr, byte> initialImage, Dictionary<double, double> LUT)
        {
            var image = new Image<Bgr, byte>(initialImage.Size);


            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    image.Data[y, x, 0] = (byte)LUT[initialImage.Data[y, x, 0]];
                    image.Data[y, x, 1] = (byte)LUT[initialImage.Data[y, x, 1]];
                    image.Data[y, x, 2] = (byte)LUT[initialImage.Data[y, x, 2]];
                }
            }

            return image;
        }

        public static Dictionary<double, double> GetLUTValues(List<Point> curvePoints, double imageWidth, double imageHeight)
        {
            var lutValues = curvePoints
                .GroupBy(point => NormalizeValue(point.X, imageWidth))
                .ToDictionary(group => group.Key,
                    group => NormalizeValue(imageHeight - group.Sum(point => point.Y) / group.Count(), imageHeight));

            return lutValues;
        }

        private static List<Point> GenerateCubicHermiteSplinePoints(List<Point> points, double tStep)
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

    }
}