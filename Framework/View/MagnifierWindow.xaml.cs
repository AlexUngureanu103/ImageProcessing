using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Utilities;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using ImageConverter = Framework.Converters.ImageConverter;
using Point = System.Windows.Point;

namespace Framework.View
{
    public partial class MagnifierWindow : Window
    {
        private Point LastPoint { get; set; }

        public MagnifierWindow()
        {
            InitializeComponent();

            DataProvider.MagnifierOn = true;

            Application.Current.Windows.OfType<MainWindow>().First().Update();
            Update();
        }

        public void Update()
        {
            if (LastPoint != DataProvider.LastPosition)
            {
                DisplayGray();
                DisplayColor();

                LastPoint = DataProvider.LastPosition;
            }
        }

        private void DisplayColor()
        {
            if (DataProvider.ColorInitialImage != null)
                imageBoxOriginal.Source = GetImage(DataProvider.ColorInitialImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
            if (DataProvider.ColorProcessedImage != null)
                imageBoxProcessed.Source = GetImage(DataProvider.ColorProcessedImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
        }

        private void DisplayGray()
        {
            if (DataProvider.GrayInitialImage != null)
                imageBoxOriginal.Source = GetImage(DataProvider.GrayInitialImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
            if (DataProvider.GrayProcessedImage != null)
                imageBoxProcessed.Source = GetImage(DataProvider.GrayProcessedImage, (int)imageBoxOriginal.Width, (int)imageBoxOriginal.Height);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataProvider.MagnifierOn = false;
            Application.Current.Windows.OfType<MainWindow>().First().Update();
        }

        private ImageSource GetImage(Image<Bgr, byte> image, int width, int height)
        {
            Bitmap flag = new Bitmap(width, height);
            Graphics flagGraphics = Graphics.FromImage(flag);

            int i = 0;
            int widthStep = width / 9;
            int heightStep = height / 9;
            int yStep = 0;
            for (int x = -4; x <= 4; ++x)
            {
                int xStep = 0;
                int j = 0;
                for (int y = -4; y <= 4; ++y)
                {
                    if (DataProvider.LastPosition.Y + y >= 0 && DataProvider.LastPosition.X + x >= 0 &&
                        DataProvider.LastPosition.Y + y < image.Height && DataProvider.LastPosition.X + x < image.Width)
                    {
                        int blueColor = image.Data[(int)DataProvider.LastPosition.Y + y,
                            (int)DataProvider.LastPosition.X + x, 0];
                        int greenColor = image.Data[(int)DataProvider.LastPosition.Y + y,
                            (int)DataProvider.LastPosition.X + x, 1];
                        int redColor = image.Data[(int)DataProvider.LastPosition.Y + y,
                            (int)DataProvider.LastPosition.X + x, 2];

                        string text = redColor + "\n" + greenColor + "\n" + blueColor;
                        Font font = new Font(new FontFamily("Arial"), 7, System.Drawing.FontStyle.Bold, GraphicsUnit.Point);
                        Rectangle rectangle = new Rectangle(yStep, xStep, widthStep, heightStep);
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(redColor, greenColor, blueColor)), rectangle);
                        if (blueColor <= 128 & greenColor <= 128 & redColor <= 128)
                            flagGraphics.DrawString(text, font, Brushes.White, rectangle);
                        else
                            flagGraphics.DrawString(text, font, Brushes.Black, rectangle);
                    }
                    else
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)),
                            new Rectangle(yStep, xStep, widthStep, heightStep));
                    xStep += widthStep;
                    ++j;
                }
                yStep += heightStep;
                ++i;
            }

            return ImageConverter.Convert(flag);
        }

        private ImageSource GetImage(Image<Gray, byte> image, int width, int height)
        {
            Bitmap flag = new Bitmap(width, height);
            Graphics flagGraphics = Graphics.FromImage(flag);

            int i = 0;
            int widthStep = width / 9;
            int heightStep = height / 9;
            int yStep = 0;
            for (int x = -4; x <= 4; ++x)
            {
                int xStep = 0;
                int j = 0;
                for (int y = -4; y <= 4; ++y)
                {
                    if (DataProvider.LastPosition.Y + y >= 0 && DataProvider.LastPosition.X + x >= 0 &&
                        DataProvider.LastPosition.Y + y < image.Height && DataProvider.LastPosition.X + x < image.Width)
                    {
                        int grayColor = image.Data[(int)DataProvider.LastPosition.Y + y,
                            (int)DataProvider.LastPosition.X + x, 0];

                        string text = "\n" + grayColor + "\n";
                        Font font = new Font(new FontFamily("Arial"), 7, System.Drawing.FontStyle.Bold, GraphicsUnit.Point);
                        Rectangle rectangle = new Rectangle(yStep, xStep, widthStep, heightStep);
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(grayColor, grayColor, grayColor)),
                            rectangle);
                        if (grayColor <= 128)
                            flagGraphics.DrawString(text, font, Brushes.White, rectangle);
                        else
                            flagGraphics.DrawString(text, font, Brushes.Black, rectangle);
                    }
                    else
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)),
                            new Rectangle(yStep, xStep, widthStep, heightStep));
                    xStep += widthStep;
                    ++j;
                }
                yStep += heightStep;
                ++i;
            }

            return ImageConverter.Convert(flag);
        }
    }
}