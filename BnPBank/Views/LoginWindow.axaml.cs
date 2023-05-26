using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BnPBank.Services;
using BnPBank.ViewModels;
using Avalonia.Diagnostics;
using BnPBank.Data;
using System;
using System.Linq;
using BnPBank.Models;
using Microsoft.EntityFrameworkCore;


namespace BnPBank.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var sessionTokenManager = new SessionTokenManager();
            var authenticationService = new AuthenticationService(sessionTokenManager);
            DataContext = new LoginWindowViewModel(new NavigationService(App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime, authenticationService), authenticationService);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

}