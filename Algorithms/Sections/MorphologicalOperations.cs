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
            //          img = ErosionGray(img, kernelSize);

            return img;
        }

        private static Image<Gray, byte> DilationGray(Image<Gray, byte> image, int kernelSize)
        {
            var img = new Image<Gray, byte>(image.Size);

            Parallel.For(0, img.Height, y =>
            {
                for (int x = 0; x < img.Width; x++)
                {
                    byte[] maxBgr = new byte[] { 0, 0, 0 };
                    for (int t = Math.Max(0, y - kernelSize / 2); t <= Math.Min(image.Height - 1, y + kernelSize / 2); t++)
                    {
                        for (int s = Math.Max(0, x - kernelSize / 2); s <= Math.Min(image.Width - 1, x + kernelSize / 2); s++)
                        {
                            var aux = image.Data[Math.Max(0, x - s), Math.Max(0, y - t), 0] + image.Data[s, t, 0];

                            if (aux > maxBgr[0])
                            {
                                maxBgr[0] = (byte)aux;
                            }
                        }
                    }
                    img.Data[x, y, 0] = maxBgr[0];
                }
            });

            return img;
        }

        private static Image<Gray, byte> ErosionGray(Image<Gray, byte> image, int kernelSize)
        {
            var img = new Image<Gray, byte>(image.Size);

            Parallel.For(0, img.Height, y =>
            {
                for (int x = 0; x < img.Width; x++)
                {
                    byte[] maxBgr = new byte[] { 0, 0, 0 };
                    for (int t = Math.Max(0, y - kernelSize / 2); t <= Math.Min(image.Height - 1, y + kernelSize / 2); t++)
                    {
                        for (int s = Math.Max(0, x - kernelSize / 2); s <= Math.Min(image.Width - 1, x + kernelSize / 2); s++)
                        {
                            var aux = image.Data[Math.Max(0, x - s), Math.Max(0, y - t), 0] - image.Data[s, t, 0];

                            if (aux > maxBgr[0])
                            {
                                maxBgr[0] = (byte)aux;
                            }
                        }
                    }
                    img.Data[x, y, 0] = maxBgr[0];
                }
            });

            return img;
        }
    }
}