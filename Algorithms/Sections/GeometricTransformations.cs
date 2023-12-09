using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading.Tasks;

namespace Algorithms.Sections
{
    public class GeometricTransformations
    {
        public static Image<Gray, byte> SphericalDeformation(Image<Gray, byte> image, double reflectionIndex, double rMax)
        {
            var img = new Image<Gray, byte>(image.Width, image.Height);

            var x0 = image.Width / 2;
            var y0 = image.Height / 2;

            Parallel.For(0, img.Height, y =>
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var dx = x - x0;
                    var dy = y - y0;
                    var r = Math.Sqrt(dx * dx + dy * dy);

                    if (r >= rMax)
                    {
                        continue;
                    }

                    var z = Math.Sqrt(rMax * rMax - r * r);
                    var dxSqrt = Math.Sqrt(dx * dx + z * z);
                    var dySqrt = Math.Sqrt(dy * dy + z * z);
                    var betaX = (1 - 1 / reflectionIndex) * Math.Asin(dx / dxSqrt);
                    var betaY = (1 - 1 / reflectionIndex) * Math.Asin(dy / dySqrt);

                    var xInverse = Math.Max(0, x0 - z * Math.Tan(betaX));
                    var yInverse = Math.Max(0, y0 - z * Math.Tan(betaY));


                    if (xInverse >= 0 && xInverse < image.Width && yInverse >= 0 && yInverse < image.Height)
                    {
                        img.Data[y, x, 0] = image.Data[(byte)Math.Round(yInverse), (byte)Math.Round(xInverse), 0];
                    }
                }
            });

            return img;
        }

        private static double[,] SphericalDeformationData(Image<Gray, byte> image, double refractionIndex, double rMax)
        {
            var pixelData = new double[image.Height, image.Width];

            var x0 = image.Width / 2;
            var y0 = image.Height / 2;

            Parallel.For(0, image.Height, y =>
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var dx = x - x0;
                    var dy = y - y0;
                    var r = Math.Sqrt(dx * dx + dy * dy);

                    if (r >= rMax)
                    {
                        continue;
                    }

                    var z = Math.Sqrt(rMax * rMax - r * r);

                    var dxSqrt = Math.Sqrt(dx * dx + z * z);
                    var betaX = (1 - 1 / refractionIndex) * Math.Asin(dx / dxSqrt);

                    var dySqrt = Math.Sqrt(dy * dy + z * z);
                    var betaY = (1 - 1 / refractionIndex) * Math.Asin(dy / dySqrt);

                    var xInverse = Math.Max(0, x0 - z * Math.Tan(betaX));
                    var yInverse = Math.Max(0, y0 - z * Math.Tan(betaY));

                    xInverse = Math.Round(xInverse);
                    yInverse = Math.Round(yInverse);

                    xInverse = Math.Min(xInverse, image.Width - 1);
                    yInverse = Math.Min(yInverse, image.Height - 1);

                    if (xInverse >= 0 && xInverse < image.Width && yInverse >= 0 && yInverse < image.Height)
                    {
                        pixelData[y, x] = image.Data[(byte)yInverse, (byte)xInverse, 0];
                    }
                }
            });

            return pixelData;
        }

        public static Image<Bgr, byte> SphericalDeformation(Image<Bgr, byte> image, double reflectionIndex, double rMax)
        {
            var resultImg = new Image<Bgr, byte>(image.Size);

            var imgBDeformed = SphericalDeformationData(image[0], reflectionIndex, rMax);
            var imgGDeformed = SphericalDeformationData(image[1], reflectionIndex, rMax);
            var imgRDeformed = SphericalDeformationData(image[2], reflectionIndex, rMax);

            Parallel.For(0, resultImg.Height, y =>
            {
                for (int x = 0; x < resultImg.Width; x++)
                {
                    resultImg.Data[y, x, 0] = (byte)imgBDeformed[y, x];
                    resultImg.Data[y, x, 1] = (byte)imgGDeformed[y, x];
                    resultImg.Data[y, x, 2] = (byte)imgRDeformed[y, x];
                }
            });

            return resultImg;
        }
    }
}