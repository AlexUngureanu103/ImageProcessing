using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Bgr, byte> FiltrulMedianVectorial(Image<Bgr, byte> image, int filterSize)
        {
            var img = new Image<Bgr, byte>(image.Size);

            Parallel.For(0, img.Height, y =>
            {
                for (int x = 0; x < img.Width; x++)
                {
                    img.Data[y, x, 0] = GetByteMedianValue(image, x, y, filterSize)[0];
                    img.Data[y, x, 1] = GetByteMedianValue(image, x, y, filterSize)[1];
                    img.Data[y, x, 2] = GetByteMedianValue(image, x, y, filterSize)[2];
                }
            });

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
            var listBgr = new List<byte[]>(filterSize * filterSize);
            for (int xNew = Math.Max(0, y - filterSize / 2); xNew <= Math.Min(image.Height - 1, y + filterSize / 2); xNew++)
            {
                for (int yNew = Math.Max(0, x - filterSize / 2); yNew <= Math.Min(image.Width - 1, x + filterSize / 2); yNew++)
                {
                    listBgr.Add(new byte[] { image.Data[xNew, yNew, 0], image.Data[xNew, yNew, 0], image.Data[xNew, yNew, 0] });
                }
            }

            var matrix = new List<List<double>>(filterSize);
            for (int i = 0; i < listBgr.Count; i++)
            {
                matrix.Add(new List<double>(filterSize));
                for (int j = 0; j < listBgr.Count; j++)
                {
                    matrix[i].Add(GetDistance(listBgr[i], listBgr[j]));
                }
            }

            var sumList = matrix.Select(row => row.Sum()).ToList();
            var minIndex = sumList.IndexOf(sumList.Min());

            return listBgr[minIndex];
        }

        private static byte[] GetByteMedianValue(Image<Bgr, byte> image, int x, int y, int filterSize)
        {
            var listBgr = new List<byte[]>(filterSize * filterSize);
            for (int xNew = Math.Max(0, y - filterSize / 2); xNew <= Math.Min(image.Height - 1, y + filterSize / 2); xNew++)
            {
                for (int yNew = Math.Max(0, x - filterSize / 2); yNew <= Math.Min(image.Width - 1, x + filterSize / 2); yNew++)
                {
                    listBgr.Add(new byte[] { image.Data[xNew, yNew, 0], image.Data[xNew, yNew, 1], image.Data[xNew, yNew, 2] });
                }
            }

            var matrix = new List<List<double>>(filterSize);
            for (int i = 0; i < listBgr.Count; i++)
            {
                matrix.Add(new List<double>(filterSize));
                for (int j = 0; j < listBgr.Count; j++)
                {
                    matrix[i].Add(GetDistance(listBgr[i], listBgr[j]));
                }
            }

            var sumList = matrix.Select(row => row.Sum()).ToList();
            var minIndex = sumList.IndexOf(sumList.Min());

            return listBgr[minIndex];
        }

        private static double GetDistance(byte[] bgr1, byte[] bgr2)
        {
            return Math.Sqrt(Math.Pow(bgr1[0] - bgr2[0], 2) + Math.Pow(bgr1[1] - bgr2[1], 2) + Math.Pow(bgr1[2] - bgr2[2], 2));
        }
    }
}