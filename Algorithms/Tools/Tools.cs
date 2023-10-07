﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        public static Image<Bgr, byte> Convert(Image<Gray, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Convert<Bgr, byte>();
            return result;
        }
        #endregion

        #region Thresholding

        public static Image<Gray, byte> Thresholding(Image<Gray, byte> inputImage, byte treshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] >= treshold)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                    else
                    {
                        result.Data[y, x, 0] = 0;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Crop Image


        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Point firstPoint, Point secondPoint)
        {
            Point topLeftPoint = new Point(Math.Min(firstPoint.X, secondPoint.X), Math.Min(firstPoint.Y, secondPoint.Y));
            Point bottomRightPoint = new Point(Math.Max(firstPoint.X, secondPoint.X), Math.Max(firstPoint.Y, secondPoint.Y));
            Image<Bgr, byte> result = new Image<Bgr, byte>((int)(bottomRightPoint.X - topLeftPoint.X), (int)(bottomRightPoint.Y - topLeftPoint.Y));

            for (int y = 0; y < bottomRightPoint.Y - topLeftPoint.Y; y++)
            {
                for (int x = 0; x < bottomRightPoint.X - topLeftPoint.X; x++)
                {

                    result.Data[y, x, 0] = inputImage.Data[y + (int)topLeftPoint.Y, x + (int)topLeftPoint.X, 0];
                    result.Data[y, x, 1] = inputImage.Data[y + (int)topLeftPoint.Y, x + (int)topLeftPoint.X, 1];
                    result.Data[y, x, 2] = inputImage.Data[y + (int)topLeftPoint.Y, x + (int)topLeftPoint.X, 2];
                }
            }

            return result;
        }

        #endregion
    }
}