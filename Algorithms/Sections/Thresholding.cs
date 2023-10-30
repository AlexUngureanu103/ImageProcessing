using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        public static Image<Gray, byte> TriangleThresholding(Image<Gray, byte> image)
        {
            //double[] testHistogram = {
            //      0,      0,     4,    14,    15,    10,    24,    31,    57,    48,
            //      50,     62,    71,   106,    99,   113,   121,   156,   161,   189,
            //      171,    163,   199,   179,   203,   171,   201,   175,   172,   162,
            //      156,    143,   148,   132,   149,   145,   137,   148,   137,   109,
            //      117,    121,   111,   106,    94,   123,   122,   118,   135,   111,
            //      127,    129,   120,   138,   162,   156,   151,   162,   229,   225,
            //      276,    336,   437,   493,   631,   756,   905,  1053,  1176,  1449,
            //      1471,   1837,  2252,  2465,  3783, 11056, 16892,   284};


            var img = image.Clone();
            var histogram = GrayHistogram(img);
            int threshold = TriangleThreshold(histogram);
            img = Algorithms.Tools.Tools.Thresholding(img, threshold);

            //MessageBox.Show(threshold.ToString());
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

        private static int TriangleThreshold(double[] histogram)
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

    }
}