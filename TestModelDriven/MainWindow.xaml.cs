using TestModelDriven.ViewModels;

namespace TestModelDriven
{
    public partial class MainWindow
    {
        public TestApplicationViewModel Application { get; }

        public MainWindow()
        {
            Application = new TestApplicationViewModel();

            DataContext = this;
            InitializeComponent();
        }
    }
}
