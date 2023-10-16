using Emgu.CV.Structure;
using Emgu.CV;
using Framework.Model;
using System.Windows.Media;
using static Framework.Converters.ImageConverter;

namespace Framework.ViewModel
{
    public class SplineTollVM : BaseVM
    {
        public SplineTollVM()
        {      
            OriginalCanvasHeight = 300;
            OriginalCanvasWidth = 500;
            ScaleValue = 1;
            

            Graph = Convert(new Image<Gray, byte>(500, 300, new Gray(128)));

            string theme = Properties.Settings.Default.Theme;
            SetThemeMode(theme);
        }

        public void SetThemeMode(string theme)
        {
            switch (theme)
            {
                case "Lime Green":
                    Theme = LimeGreenTheme.Instance;
                    break;

                case "Forest Green":
                    Theme = ForestGreenTheme.Instance;
                    break;

                case "Pale Pink":
                    Theme = PalePinkTheme.Instance;
                    break;

                case "Lavender Violet":
                    Theme = LavenderVioletTheme.Instance;
                    break;

                case "Cobalt Blue":
                    Theme = CobaltBlueTheme.Instance;
                    break;
            }
        }

        private IThemeMode _theme;
        public IThemeMode Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                NotifyPropertyChanged(nameof(Theme));
            }
        }

        private double _scaleValue;
        public double ScaleValue
        {
            get => _scaleValue;
            set
            {
                _scaleValue = value;
                NotifyPropertyChanged(nameof(ScaleValue));
            }
        }

        public ImageSource _graph;
        public ImageSource Graph
        {
            get
            {
                return _graph;
            }
            set
            {
                _graph = value;

                if (_graph != null)
                {
                    OriginalCanvasWidth = Graph.Width * ScaleValue;
                    OriginalCanvasHeight = Graph.Height * ScaleValue;
                }
                else
                {
                    OriginalCanvasWidth = 0;
                    OriginalCanvasHeight = 0;
                }

                NotifyPropertyChanged(nameof(Graph));
            }
        }

        private string _xPos;
        public string XPos
        {
            get => _xPos;
            set
            {
                _xPos = value;
                NotifyPropertyChanged(nameof(XPos));
            }
        }

        private string _yPos;
        public string YPos
        {
            get => _yPos;
            set
            {
                _yPos = value;
                NotifyPropertyChanged(nameof(YPos));
            }
        }

        private string _grayValue;
        public string GrayValue
        {
            get => _grayValue;
            set
            {
                _grayValue = value;
                NotifyPropertyChanged(nameof(GrayValue));
            }
        }

        private string _redValue;
        public string RedValue
        {
            get => _redValue;
            set
            {
                _redValue = value;
                NotifyPropertyChanged(nameof(RedValue));
            }
        }

        private string _greenValue;
        public string GreenValue
        {
            get => _greenValue;
            set
            {
                _greenValue = value;
                NotifyPropertyChanged(nameof(GreenValue));
            }
        }

        private string _blueValue;
        public string BlueValue
        {
            get => _blueValue;
            set
            {
                _blueValue = value;
                NotifyPropertyChanged(nameof(BlueValue));
            }
        }
        
        #region Canvases properties
        public double _originalCanvasWidth;
        public double OriginalCanvasWidth
        {
            get => _originalCanvasWidth;
            set
            {
                _originalCanvasWidth = value;
                NotifyPropertyChanged(nameof(OriginalCanvasWidth));
            }
        }

        public double _originalCanvasHeight;
        public double OriginalCanvasHeight
        {
            get => _originalCanvasHeight;
            set
            {
                _originalCanvasHeight = value;
                NotifyPropertyChanged(nameof(OriginalCanvasHeight));
            }
        }

        //public double _processedCanvasWidth;
        //public double ProcessedCanvasWidth
        //{
        //    get => _processedCanvasWidth;
        //    set
        //    {
        //        _processedCanvasWidth = value;
        //        NotifyPropertyChanged(nameof(ProcessedCanvasWidth));
        //    }
        //}

        //public double _processedCanvasHeight;
        //public double ProcessedCanvasHeight
        //{
        //    get => _processedCanvasHeight;
        //    set
        //    {
        //        _processedCanvasHeight = value;
        //        NotifyPropertyChanged(nameof(ProcessedCanvasHeight));
        //    }
        //}
        #endregion
    }
}
