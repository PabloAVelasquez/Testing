using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BnPBank.ViewModels;
using BnPBank.Services;
using Avalonia;

namespace BnPBank.Views
{
    public partial class UserRegistrationWindow : Window
    {
        // Public parameterless constructor for XAML parser
        public UserRegistrationWindow()
        {
            InitializeComponent();

            var authenticationService = AvaloniaLocator.Current.GetService<AuthenticationService>();
            var navigationService = AvaloniaLocator.Current.GetService<INavigationService>();

            DataContext = new UserRegistrationWindowViewModel(navigationService, authenticationService);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}