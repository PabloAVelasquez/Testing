using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BnPBank.Views;

namespace BnPBank.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}