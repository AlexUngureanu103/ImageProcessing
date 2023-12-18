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

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var dx = x - x0;
                    var dy = y - y0;
                    var r = Math.Sqrt(dx * dx + dy * dy);

                    if (r <= rMax)
                    {
                        var z = Math.Sqrt(rMax * rMax - r * r);
                        var dxSqrt = Math.Sqrt(dx * dx + z * z);
                        var dySqrt = Math.Sqrt(dy * dy + z * z);
                        var betaX = (1 - 1 / reflectionIndex) * Math.Asin(dx / dxSqrt);
                        var betaY = (1 - 1 / reflectionIndex) * Math.Asin(dy / dySqrt);

                        var xInverse = Math.Max(0, x - z * Math.Tan(betaX));
                        var yInverse = Math.Max(0, y - z * Math.Tan(betaY));

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
                    else
                    {
                        img.Data[y, x, 0] = image.Data[y, x, 0];
                    }
                }
            };

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

                    if (r <= rMax)
                    {
                        var z = Math.Sqrt(rMax * rMax - r * r);

                        var dxSqrt = Math.Sqrt(dx * dx + z * z);
                        var betaX = (1 - 1 / refractionIndex) * Math.Asin(dx / dxSqrt);

                        var dySqrt = Math.Sqrt(dy * dy + z * z);
                        var betaY = (1 - 1 / refractionIndex) * Math.Asin(dy / dySqrt);

                        var xInverse = Math.Max(0, x - z * Math.Tan(betaX));
                        var yInverse = Math.Max(0, y - z * Math.Tan(betaY));

                        xInverse = Math.Min(xInverse, image.Width - 1);
                        yInverse = Math.Min(yInverse, image.Height - 1);

                        pixelData[y, x] = (byte)BiliniarInterpolation(image, xInverse, yInverse);
                    }
                    else
                    {
                        pixelData[y, x] = image.Data[y, x, 0];
                    }
                }
            });

            return pixelData;
        }

        private static double BiliniarInterpolation(Image<Gray, byte> image, double xInverse, double yInverse)
        {
            var x1 = (int)Math.Floor(xInverse);// [xc]
            var y1 = (int)Math.Floor(yInverse);// [yc]
            var x2 = x1 + 1;// ([xc] + 1)
            var y2 = y1 + 1;// ([yc] + 1)

            var weightX2 = xInverse - x1; // {xc}
            var weightX1 = 1 - weightX2; //  1-{xc}
            var weightY2 = yInverse - y1; // {yc}
            var weightY1 = 1 - weightY2; // 1-{yc}

            x1 = Math.Max(0, Math.Min(x1, image.Width - 1));
            y1 = Math.Max(0, Math.Min(y1, image.Height - 1));
            x2 = Math.Max(0, Math.Min(x2, image.Width - 1));
            y2 = Math.Max(0, Math.Min(y2, image.Height - 1));

            var pixelValue = weightX1 * weightY1 * image.Data[y1, x1, 0] +
                             weightX2 * weightY1 * image.Data[y1, x2, 0] +
                             weightX1 * weightY2 * image.Data[y2, x1, 0] +
                             weightX2 * weightY2 * image.Data[y2, x2, 0];

            // var pixelValue = (1-{xc})    *   (1-{yc})    *   f(x1,y1) +
            //                  {xc}        *   (1-{yc})    *   f(x2,y1) +
            //                  (1-{xc})    *   {yc}        *   f(x1,y2) +
            //                  {xc}        *   {yc}        *   f(x2,y2)

            //  f (xc , [yc ]) = ({xc})f ([xc ] + 1, [yc ]) + (1 − {xc})f ([xc ], [yc ])
            //  f (xc , [yc ] + 1) = ({xc})f ([xc ] + 1, [yc ] + 1) + (1 − {xc})f ([xc ], [yc ] + 1)


            //  f(xc, yc) = ({ yc})f(xc, [yc] + 1) + (1 − { yc})f(xc, [yc])


            // Step: swap formulas from line 138 ans 139 into the formula from line 142

            //  f(xc, yc) =     { yc}       *   (   ({xc})f ([xc ] + 1, [yc ] + 1) + (1 − {xc})f ([xc ], [yc ] + 1)  ) +
            //                  (1 − { yc}) *   (   ({xc})f ([xc ] + 1, [yc ]) + (1 − {xc})f ([xc ], [yc ])   )


            //  STEP: swap [xc] and [yc] with x1 and y1 &
            //  ([xc] + 1 )and ([yc] + 1) with x2 and y2

            //  f(xc, yc) =     { yc}       *   (   ({xc})f (x2, y2) + (1 − {xc})f (x1, y2)     ) +
            //                  (1 − { yc}) *   (   ({xc})f (x2, y1) + (1 − {xc})f (x1, y1)     )

            //  STEP complete formulas:

            //  f(xc, yc) =     {yc}        *   {xc}        *    f(x2,y2) +             Line    136
            //                  {yc}        *   (1-{xc})    *    f(x1,y2) +             Line    135
            //                  (1-{yc})    *   {xc}        *    f(x2,y1) +             Line    134
            //                  (1-{yc})    *   (1-{xc})    *    f(x1,y1)               Line    133



            return pixelValue;
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