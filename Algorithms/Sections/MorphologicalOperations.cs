using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading.Tasks;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static Image<Gray, byte> ClosingGray(Image<Gray, byte> image, int kernelSize)
        {
            var img = DilationGray(image, kernelSize);
            img = ErosionGray(img, kernelSize);

            return img;
        }

        private static Image<Gray, byte> DilationGray(Image<Gray, byte> image, int kernelSize)
        {
            var img = new Image<Gray, byte>(image.Size);
            int kernelOffset = (kernelSize - 1) / 2;

            Parallel.For(kernelOffset, img.Height - kernelOffset, y =>
            {
                for (int x = kernelOffset; x < img.Width - kernelOffset; x++)
                {
                    byte[] maxBgr = new byte[] { 0, 0, 0 };
                    for (int t = -kernelOffset; t < kernelOffset; t++)
                    {
                        for (int s = -kernelOffset; s < kernelOffset; s++)
                        {
                            var aux = image.Data[Math.Max(0, y + s), Math.Max(0, x + t), 0];

                            if (aux > maxBgr[0])
                            {
                                maxBgr[0] = (byte)aux;
                            }
                        }
                    }
                    img.Data[y, x, 0] = maxBgr[0];
                }
            });

            return img;
        }

        private static Image<Gray, byte> ErosionGray(Image<Gray, byte> image, int kernelSize)
        {
            var img = new Image<Gray, byte>(image.Size);
            int kernelOffset = (kernelSize - 1) / 2;

            Parallel.For(kernelOffset, img.Height - kernelOffset, y =>
            {
                for (int x = kernelOffset; x < img.Width - kernelOffset; x++)
                {
                    byte[] minBgr = new byte[] { 255, 255, 255 };
                    for (int t = -kernelOffset; t < kernelOffset; t++)
                    {
                        for (int s = -kernelOffset; s < kernelOffset; s++)
                        {
                            var aux = image.Data[Math.Max(0, y + s), Math.Max(0, x + t), 0];

                            if (aux < minBgr[0])
                            {
                                minBgr[0] = (byte)aux;
                            }
                        }
                    }
                    img.Data[y, x, 0] = minBgr[0];
                }
            });

            return img;
        }
    }
}