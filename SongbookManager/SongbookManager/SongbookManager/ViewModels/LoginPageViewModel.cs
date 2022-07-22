using SongbookManager.Helpers;
using SongbookManager.Models;
using SongbookManager.Resx;
using SongbookManager.Services;
using SongbookManager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region [Properties]
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }

        private bool result;
        public bool Result
        {
            get { return this.isBusy; }
            set
            {
                this.isBusy = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return this.result; }
            set
            {
                this.result = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }
        #endregion

        #region [Commands]
        public Command LoginCommand { get; set; }
        public Command SignUpCommand { get; set; }
        public Command ForgotPasswordCommand { get; set; }
        #endregion

        public LoginPageViewModel()
        {
            LoginCommand = new Command(async () => await LoginActionAsync());
            SignUpCommand = new Command(async () => await SignUpAction());
            ForgotPasswordCommand = new Command(async () => await ForgotPasswordAction());
        }

        #region [Actions]
        private async Task SignUpAction()
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
        }

        private async Task LoginActionAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                // Old DataBase structure
                //User userLogged = await App.Database.LoginUser(Email, Password);

                var userService = new UserService();
                Result = await userService.LoginUser(Email, Password);

                if (Result)
                {
                    Preferences.Set("Email", Email);
                    await Shell.Current.GoToAsync($"//{nameof(MusicPage)}");
                    
                    // Old navigation
                    //await Application.Current.MainPage.Navigation.PushAsync(new MusicPage());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(AppResources.Invalid, AppResources.InvalidEmailPassword, AppResources.Ok);
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotSignIn, AppResources.Ok);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ForgotPasswordAction()
        {
            await Shell.Current.GoToAsync($"{nameof(ForgotPasswordPage)}");
        }
        #endregion

        #region [Public Methods]
        public async void OnAppearingAsync()
        {
            var loggedUserEmail = Preferences.Get("Email", string.Empty);
            if (string.IsNullOrEmpty(loggedUserEmail))
            {
                Email = string.Empty;
                Password = string.Empty;
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(MusicPage)}");
            }
        }
        #endregion
    }
}
