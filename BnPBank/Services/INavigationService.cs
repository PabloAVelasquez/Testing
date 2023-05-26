using System;

namespace BnPBank.Services
{
    public interface INavigationService
    {
        void NavigateToMainWindow(string userName);
        void NavigateToUserRegistrationWindow();
        void NavigateToLoginWindow();
        void NavigateToAdminDashboard();
        void NavigateToAccountSettings();
        // Add other navigation methods as needed
    }
}