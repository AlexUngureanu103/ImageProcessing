using Emgu.CV;
using Emgu.CV.Structure;
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

            var structuringElement = new byte[kernelSize, kernelSize];
            if (kernelSize == 3)
            {
                structuringElement = new byte[3, 3]
                {
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 0 }
                };
            }

            Parallel.For(kernelOffset, img.Height - kernelOffset, y =>
            {
                for (int x = kernelOffset; x < img.Width - kernelOffset; x++)
                {
                    int maxBgr = 0;
                    for (int t = -kernelOffset; t < kernelOffset; t++)
                    {
                        for (int s = -kernelOffset; s < kernelOffset; s++)
                        {
                            int aux = image.Data[y - t, x - s, 0] + structuringElement[s + kernelOffset, t + kernelOffset];

                            if (aux > maxBgr)
                            {
                                maxBgr = aux;
                            }
                        }
                    }
                    img.Data[y, x, 0] = (byte)maxBgr;
                }
            });

            return img;
        }

        private static Image<Gray, byte> ErosionGray(Image<Gray, byte> image, int kernelSize)
        {
            var img = new Image<Gray, byte>(image.Size);
            int kernelOffset = (kernelSize - 1) / 2;

            var structuringElement = new byte[kernelSize, kernelSize];
            if (kernelSize == 3)
            {
                structuringElement = new byte[3, 3]
                {
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 0 }
                };
            }

            Parallel.For(kernelOffset, img.Height - kernelOffset, y =>
            {
                for (int x = kernelOffset; x < img.Width - kernelOffset; x++)
                {
                    int minBgr = 255;
                    for (int t = -kernelOffset; t < kernelOffset; t++)
                    {
                        for (int s = -kernelOffset; s < kernelOffset; s++)
                        {
                            int aux = image.Data[y - t, x - s, 0] - structuringElement[s + kernelOffset, t + kernelOffset];

                            if (aux < minBgr)
                            {
                                minBgr = aux;
                            }
                        }
                    }
                    img.Data[y, x, 0] = (byte)minBgr;
                }
            });

            return img;
        }
    }
}