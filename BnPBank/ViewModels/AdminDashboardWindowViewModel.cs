using BnPBank.Services;
using ReactiveUI;

namespace BnPBank.ViewModels
{
    public class AdminDashboardWindowViewModel : ReactiveObject
    {
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authenticationService;

        public AdminDashboardWindowViewModel(INavigationService navigationService, AuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            // Other initialization tasks go here
        }

        // Add properties and methods for your admin dashboard here
    }
}