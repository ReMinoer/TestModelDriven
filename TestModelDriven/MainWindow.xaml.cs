using TestModelDriven.ViewModels;

namespace TestModelDriven
{
    public partial class MainWindow
    {
        public ApplicationViewModel Application { get; }

        public MainWindow()
        {
            Application = new ApplicationViewModel();

            DataContext = this;
            InitializeComponent();
        }
    }
}
