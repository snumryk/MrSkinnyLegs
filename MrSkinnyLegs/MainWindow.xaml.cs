using System.Windows;
using WebSpider;

namespace MrSkinnyLegs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _webAdress;
        private string _filePath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _webAdress = WebAdressField.Text;
            _filePath = FilePathField.Text;
            await Parser.GetBook(_webAdress, _filePath);
        }
    }
}
