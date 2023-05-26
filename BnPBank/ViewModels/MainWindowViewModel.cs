using System.Reactive;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BnPBank.Views;
using ReactiveUI;
using BnPBank.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BnPBank.Services;
using System;

namespace BnPBank.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _greeting;
        private string _welcomeText;
        private byte[] _profilePicture;
        private string _userName;
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authenticationService;

        public string Greeting
        {
            get => _greeting;
            set => this.RaiseAndSetIfChanged(ref _greeting, value);
        }

        public string WelcomeText
        {
            get => _welcomeText;
            set => this.RaiseAndSetIfChanged(ref _welcomeText, value);
        }

        public byte[] ProfilePicture
        {
            get => _profilePicture;
            set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
        }

        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        public ReactiveCommand<Unit, Unit> AccountsViewCommand { get; }
        public ReactiveCommand<Unit, Unit> TransactionsViewCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateAccountViewCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateTransactionViewCommand { get; }
        public ReactiveCommand<Unit, Unit> AccountSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> LogOutCommand { get; }

        public MainWindowViewModel(INavigationService navigationService, AuthenticationService authenticationService, string loggedInUserName)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            WelcomeText = "Your trusted partner for all your banking needs. Securely manage your finances anytime, anywhere.";

            //AccountsViewCommand = ReactiveCommand.Create(_navigationService.NavigateToAccountsView);
            //TransactionsViewCommand = ReactiveCommand.Create(_navigationService.NavigateToTransactionsView);
            //CreateAccountViewCommand = ReactiveCommand.Create(_navigationService.NavigateToCreateAccountView);
            //CreateTransactionViewCommand = ReactiveCommand.Create(_navigationService.NavigateToCreateTransactionView);
            AccountSettingsCommand = ReactiveCommand.Create(_navigationService.NavigateToAccountSettings);
            LogOutCommand = ReactiveCommand.Create(LogOut);

            LoadUserDetails(loggedInUserName);
        }

        private void LoadUserDetails(string userName)
        {
            using (var dbContext = new BankingDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Username == userName);

                if (user != null)
                {
                    UserName = user.FirstName.Substring(0, 1).ToUpper() + user.FirstName.Substring(1).ToLower();
                    Greeting = $"Welcome, {UserName}!";
                    ProfilePicture = user.ProfilePicture;
                }
            }
        }

        private void LogOut()
        {
            _authenticationService.LogoutUser();
            _navigationService.NavigateToLoginWindow();
        }
    }
}