using Framework.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Framework.View
{
    public partial class DialogBox : Window
    {
        private readonly DialogBoxVM _dialogBoxVM;

        public DialogBox(MainVM mainVM, List<string> parameters)
        {
            InitializeComponent();

            _dialogBoxVM = new DialogBoxVM();

            _dialogBoxVM.Theme = mainVM.Theme;
            _dialogBoxVM.CreateParameters(parameters);

            DataContext = _dialogBoxVM;
        }

        public List<double> GetValues()
        {
            return _dialogBoxVM.GetValues();
        }
    }
}