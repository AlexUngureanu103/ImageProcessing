using Framework.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Framework.View
{
    /// <summary>
    /// Interaction logic for SliderDialogBox.xaml
    /// </summary>
    public partial class SliderDialogBox : Window
    {
        private readonly SliderDialogBoxVM _dialogBoxVM;

        public SliderDialogBox(MainVM mainVM, List<string> parameters)
        {
            InitializeComponent();

            _dialogBoxVM = new SliderDialogBoxVM();

            _dialogBoxVM.Theme = mainVM.Theme;
            _dialogBoxVM.CreateParameters(parameters);

            DataContext = _dialogBoxVM;
        }

        public List<int> GetValues() => _dialogBoxVM.GetValues();
    }
}
