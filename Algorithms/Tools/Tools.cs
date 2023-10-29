using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
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
            TriangleThreshold_V2(histogram);

            int threshold = TriangleThreshold(histogram);

            int threshold2 = TriangleMethod(img);

            int threshold3 = TriangleThreshold_V3(histogram);

            img = img.ThresholdBinary(new Gray(threshold3), new Gray(255));

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

        private static int TriangleThreshold_V3(double[] histogram)
        {
            int min = 0;
            int max = 0;
            int min2 = 0;
            int dmax = 0;

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

            for (int i = histogram.Length - 1; i > 0; i--)
            {
                if (histogram[i] > 0)
                {
                    min2 = i;
                    break;
                }
            }
            if(min2 <histogram.Length - 1)
            {
                min2++;
            }

            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i]> dmax)
                {
                    max = i;
                    dmax = (int)histogram[i];
                }
            }

            bool inverted = false;
            if((max-min)< (min2-max))
            {
                inverted = true;
                int left = 0;
                int right = histogram.Length - 1;

                while (left < right)
                {
                    double temp = histogram[left];
                    histogram[left] = histogram[right];
                    histogram[right] = temp;

                    left++;
                    right--;
                }

                min = histogram.Length - 1 - min2;
                max = histogram.Length - 1 - max;
            }

            if( min == max)
            {
                return min;
            }

            double nx;
            double ny;
            double d;

            nx = histogram[max];
            ny = min - max;
            d = Math.Sqrt(nx * nx + ny * ny);
            nx /= d;
            ny /= d;
            d = nx * min + ny * histogram[min];

            int split = min;
            double splitDistance = 0;
            for (int i = min +1; i <= max; i++)
            {
                double newDistance = nx * i + ny * histogram[i] - d;
                if(newDistance > splitDistance)
                {
                    split = i;
                    splitDistance = newDistance;
                }
            }
            split--;

            if(inverted)
            {
                int left = 0;
                int right = histogram.Length - 1;
                while(left < right)
                {
                    double temp = histogram[left];
                    histogram[left] = histogram[right];
                    histogram[right] = temp;
                    left++;
                    right--;
                }
                split = histogram.Length - 1 - split;
            }

            return split;
        }

        private static void TriangleThreshold_V2(double[] histogram)
        {
            var histogramMax = double.MinValue;
            var histogramMin = double.MaxValue;

            var histogramMaxPos = new List<int>();
            var histogramMinPos = new List<int>();

            for (int i = 0; i < histogram.Length; ++i)
            {
                //if (histogram[i] == 0)
                //{
                //    continue;
                //}
                if (histogram[i] == histogramMax)
                {
                    histogramMaxPos.Add(i);
                }
                else if (histogram[i] > histogramMax)
                {
                    histogramMax = histogram[i];
                    histogramMaxPos.Clear();
                    histogramMaxPos.Add(i);
                }
                if (histogram[i] == histogramMin)
                {
                    histogramMinPos.Add(i);
                }
                else if (histogram[i] < histogramMin)
                {
                    histogramMin = histogram[i];
                    histogramMinPos.Clear();
                    histogramMinPos.Add(i);
                }
            }

            int min_idx = -1;
            int max_idx = -1;
            int bin_dist = -1;

            foreach (var i in histogramMinPos)
            {
                foreach (var j in histogramMaxPos)
                {
                    int dist_tmp = i - j;
                    if (Math.Abs(dist_tmp) > bin_dist)
                    {
                        min_idx = i;
                        max_idx = j;
                        bin_dist = Math.Abs(dist_tmp);
                    }
                }
            }

            int start_idx;
            int stop_idx;

            if (min_idx < max_idx)
            {
                start_idx = min_idx + 1; // add one because we don't need to check the endpoint bin
                stop_idx = max_idx;
            }
            else
            {
                start_idx = max_idx + 1;
                stop_idx = min_idx;
            }

            double h = -1;
            int split = 0;
            histogramMax -= histogramMin;
            for (int i = start_idx; i < stop_idx; i++)
            {
                double h_tmp = (Math.Abs(min_idx - i) / bin_dist) - ((histogram[i] - histogramMin) / histogramMax);
                if (h_tmp > h)
                {
                    h = h_tmp;
                    split = i;
                }
            }

            MessageBox.Show($"h: {h}, split: {split}");
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
            double numerator = Math.Abs(((x2 - x1) * (y1 - y0)) - ((x1 - x0) * (y2 - y1)));
            double denominator = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return numerator / denominator;
        }

        public static int TriangleMethod(Image<Gray, byte> grayImage)
        {
            double[] histogram = GrayHistogram(grayImage);
            double histogramMax = double.MinValue;
            double histogramMin = double.MaxValue;

            int histogramMaxPos = 0;
            int histogramMinPos = 0;

            for (int i = 0; i < histogram.Length; ++i)
            {
                //if (histogram[i] == 0)
                //{
                //    continue;
                //}
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
                if (histogram[i] == histogramMin && Math.Abs(i - histogramMaxPos) > Math.Abs(histogramMinPos - histogramMaxPos))
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