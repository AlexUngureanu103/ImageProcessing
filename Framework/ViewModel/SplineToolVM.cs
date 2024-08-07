﻿using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Converters;
using Framework.Model;
using Framework.ViewModel.Commands;
using System.Windows.Input;
using System.Windows.Media;

namespace Framework.ViewModel
{
    public class SplineToolVM : BaseVM
    {
        public SplineToolsMenuCommands SplineToolsMenuCommands { get; set; }

        public SplineToolVM()
        {
            SplineToolsMenuCommands = new SplineToolsMenuCommands(this);

            OriginalCanvasHeight = 600;
            OriginalCanvasWidth = 800;

            ProcessedCanvasHeight = 600;
            ProcessedCanvasWidth = 800;

            SplineToolScaleValue = 1;

            Image<Bgr, byte> image = new Image<Bgr, byte>(800, 600, new Bgr(255, 255, 255));

            Graph = ImageConverter.Convert(image);
            ProcessedGraph = ImageConverter.Convert(image);
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
        public double SplineToolScaleValue
        {
            get => _scaleValue;
            set
            {
                _scaleValue = value;
                NotifyPropertyChanged(nameof(SplineToolScaleValue));
            }
        }

        public MainVM MainVM { get; set; }

        private ImageSource _initialGraph;
        public ImageSource Graph
        {
            get
            {
                return _initialGraph;
            }
            set
            {
                _initialGraph = value;

                if (_initialGraph != null)
                {
                    OriginalCanvasWidth = Graph.Width * SplineToolScaleValue;
                    OriginalCanvasHeight = Graph.Height * SplineToolScaleValue;
                }
                else
                {
                    OriginalCanvasWidth = 0;
                    OriginalCanvasHeight = 0;
                }

                NotifyPropertyChanged(nameof(Graph));
            }
        }

        private ImageSource _processedGraph;
        public ImageSource ProcessedGraph
        {
            get
            {
                return _processedGraph;
            }
            set
            {
                _processedGraph = value;
                if (_processedGraph != null)
                {
                    ProcessedCanvasWidth = ProcessedGraph.Width * SplineToolScaleValue;
                    ProcessedCanvasHeight = ProcessedGraph.Height * SplineToolScaleValue;
                }
                else
                {
                    ProcessedCanvasWidth = 0;
                    ProcessedCanvasHeight = 0;
                }
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

        #region Reset zoom
        private ICommand _resetZoomCommand;
        public ICommand ResetZoomCommand
        {
            get
            {
                if (_resetZoomCommand == null)
                    _resetZoomCommand = new RelayCommand(ResetZoom);

                return _resetZoomCommand;
            }
        }

        public void ResetZoom(object parameter)
        {
            SplineToolScaleValue = 1;
        }
        #endregion

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

        public double _processedCanvasWidth;
        public double ProcessedCanvasWidth
        {
            get => _processedCanvasWidth;
            set
            {
                _processedCanvasWidth = value;
                NotifyPropertyChanged(nameof(ProcessedCanvasWidth));
            }
        }

        public double _processedCanvasHeight;
        public double ProcessedCanvasHeight
        {
            get => _processedCanvasHeight;
            set
            {
                _processedCanvasHeight = value;
                NotifyPropertyChanged(nameof(ProcessedCanvasHeight));
            }
        }
        #endregion
    }
}
