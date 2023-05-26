using BnPBank.Data;
using BnPBank.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using BnPBank.Views;
using System.Security.Cryptography;
using System.Threading.Channels;
using System.Text.RegularExpressions;
using BnPBank.Services;

namespace BnPBank.ViewModels
{
    public class UserRegistrationWindowViewModel : ReactiveObject
    {
        private readonly Window _window;
        private string _username;
        private string _password;
        private string _email;
        private string _firstName;
        private string _lastName;
        private string _confirmPassword;
        private string _confirmEmail;
        private byte[]? _profilePicture;
        private string _errorMessage;
        private bool _isFirstNameEmpty;
        private bool _isLastNameEmpty;
        private bool _isUsernameInvalid;
        private bool _isPasswordInvalid;
        private bool _isEmailInvalid;
        private bool _isValidationAttempted;
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authenticationService;

        public bool IsValidationAttempted
        {
            get => _isValidationAttempted;
            set => this.RaiseAndSetIfChanged(ref _isValidationAttempted, value);
        }

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

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }

        public string ConfirmEmail
        {
            get => _confirmEmail;
            set => this.RaiseAndSetIfChanged(ref _confirmEmail, value);
        }

        public byte[]? ProfilePicture
        {
            get => _profilePicture;
            set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
        }

        public bool IsFirstNameEmpty
        {
            get => _isFirstNameEmpty;
            set => this.RaiseAndSetIfChanged(ref _isFirstNameEmpty, value);
        }

        public bool IsLastNameEmpty
        {
            get => _isLastNameEmpty;
            set => this.RaiseAndSetIfChanged(ref _isLastNameEmpty, value);
        }

        public bool IsUsernameInvalid
        {
            get => _isUsernameInvalid;
            set => this.RaiseAndSetIfChanged(ref _isUsernameInvalid, value);
        }

        public bool IsPasswordInvalid
        {
            get => _isPasswordInvalid;
            set => this.RaiseAndSetIfChanged(ref _isPasswordInvalid, value);
        }

        public bool IsEmailInvalid
        {
            get => _isEmailInvalid;
            set => this.RaiseAndSetIfChanged(ref _isEmailInvalid, value);
        }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
        public ReactiveCommand<Unit, Unit> UploadCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenLoginWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public UserRegistrationWindowViewModel(INavigationService navigationService, AuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            RegisterCommand = ReactiveCommand.CreateFromTask(RegisterUserAsync);
            UploadCommand = ReactiveCommand.Create(UploadProfilePicture);
            OpenLoginWindowCommand = ReactiveCommand.Create(OpenLoginWindow);
            CancelCommand = ReactiveCommand.Create(Cancel);
            // Subscribe to property change events to update the validation properties
            this.WhenAnyValue(
                x => x.FirstName,
                x => x.LastName,
                x => x.Username,
                x => x.Password,
                x => x.Email)
                .Subscribe(_ =>
                {
                    UpdateValidationProperties();
                });
        }

        private void UpdateValidationProperties()
        {
            IsFirstNameEmpty = string.IsNullOrEmpty(FirstName);
            IsLastNameEmpty = string.IsNullOrEmpty(LastName);
            IsUsernameInvalid = string.IsNullOrEmpty(Username) || !Regex.IsMatch(Username, @"^[a-zA-Z0-9_]+$") || Username.Length < 3;
            IsPasswordInvalid = string.IsNullOrEmpty(Password) || !Regex.IsMatch(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
            IsEmailInvalid = string.IsNullOrEmpty(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private async Task RegisterUserAsync()
        {
            try
            {
                // Update the validation properties
                UpdateValidationProperties();

                // Set IsValidationAttempted to true for all fields
                IsValidationAttempted = true;

                // Check if any required fields are invalid
                if (IsFirstNameEmpty || IsLastNameEmpty || IsUsernameInvalid || IsPasswordInvalid || IsEmailInvalid)
                {
                    if (IsUsernameInvalid)
                        ErrorMessage = "Username is invalid. It should contain only alphanumeric characters and underscores, and must be at least 3 characters long.";
                    else if (IsEmailInvalid)
                        ErrorMessage = "Email is invalid. Please enter a valid email address.";
                    else if (IsPasswordInvalid)
                    {
                        ErrorMessage = "Password is invalid. It must meet the following criteria:"
                            + "\n- Contain at least 1 lowercase alphabetical character"
                            + "\n- Contain at least 1 uppercase alphabetical character"
                            + "\n- Contain at least 1 numeric character"
                            + "\n- Contain at least one special character"
                            + "\n- Be between 8 and 15 characters in length";
                    }
                    else
                        ErrorMessage = "Please correct the highlighted fields.";

                    return;
                }

                // Check if the password and confirm password match
                if (Password != ConfirmPassword)
                {
                    // Passwords do not match
                    // Set the error message property
                    ErrorMessage = "Passwords do not match.";
                    return;
                }

                // Check if the email and confirm email match
                if (Email != ConfirmEmail)
                {
                    // Emails do not match
                    // Set the error message property
                    ErrorMessage = "Emails do not match.";
                    return;
                }

                ErrorMessage = null;

                // Hash the password with BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

                using (var dbContext = new BankingDbContext())
                {
                    // Check if the username or email already exists
                    var existingUser = await dbContext.Users
                        .FirstOrDefaultAsync(u => u.Username == Username || u.Email == Email);
                    if (existingUser != null)
                    {
                        ErrorMessage = "The username or email is already taken.";
                        return;
                    }

                    // Create a new user object
                    var user = new User
                    {
                        Username = Username,
                        Email = Email,
                        FirstName = FirstName,
                        LastName = LastName,
                        ProfilePicture = ProfilePicture ?? GetDefaultProfilePicture(), // Directly assign byte array
                        HashedPassword = hashedPassword,
                    };

                    // Add the user to the database
                    dbContext.Users.Add(user);
                    await dbContext.SaveChangesAsync();

                    // Provide feedback to the user
                    Console.WriteLine($"User {user.Username} registered successfully!");

                    // Raise the event to notify the UI
                    _navigationService.NavigateToMainWindow(user.Username);
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details or display a message to the user
                ErrorMessage = "An error occurred while registering the user: " + ex.Message;

                // If you want more detailed information, you can inspect ex.InnerException
                if (ex.InnerException != null)
                {
                    ErrorMessage += "\nInner Exception: " + ex.InnerException.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while registering the user: " + ex.Message;
            }
        }

        private byte[] GetDefaultProfilePicture()
        {
            var defaultImagePath = "Assets/defaultProfilePicture.png"; // Adjust this path to your needs.
            return File.ReadAllBytes(defaultImagePath);
        }

        private async void UploadProfilePicture()
        {
            // Open a file dialog to select an image file
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Image files", Extensions = { "jpg", "jpeg", "png", "gif" } });
            var result = await dialog.ShowAsync((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

            if (result != null && result.Length > 0)
            {
                try
                {
                    // Read the selected image file
                    byte[] imageBytes = File.ReadAllBytes(result[0]);

                    // Set the profile picture property
                    ProfilePicture = imageBytes;
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Failed to load the profile picture: " + ex.Message;
                }
            }
        }

        private void Cancel()
        {
            // Navigate back to the login window
            _navigationService.NavigateToLoginWindow();
        }

        private void OpenLoginWindow()
        {
            _navigationService.NavigateToLoginWindow();
        }
    }
}