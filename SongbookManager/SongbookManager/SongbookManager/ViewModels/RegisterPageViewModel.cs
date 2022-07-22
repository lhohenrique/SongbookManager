using SongbookManager.Resx;
using SongbookManager.Services;
using SongbookManager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class RegisterPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UserService userService;

        #region [Properties]
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

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

        private string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ConfirmPassword"));
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

        private string nameErrorMessage = string.Empty;
        public string NameErrorMessage
        {
            get { return nameErrorMessage; }
            set
            {
                nameErrorMessage = value;
                PropertyChanged(this, new PropertyChangedEventArgs("NameErrorMessage"));
            }
        }

        private string emailErrorMessage = string.Empty;
        public string EmailErrorMessage
        {
            get { return emailErrorMessage; }
            set
            {
                emailErrorMessage = value;
                PropertyChanged(this, new PropertyChangedEventArgs("EmailErrorMessage"));
            }
        }

        private string passwordErrorMessage = string.Empty;
        public string PasswordErrorMessage
        {
            get { return passwordErrorMessage; }
            set
            {
                passwordErrorMessage = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PasswordErrorMessage"));
            }
        }
        #endregion

        #region [Commands]
        public Command RegisterCommand { get; set; }
        #endregion

        public RegisterPageViewModel()
        {
            userService = new UserService();

            RegisterCommand = new Command(async () => await RegisterActionAsync());
        }

        #region [Action]
        private async Task RegisterActionAsync()
        {
            bool infoValid = false;

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                infoValid = CheckInformations(Name, Email, Password, ConfirmPassword);

                if (infoValid)
                {
                    // Old DataBase structure
                    //User newUser = new User()
                    //{
                    //    Name = this.Name,
                    //    Email = this.Email,
                    //    Password = this.Password
                    //};

                    //await App.Database.RegisterUser(newUser);

                    Result = await userService.RegisterUSer(Name, Email, Password);

                    if (Result)
                    {
                        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UserAlreadyRegistered, AppResources.Ok);
                    }
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnableRegisterUser, AppResources.Ok);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region [Private Methods]
        private bool CheckInformations(string nameToValidate, string emailToValidate, string passwordToValidate, string confirmPasswordToValidate)
        {
            var nameValid = ValidateName(nameToValidate);
            var emailValid = ValidateEmail(emailToValidate);
            var passwordValid = ValidatePassword(passwordToValidate, confirmPasswordToValidate);

            return nameValid && emailValid && passwordValid;
        }

        private bool ValidateName(string nameToValidate)
        {
            bool isValid = false;

            if (String.IsNullOrWhiteSpace(nameToValidate))
            {
                NameErrorMessage = AppResources.GiveAName;
            }
            else
            {
                NameErrorMessage = string.Empty;
                isValid = true;
            }

            return isValid;
        }

        private bool ValidateEmail(string emailToValidate)
        {
            bool isValid = false;

            var emailPattern = "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$";

            if (String.IsNullOrWhiteSpace(emailToValidate) || !(Regex.IsMatch(emailToValidate, emailPattern)))
            {
                EmailErrorMessage = AppResources.InvalidEmail;
            }
            else
            {
                EmailErrorMessage = string.Empty;
                isValid = true;
            }

            return isValid;
        }

        private bool ValidatePassword(string passwordToValidade, string confirmPasswordToValidate)
        {
            bool isValid = false;

            if (String.IsNullOrWhiteSpace(passwordToValidade) || passwordToValidade.Length < 6)
            {
                PasswordErrorMessage = AppResources.MinimumCharacterMessage;
            }
            else if (!passwordToValidade.Equals(confirmPasswordToValidate))
            {
                PasswordErrorMessage = AppResources.PasswordsNotMatch;
            }
            else
            {
                PasswordErrorMessage = string.Empty;
                isValid = true;
            }

            return isValid;
        }
        #endregion
    }
}
