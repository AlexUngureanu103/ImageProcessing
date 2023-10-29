using Framework.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Framework.View
{
    public partial class GraphWindow : Window
    {
        private readonly GraphVM _graphVM;

        public GraphWindow(MainVM mainVM, PointCollection points)
        {
            InitializeComponent();

            _graphVM = new GraphVM();

            _graphVM.Theme = mainVM.Theme;
            _graphVM.CreateGraph(points.Clone());

            DataContext = _graphVM;
        }
    }
}