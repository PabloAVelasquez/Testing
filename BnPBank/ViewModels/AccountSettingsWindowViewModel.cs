using BnPBank.Data;
using BnPBank.Models;
using ReactiveUI;
using System;
using System.Reactive;
using Avalonia;
using BnPBank.Services;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BnPBank.ViewModels
{
    public class AccountSettingsWindowViewModel : ReactiveObject
    {
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authenticationService;
        private readonly SessionTokenManager _sessionTokenManager;

        private string _currentPassword;
        private string _newPassword;
        private string _confirmNewPassword;
        private string _errorMessage;
        private byte[] _profilePicture;

        public byte[] ProfilePicture
        {
            get => _profilePicture;
            set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set => this.RaiseAndSetIfChanged(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => this.RaiseAndSetIfChanged(ref _newPassword, value);
        }

        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmNewPassword, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ReactiveCommand<Unit, Unit> UploadCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }


        public AccountSettingsWindowViewModel(INavigationService navigationService, AuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;

            UploadCommand = ReactiveCommand.Create(UploadProfilePicture);
            SaveChangesCommand = ReactiveCommand.Create(SaveChanges);
            DeleteAccountCommand = ReactiveCommand.Create(DeleteAccount);
            CancelCommand = ReactiveCommand.Create(Cancel);
            LoadUserProfilePicture();

            InitializeAsync().ConfigureAwait(false);
        }

        private async Task InitializeAsync()
        {
            await LoadUserProfilePicture();
        }

        private async Task LoadUserProfilePicture()
        {
            // Retrieve user's username from SessionTokenManager
            var username = _sessionTokenManager.AuthenticatedUsername;

            if (!string.IsNullOrWhiteSpace(username))
            {
                // Retrieve user's data from the database
                using (var dbContext = new BankingDbContext())
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

                    if (user != null)
                    {
                        // Check if the user has a profile picture
                        if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                        {
                            ProfilePicture = user.ProfilePicture;
                        }
                        else
                        {
                            // Load default profile picture
                            ProfilePicture = GetDefaultProfilePicture();
                        }
                    }
                    else
                    {
                        // Handle scenario when user is not found in the database
                        // For example, load default profile picture
                        ProfilePicture = GetDefaultProfilePicture();
                    }
                }
            }
            else
            {
                // Handle scenario when no user is authenticated
                // For example, load default profile picture
                ProfilePicture = GetDefaultProfilePicture();
            }
        }

        private byte[] GetDefaultProfilePicture()
        {
            var defaultImagePath = "Assets/defaultProfilePicture.png";
            return File.ReadAllBytes(defaultImagePath);
        }

        private async void UploadProfilePicture()
        {
            // Same logic from UserRegistrationWindowViewModel
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Image files", Extensions = { "jpg", "jpeg", "png", "gif" } });
            var result = await dialog.ShowAsync((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

            if (result != null && result.Length > 0)
            {
                try
                {
                    byte[] imageBytes = await File.ReadAllBytesAsync(result[0]);
                    ProfilePicture = imageBytes;

                    // TODO: Save changes to user's profile picture in database
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to load the profile picture: " + ex.Message;
                }
            }
        }

        private void SaveChanges()
        {
            // TODO: Implement SaveChanges
        }

        private void DeleteAccount()
        {
            // TODO: Implement DeleteAccount
        }

        private void Cancel()
        {
            // Navigate back to the MainWindow
            _navigationService.NavigateToMainWindow(_sessionTokenManager.AuthenticatedUsername);
        }
    }
}