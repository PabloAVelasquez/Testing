using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BnPBank.Views
{
    public partial class AdminDashboardWindow : Window
    {
        public AdminDashboardWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}