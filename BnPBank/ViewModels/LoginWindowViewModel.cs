using Avalonia.Controls;
using BnPBank.Data;
using BnPBank.Views;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BnPBank.Services;

namespace BnPBank.ViewModels
{
    public class LoginWindowViewModel : ReactiveObject
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authenticationService;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        public LoginWindowViewModel(INavigationService navigationService, AuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);
            RegisterCommand = ReactiveCommand.Create(OpenUserRegistrationWindow);
        }

        private async Task LoginAsync()
        {
            // Authenticate the user with the given username and password
            var user = await _authenticationService.AuthenticateUserAsync(Username, Password);

            // If authentication was unsuccessful, return
            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            Console.WriteLine("Login successful!");

            // Navigate to the main application window for regular users,
            // and to the admin dashboard for admins
            if (user.IsAdmin)
            {
                _navigationService.NavigateToAdminDashboard();
            }
            else
            {
                _navigationService.NavigateToMainWindow(user.Username);
            }
        }

        private void OpenUserRegistrationWindow()
        {
            _authenticationService.LogoutUser();
            _navigationService.NavigateToUserRegistrationWindow();
        }
    }
}