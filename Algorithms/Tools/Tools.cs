﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        public static Image<Bgr, byte> Convert(Image<Gray, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Convert<Bgr, byte>();
            return result;
        }
        #endregion

        #region Thresholding

        public static Image<Gray, byte> Thresholding(Image<Gray, byte> inputImage, byte treshold)
        {
            Image<Gray, byte> emguCvImg = inputImage.ThresholdBinary(new Gray(treshold), new Gray(255));

            return emguCvImg;
        }

        public static Image<Gray, byte> TriangleThresholding(Image<Gray, byte> image)
        {
            var img = image.Clone();

            var histogram = GrayHistogram(img);

            int threshold = TriangleThreshold(histogram);

            int threshold2 = TriangleMethod(img);

            img = img.ThresholdBinary(new Gray(threshold), new Gray(255));

            return img;
        }

        public static double[] GrayHistogram(Image<Gray, byte> grayImage)
        {
            double[] histogram = new double[256];

            for (int y = 0; y < grayImage.Size.Height; ++y)
                for (int x = 0; x < grayImage.Size.Width; ++x)
                    histogram[grayImage.Data[y, x, 0]] += 1;

            //for (int i = 0; i < 256; ++i)
            //    histogram[i] /= grayImage.Size.Height * grayImage.Size.Width;

            return histogram;
        }

        private static int TriangleThreshold(double[] histogram)
        {
            int threshold = 0;
            double minEntropy = double.MaxValue;

            for (int i = 0; i < histogram.Length; i++)
            {
                double w1 = 0;
                double w2 = 0;
                double u1 = 0;
                double u2 = 0;

                for (int j = 0; j <= i; j++)
                {
                    w1 += histogram[j];
                    u1 += j * histogram[j];
                }

                for (int j = i + 1; j < histogram.Length; j++)
                {
                    w2 += histogram[j];
                    u2 += j * histogram[j];
                }

                if (w1 == 0 || w2 == 0)
                {
                    continue;
                }

                u1 /= w1;
                u2 /= w2;

                double entropy = w1 * Math.Log(w1) + w2 * Math.Log(w2);

                if (entropy < minEntropy)
                {
                    minEntropy = entropy;
                    threshold = i;
                }
            }

            return threshold;
        }

        private static double PointToLineDistance(double x0, double y0, double x1, double y1, double x2, double y2)
        {
            double numerator = System.Math.Abs(((x2 - x1) * (y1 - y0)) - ((x1 - x0) * (y2 - y1)));
            double denominator = System.Math.Sqrt(System.Math.Pow(x2 - x1, 2) + System.Math.Pow(y2 - y1, 2));
            return numerator / denominator;
        }

        public static int TriangleMethod(Image<Gray, byte> grayImage)
        {
            double[] histogram = GrayHistogram(grayImage);
            double histogramMax = -0.1, histogramMin = 1.1;
            int histogramMaxPos = 0, histogramMinPos = 0;

            for (int i = 0; i < histogram.Length; ++i)
            {
                if (histogram[i] > histogramMax)
                {
                    histogramMax = histogram[i];
                    histogramMaxPos = i;
                }
                if (histogram[i] < histogramMin)
                {
                    histogramMin = histogram[i];
                    histogramMinPos = i;
                }
            }

            for (int i = 0; i < histogram.Length; ++i)
                if (histogram[i] == histogramMin && System.Math.Abs(i - histogramMaxPos) > System.Math.Abs(histogramMinPos - histogramMaxPos))
                    histogramMinPos = i;

            double xMax, yMax, xMin, yMin, xCrt, yCrt, distMax = -1;
            int result = 0;
            yMax = 1;
            yMin = 0;

            if (histogramMaxPos < histogramMinPos)
            {
                xMax = 0;
                xMin = 1;
            }
            else
            {
                xMax = 1;
                xMin = 0;
            }

            for (int i = System.Math.Min(histogramMaxPos, histogramMinPos) + 1; i < System.Math.Max(histogramMaxPos, histogramMinPos); ++i)
            {
                double num1 = i - System.Math.Min(histogramMaxPos, histogramMinPos);
                double num2 = System.Math.Abs(histogramMaxPos - histogramMinPos);

                xCrt = num1 / num2;
                yCrt = histogram[i];
                double distance = PointToLineDistance(xCrt, yCrt, xMax, yMax, xMin, yMin);
                if (distance > distMax)
                {
                    distMax = distance;
                    result = i;
                }
            }

            return result;
        }
        #endregion

        #region GetPoints

        public static Point GetTopLeftPoint(Point firstPoint, Point secondPoint)
        {
            Point topLeftPoint = new Point(Math.Min(firstPoint.X, secondPoint.X), Math.Min(firstPoint.Y, secondPoint.Y));
            return topLeftPoint;
        }

        public static Point GetBottomRightPoint(Point firstPoint, Point secondPoint)
        {
            Point bottomRightPoint = new Point(Math.Max(firstPoint.X, secondPoint.X), Math.Max(firstPoint.Y, secondPoint.Y));
            return bottomRightPoint;
        }

        #endregion

        #region Crop Image

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Point firstPoint, Point secondPoint)
        {
            Point topLeftPoint = GetTopLeftPoint(firstPoint, secondPoint);
            Point bottomRightPoint = GetBottomRightPoint(firstPoint, secondPoint);

            Image<Bgr, byte> result = new Image<Bgr, byte>((int)(bottomRightPoint.X - topLeftPoint.X + 1), (int)(bottomRightPoint.Y - topLeftPoint.Y + 1));

            for (int y = 0; y < (int)(bottomRightPoint.Y - topLeftPoint.Y); y++)
            {
                for (int x = 0; x < (int)(bottomRightPoint.X - topLeftPoint.X); x++)
                {

                    result.Data[y, x, 0] = inputImage.Data[y + (int)topLeftPoint.Y, x + (int)topLeftPoint.X, 0];
                    result.Data[y, x, 1] = inputImage.Data[y + (int)topLeftPoint.Y, x + (int)topLeftPoint.X, 1];
                    result.Data[y, x, 2] = inputImage.Data[y + (int)topLeftPoint.Y, x + (int)topLeftPoint.X, 2];
                }
            }

            return result;
        }

        #endregion

        #region Mirror Image

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Flip(Emgu.CV.CvEnum.FlipType.Vertical);

            return result;
        }

        #endregion

        #region Rotate Image

        public static Image<Bgr, byte> RotateImage(Image<Bgr, byte> inputImage, double angle)
        {
            inputImage = inputImage.Rotate(angle, new Bgr(0, 0, 0), false);

            return inputImage;
        }

        #endregion
    }
}