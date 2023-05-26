using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using BnPBank.Data;
using BnPBank.ViewModels;
using BnPBank.Views;
using BnPBank.Services;

namespace BnPBank
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            AvaloniaLocator.CurrentMutable
                .GetService<IAssetLoader>()
                .SetDefaultAssembly(typeof(App).Assembly);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Initialize the database context
                using (var db = new BankingDbContext())
                {
                    db.Database.EnsureCreated();
                }

                // Create the authentication service
                var authenticationService = new AuthenticationService(new SessionTokenManager());

                // Create the navigation service
                var navigationService = new NavigationService(desktop, authenticationService);

                // Register them in the Avalonia services
                AvaloniaLocator.CurrentMutable.Bind<AuthenticationService>().ToConstant(authenticationService);
                AvaloniaLocator.CurrentMutable.Bind<INavigationService>().ToConstant(navigationService);

                // Set the main window to the login window and assign its view model
                var loginWindow = new LoginWindow();
                //var loginWindow = new AccountSettingsWindow(); //TEMP for testing
                loginWindow.Show();
                desktop.MainWindow = loginWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}