using Framework.Model;
using Framework.Utilities;
using Framework.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Framework.ViewModel
{
    public class SliderDialogBoxVM : BaseVM
    {
        public void CreateParameters(List<string> texts)
        {
            Height = (texts.Count + 3) * 40;

            foreach (var text in texts)
            {
                Parameters.Add(new SliderDialogBoxParameter()
                {
                    ParamText = text,
                    Foreground = Theme.TextForeground,
                    Height = 20,
                    MinValue = 1,
                    MaxValue = 254,
                    SliderValue = "20",
                });
            }
        }

        public List<int> GetValues()
        {
            var values = new List<int>();

            foreach (var parameter in Parameters)
            {
                string text = parameter.SliderValue;
                if (text == null || text.Trim().Length == 0 || IsNumeric(text) == false)
                    values.Add(0);
                else
                {
                    var value = double.Parse(text);
                    values.Add((int)Math.Round(value));
                }

            }

            return values;
        }

        private bool IsNumeric(string text) => double.TryParse(text, out _);

        #region Properties and commands
        public double Height { get; set; }

        public IThemeMode Theme { get; set; } =
            LimeGreenTheme.Instance;

        public ObservableCollection<SliderDialogBoxParameter> Parameters { get; } =
            new ObservableCollection<SliderDialogBoxParameter>();

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(p =>
                    {
                        DataProvider.CloseWindow<SliderDialogBox>();
                    });

                return _closeCommand;
            }
        }
        #endregion
    }
}
