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

                    var x1 = (int)Math.Floor(xInverse);
                    var y1 = (int)Math.Floor(yInverse);
                    var x2 = x1 + 1;
                    var y2 = y1 + 1;

                    var weightX2 = xInverse - x1;
                    var weightX1 = 1 - weightX2;
                    var weightY2 = yInverse - y1;
                    var weightY1 = 1 - weightY2;

                    x1 = Math.Max(0, Math.Min(x1, image.Width - 1));
                    y1 = Math.Max(0, Math.Min(y1, image.Height - 1));
                    x2 = Math.Max(0, Math.Min(x2, image.Width - 1));
                    y2 = Math.Max(0, Math.Min(y2, image.Height - 1));

                    var pixelValue = weightX1 * weightY1 * image.Data[y1, x1, 0] +
                                     weightX2 * weightY1 * image.Data[y1, x2, 0] +
                                     weightX1 * weightY2 * image.Data[y2, x1, 0] +
                                     weightX2 * weightY2 * image.Data[y2, x2, 0];

                    img.Data[y, x, 0] = (byte)pixelValue;
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

                    var x1 = (int)Math.Floor(xInverse);
                    var y1 = (int)Math.Floor(yInverse);
                    var x2 = x1 + 1;
                    var y2 = y1 + 1;

                    var weightX2 = xInverse - x1;
                    var weightX1 = 1 - weightX2;
                    var weightY2 = yInverse - y1;
                    var weightY1 = 1 - weightY2;

                    x1 = Math.Max(0, Math.Min(x1, image.Width - 1));
                    y1 = Math.Max(0, Math.Min(y1, image.Height - 1));
                    x2 = Math.Max(0, Math.Min(x2, image.Width - 1));
                    y2 = Math.Max(0, Math.Min(y2, image.Height - 1));

                    var pixelValue = weightX1 * weightY1 * image.Data[y1, x1, 0] +
                                     weightX2 * weightY1 * image.Data[y1, x2, 0] +
                                     weightX1 * weightY2 * image.Data[y2, x1, 0] +
                                     weightX2 * weightY2 * image.Data[y2, x2, 0];

                    pixelData[y, x] = (byte)pixelValue;
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