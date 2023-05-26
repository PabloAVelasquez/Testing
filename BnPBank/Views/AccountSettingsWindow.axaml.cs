using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BnPBank.ViewModels;
using BnPBank.Services;
using Avalonia;

namespace BnPBank.Views
{
    public partial class AccountSettingsWindow : Window
    {
        public AccountSettingsWindow()
        {
            InitializeComponent();

            var authenticationService = AvaloniaLocator.Current.GetService<AuthenticationService>();
            var navigationService = AvaloniaLocator.Current.GetService<INavigationService>();

            DataContext = new AccountSettingsWindowViewModel(navigationService, authenticationService);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}