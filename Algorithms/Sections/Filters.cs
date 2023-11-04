using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Bgr, byte> FiltrulMedianVectorial(Image<Bgr, byte> image, int filterSize)
        {
            var img = new Image<Bgr, byte>(image.Size);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    img.Data[y, x, 0] = GetByteMedianValue(image, x, y, filterSize)[0];
                    img.Data[y, x, 1] = GetByteMedianValue(image, x, y, filterSize)[1];
                    img.Data[y, x, 2] = GetByteMedianValue(image, x, y, filterSize)[2];
                }
            }

            return img;
        }

        public static Image<Gray, byte> FiltrulMedianVectorial(Image<Gray, byte> image, int filterSize)
        {
            var img = new Image<Gray, byte>(image.Size);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    img.Data[y, x, 0] = GetByteMedianValue(image, x, y, filterSize)[0];
                }
            }

            return img;
        }

        private static byte[] GetByteMedianValue(Image<Gray, byte> image, int x, int y, int filterSize)
        {
            var list = new List<byte>();
            for (int xNew = Math.Max(0, y - filterSize / 2); xNew <= Math.Min(image.Height - 1, y + filterSize / 2); xNew++)
            {
                for (int yNew = Math.Max(0, x - filterSize / 2); yNew <= Math.Min(image.Width - 1, x + filterSize / 2); yNew++)
                {
                    list.Add(image.Data[xNew, yNew, 0]);
                }
            }
            list.Sort();
            return new byte[] { list[list.Count / 2] };
        }

        private static byte[] GetByteMedianValue(Image<Bgr, byte> image, int x, int y, int filterSize)
        {
            var listB = new List<byte>();
            var listG = new List<byte>();
            var listR = new List<byte>();
            for (int xNew = Math.Max(0, y - filterSize / 2); xNew <= Math.Min(image.Height - 1, y + filterSize / 2); xNew++)
            {
                for (int yNew = Math.Max(0, x - filterSize / 2); yNew <= Math.Min(image.Width - 1, x + filterSize / 2); yNew++)
                {
                    listB.Add(image.Data[xNew, yNew, 0]);
                    listG.Add(image.Data[xNew, yNew, 1]);
                    listR.Add(image.Data[xNew, yNew, 2]);
                }
            }
            listB.Sort();
            listG.Sort();
            listR.Sort();
            return new byte[] { listB[listB.Count / 2], listG[listG.Count / 2], listR[listR.Count / 2] };
        }
    }
}