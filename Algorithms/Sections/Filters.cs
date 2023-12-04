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
        public static (Image<Gray, byte>, Image<Bgr, byte>) Sobel(Image<Gray, byte> image, int tMin)
        {
            var img = new Image<Gray, byte>(image.Size);
            var angleImg = new Image<Bgr, byte>(image.Size);


            var sx = new int[3, 3] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 } };

            var sy = new int[3, 3] {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 } };

            var kernelOffset = 1;


            for (int y = kernelOffset; y < image.Height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < image.Width - kernelOffset; x++)
                {
                    var fxValue = 0;
                    var fyValue = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            byte grayValue = image.Data[y + i, x + j, 0];

                            fxValue += grayValue * sx[i + 1, j + 1];
                            fyValue += grayValue * sy[i + 1, j + 1];
                        }
                    }

                    var gradient = Math.Round(Math.Min(255, Math.Sqrt(fxValue * fxValue + fyValue * fyValue)));
                    if (gradient >= tMin)
                    {
                        if (fxValue == 0)
                        {
                            angleImg.Data[y, x, 0] = 0;
                            angleImg.Data[y, x, 1] = 0;
                            angleImg.Data[y, x, 2] = 255;
                        }
                        else
                        {
                            var direction = Math.Atan(fyValue / fxValue);
                            var angle = MapDirection(direction);

                            angleImg.Data[y, x, 0] = angle[0];
                            angleImg.Data[y, x, 1] = angle[1];
                            angleImg.Data[y, x, 2] = angle[2];
                        }
                    }
                    else
                    {
                        angleImg.Data[y, x, 0] = 0;
                        angleImg.Data[y, x, 1] = 0;
                        angleImg.Data[y, x, 2] = 0;
                    }

                    img.Data[y, x, 0] = (byte)gradient;
                }
            }
            return (img, angleImg);
        }

        private static byte[] MapDirection(double direction)
        {
            var bytes = new byte[3];
            if (direction > Math.PI / 2)
            {
                direction -= Math.PI;
            }
            else if (direction < -Math.PI / 2)
            {
                direction += Math.PI;
            }

            //RED
            if ((direction >= -Math.PI / 2 && direction <= -Math.PI * 3 / 8) ||
               (direction <= Math.PI / 2 && direction >= Math.PI * 3 / 8))
            {
                bytes = new byte[] { 0, 0, 255 };
            }
            //BLUE
            else if (direction >= Math.PI / 8 && direction <= Math.PI * 3 / 8)
            {
                bytes = new byte[] { 255, 0, 0 };
            }
            //GREEN
            else if (direction >= -Math.PI / 8 && direction <= Math.PI / 8)
            {
                bytes = new byte[] { 0, 255, 0 };
            }
            //YELLOW
            else if (direction >= -Math.PI * 3 / 8 && direction <= -Math.PI / 8)
            {
                bytes = new byte[] { 0, 255, 255 };
            }

            return bytes;
        }

        public static Image<Gray, byte> NonMaximaSupression(Image<Gray, byte> gradientImage, Image<Bgr, byte> angleImage)
        {
            var img = new Image<Gray, byte>(gradientImage.Size);

            for (int y = 2; y < img.Height - 2; y++)
            {
                for (int x = 2; x < img.Width - 2; x++)
                {
                    // d0 0 0 255
                    // d1 0 255 255
                    // d2 0 255 0
                    // d3 255 0 0
                    var (blue, green, red) = (angleImage.Data[y, x, 0], angleImage.Data[y, x, 1], angleImage.Data[y, x, 2]);

                    if (blue == 0 && green == 255 && red == 0)
                    {
                        if (/*gradientImage.Data[y, x, 0] >= gradientImage.Data[y, x - 2, 0] &&*/
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y, x - 1, 0] &&
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y, x + 1, 0] /*&&*/
                            /*gradientImage.Data[y, x, 0] >= gradientImage.Data[y, x + 2, 0]*/)
                        {
                            img.Data[y, x, 0] = gradientImage.Data[y, x, 0];
                        }
                        else
                        {
                            img.Data[y, x, 0] = 0;
                        }
                    }
                    else if (blue == 255 && green == 0 && red == 0)
                    {
                        if (/*gradientImage.Data[y, x, 0] >= gradientImage.Data[y - 2, x + 2, 0] &&*/
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y - 1, x + 1, 0] &&
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y + 1, x - 1, 0]/* &&*/
                            /*gradientImage.Data[y, x, 0] >= gradientImage.Data[y + 2, x - 2, 0]*/)
                        {
                            img.Data[y, x, 0] = gradientImage.Data[y, x, 0];
                        }
                        else
                        {
                            img.Data[y, x, 0] = 0;
                        }
                    }
                    else if (blue == 0 && green == 0 && red == 255)
                    {
                        if (/*gradientImage.Data[y, x, 0] >= gradientImage.Data[y - 2, x, 0] &&*/
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y - 1, x, 0] &&
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y + 1, x, 0] /*&&*/
                            /*gradientImage.Data[y, x, 0] >= gradientImage.Data[y + 2, x, 0]*/)
                        {
                            img.Data[y, x, 0] = gradientImage.Data[y, x, 0];
                        }
                        else
                        {
                            img.Data[y, x, 0] = 0;
                        }
                    }
                    else if (blue == 0 && green == 255 && red == 255)
                    {
                        if (/*gradientImage.Data[y, x, 0] >= gradientImage.Data[y - 2, x - 2, 0] &&*/
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y - 1, x - 1, 0] &&
                            gradientImage.Data[y, x, 0] >= gradientImage.Data[y + 1, x + 1, 0] /*&&*/
                            /*gradientImage.Data[y, x, 0] >= gradientImage.Data[y + 2, x + 2, 0]*/)
                        {
                            img.Data[y, x, 0] = gradientImage.Data[y, x, 0];
                        }
                        else
                        {
                            img.Data[y, x, 0] = 0;
                        }
                    }
                    else
                    {
                        img.Data[y, x, 0] = 0;
                    }
                }
            }

            return img;
        }

        #region Filtrul median vectorial

        public static Image<Bgr, byte> FiltrulMedianVectorial(Image<Bgr, byte> image, int filterSize)
        {
            var img = new Image<Bgr, byte>(image.Size);

            Parallel.For(0, img.Height, y =>
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var bgr = GetByteMedianValue(image, x, y, filterSize);
                    img.Data[y, x, 0] = bgr[0];
                    img.Data[y, x, 1] = bgr[1];
                    img.Data[y, x, 2] = bgr[2];
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

        #endregion
    }
}