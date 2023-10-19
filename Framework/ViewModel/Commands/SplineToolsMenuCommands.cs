using Framework.Utilities;
using Framework.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Framework.ViewModel.Commands
{
    public class SplineToolsMenuCommands : BaseVM
    {
        private readonly SplineToolVM _splineToolVM;

        public SplineToolsMenuCommands(SplineToolVM splineToolVM)
        {
            _splineToolVM = splineToolVM;
        }

        private ImageSource Graph
        {
            get => _splineToolVM.Graph;
            set => _splineToolVM.Graph = value;
        }

        private double ScaleValue
        {
            get => _splineToolVM.ScaleValue;
            set => _splineToolVM.ScaleValue = value;
        }

        #region Magnifier

        private ICommand _drawHermitCurveCommand;
        public ICommand DrawHermitCurveCommand
        {
            get
            {
                if (_drawHermitCurveCommand == null)
                    _drawHermitCurveCommand = new RelayCommand(DrawHermitCurve);
                return _drawHermitCurveCommand;
            }
        }

        private void DrawHermitCurve(object parameter)
        {
            if (DataProvider.VectorOfMousePosition.Count < 3)
            {
                MessageBox.Show("Please select at least 3 first.");
                return;
            }
        }
        #endregion
        #region Magnifier

        private ICommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                    _resetCommand = new RelayCommand(Reset);
                return _resetCommand;
            }
        }

        private void Reset(object parameter)
        {
            DrawingHelper.RemoveUiElements(parameter as Canvas);
            DataProvider.VectorOfMousePosition.Clear();
        }
        #endregion

    }
}
