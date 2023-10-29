using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;
using System.Windows.Forms;

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
            int threshold = TriangleThreshold_V3(histogram);
            MessageBox.Show(threshold.ToString());
            img = img.ThresholdBinary(new Gray(threshold), new Gray(255));

            return img;
        }

        public static double[] GrayHistogram(Image<Gray, byte> grayImage)
        {
            double[] histogram = new double[256];

            for (int y = 0; y < grayImage.Size.Height; ++y)
                for (int x = 0; x < grayImage.Size.Width; ++x)
                    histogram[grayImage.Data[y, x, 0]] += 1;

            return histogram;
        }

        private static int TriangleThreshold_V3(double[] histogram)
        {
            int minStart = 0;
            int max = 0;
            int minEnd = 0;
            int dmax = 0;

            minStart = GetMinFromStartOrEnd(histogram, minStart, true);
            minEnd = GetMinFromStartOrEnd(histogram, minEnd, false);
            GetMax(histogram, ref max, ref dmax);

            bool inverted = false;
            if ((max - minStart) < (minEnd - max))
            {
                inverted = true;
                InvertHistogram(histogram);

                minStart = histogram.Length - 1 - minEnd;
                max = histogram.Length - 1 - max;
            }

            if (minStart == max)
            {
                return minStart;
            }

            double nx;
            double ny;
            double d;

            nx = histogram[max];
            ny = minStart - max;
            d = Math.Sqrt(nx * nx + ny * ny);
            nx /= d;
            ny /= d;
            d = nx * minStart + ny * histogram[minStart];

            int split = FindSplitPoint(histogram, minStart, max, nx, ny, d);

            if (inverted)
            {
                InvertHistogram(histogram);
                split = histogram.Length - 1 - split;
            }

            return split;
        }

        private static int FindSplitPoint(double[] histogram, int minStart, int max, double nx, double ny, double d)
        {
            int split = minStart;
            double splitDistance = 0;
            for (int i = minStart + 1; i <= max; i++)
            {
                double newDistance = nx * i + ny * histogram[i] - d;
                if (newDistance > splitDistance)
                {
                    split = i;
                    splitDistance = newDistance;
                }
            }
            split--;
            return split;
        }

        private static void InvertHistogram(double[] histogram)
        {
            int left = 0;
            int right = histogram.Length - 1;

            while (left < right)
            {
                (histogram[right], histogram[left]) = (histogram[left], histogram[right]);
                left++;
                right--;
            }
        }

        private static void GetMax(double[] histogram, ref int max, ref int dmax)
        {
            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] > dmax)
                {
                    max = i;
                    dmax = (int)histogram[i];
                }
            }
        }

        private static int GetMinFromStartOrEnd(double[] histogram, int min, bool isFromStart)
        {
            if (isFromStart)
            {
                for (int i = 0; i < histogram.Length; i++)
                {
                    if (histogram[i] > 0)
                    {
                        min = i;
                        break;
                    }
                }
                if (min > 0)
                {
                    min--;
                }

                return min;
            }
            else
            {
                for (int i = histogram.Length - 1; i > 0; i--)
                {
                    if (histogram[i] > 0)
                    {
                        min = i;
                        break;
                    }
                }
                if (min < histogram.Length - 1)
                {
                    min++;
                }

                return min;
            }
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