using SongbookManager.Helpers;
using SongbookManager.Resx;
using SongbookManager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class ChangePasswordPageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private UserService userService;

        #region [Properties]
        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set
            {
                newPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("NewPassword"));
            }
        }

        private string confirmNewPassword;
        public string ConfirmNewPassword
        {
            get => confirmNewPassword;
            set
            {
                confirmNewPassword = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ConfirmNewPassword"));
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
        public Command ChangePasswordCommand { get; set; }
        #endregion

        public ChangePasswordPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            userService = new UserService();

            ChangePasswordCommand = new Command(async () => await ChangePasswordAction());
        }

        #region [Actions]
        public async Task ChangePasswordAction()
        {
            if (ValidatePassword(NewPassword, ConfirmNewPassword))
            {
                try
                {
                    var user = LoggedUserHelper.LoggedUser;
                    user.Password = NewPassword;
                    await userService.UpdateUser(user);

                    await Application.Current.MainPage.DisplayAlert(AppResources.PasswordChanged, string.Empty, AppResources.Ok);

                    await Navigation.PopAsync();
                }
                catch (Exception)
                {
                    await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotChangePassword, AppResources.Ok);
                }
            }
        }
        #endregion

        #region [Private Methods]
        private bool ValidatePassword(string passwordToValidade, string confirmPasswordToValidate)
        {
            bool isValid = false;

            if (!String.IsNullOrWhiteSpace(passwordToValidade) && passwordToValidade.Equals(confirmPasswordToValidate))
            {
                PasswordErrorMessage = string.Empty;
                isValid = true;
            }
            else
            {
                PasswordErrorMessage = AppResources.PasswordsNotMatch;
            }

            return isValid;
        }
        #endregion
    }
}
