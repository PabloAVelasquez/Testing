using Avalonia.Controls;
using BnPBank.Views;
using Avalonia.Controls.ApplicationLifetimes;
using BnPBank.ViewModels;
using BnPBank.Services;
using Avalonia;

namespace BnPBank.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IClassicDesktopStyleApplicationLifetime _appLifetime;
        private readonly AuthenticationService _authenticationService;

        // Keep a single instance of MainWindow.
        private MainWindow _mainWindow;

        public NavigationService(IClassicDesktopStyleApplicationLifetime appLifetime, AuthenticationService authenticationService)
        {
            _appLifetime = appLifetime;
            _authenticationService = authenticationService;
            _mainWindow = new MainWindow();
        }

        public void NavigateToMainWindow(string userName)
        {
            _mainWindow.DataContext = new MainWindowViewModel(this, _authenticationService, userName);
            _mainWindow.Show();
            (_appLifetime.MainWindow as Window)?.Close();
            _appLifetime.MainWindow = _mainWindow;
        }

        public void NavigateToLoginWindow()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            (_appLifetime.MainWindow as Window)?.Close();
            _appLifetime.MainWindow = loginWindow;
        }

        public void NavigateToUserRegistrationWindow()
        {
            var userRegistrationWindow = new UserRegistrationWindow();
            userRegistrationWindow.Show();
            (_appLifetime.MainWindow as Window)?.Close();
            _appLifetime.MainWindow = userRegistrationWindow;
        }

        public void NavigateToAdminDashboard()
        {
            var adminDashboardWindow = new AdminDashboardWindow();
            adminDashboardWindow.Show();
            (_appLifetime.MainWindow as Window)?.Close();
            _appLifetime.MainWindow = adminDashboardWindow;
        }

        public void NavigateToAccountSettings()
        {
            var accountSettingsWindow = new AccountSettingsWindow
            {
                DataContext = AvaloniaLocator.Current.GetService<AccountSettingsWindowViewModel>()
            };

            accountSettingsWindow.Show();
            _appLifetime.MainWindow.Close();
            _appLifetime.MainWindow = accountSettingsWindow;
        }
    }
}