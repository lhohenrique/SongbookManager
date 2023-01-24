using SongbookManager.Helpers;
using SongbookManager.Resx;
using SongbookManager.Services;
using SongbookManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
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

        private bool isEditing;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsEditing"));
            }
        }

        private bool isSinger;
        public bool IsSinger
        {
            get { return isSinger; }
            set
            {
                isSinger = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsSinger"));
                UpdateUserAsync();
            }
        }

        private ObservableCollection<string> menuList = new ObservableCollection<string>()
        {
            string.Empty,
            AppResources.ChangePassword,
            AppResources.Feedback,
            //AppResources.RateUs,
            AppResources.PrivacyPolicy,
            AppResources.Logout
        };
        public ObservableCollection<string> MenuList
        {
            get { return menuList; }
            set { menuList = value; }
        }

        private string selectedMenu;
        public string SelectedMenu
        {
            get => selectedMenu;
            set
            {
                selectedMenu = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedMenu"));
                SelectedItemChangedAction();
            }
        }
        #endregion

        #region [Commands]
        public Command EditUserNameCommand { get; set; }
        public Command SaveUserNameCommand { get; set; }
        public Command ChangePasswordCommand { get; set; }
        public Command FeedbackCommand { get; set; }
        public Command RateAppCommand { get; set; }
        public Command SettingsCommand { get; set; }
        public Command PrivacyPolicyCommand { get; set; }
        public Command LogoutCommand { get; set; }
        #endregion

        public ProfilePageViewModel(INavigation navigation)
        {
            this.Navigation = navigation;
            this.userService = new UserService();

            EditUserNameCommand = new Command(() => EditUserNameAction());
            SaveUserNameCommand = new Command(async () => await SaveUserNameAction());
            ChangePasswordCommand = new Command(() => ChangePasswordAction());
            FeedbackCommand = new Command(() => FeedbackAction());
            RateAppCommand = new Command(() => RateAppAction());
            SettingsCommand = new Command(() => SettingsAction());
            PrivacyPolicyCommand = new Command(() => PrivacyPolicyAction());
            LogoutCommand = new Command(() => LogoutAction());
        }

        #region [Actions]
        public void EditUserNameAction()
        {
            IsEditing = true;
        }

        public async Task SaveUserNameAction()
        {
            try
            {
                var user = LoggedUserHelper.LoggedUser;
                user.Name = Name;

                await userService.UpdateUser(user);
            }
            catch (Exception)
            {

            }
            finally
            {
                IsEditing = false;
            }
        }

        public void ChangePasswordAction()
        {
            Navigation.PushAsync(new ChangePasswordPage());
        }

        public void FeedbackAction()
        {
            string emailTo = GlobalVariables.FromEmail;
            string subject = AppResources.FeedbackForSongbook;

            string url = "mailto:" + emailTo + "?subject=" + subject;

            Launcher.OpenAsync(new Uri(url));
        }

        public void RateAppAction()
        {
            //var activity = Android.App.Application.Context;
            //var url = $"market://details?id={(activity as Context)?.PackageName}";

            //string url = Device.RuntimePlatform == Device.iOS ? "https://itunes.apple.com/br/app/skype-para-iphone/id304878510?mt=8"
            //   : "https://play.google.com/store/apps/details?id=com.skype.raider&hl=pt_BR";

            string url = Device.RuntimePlatform == Device.iOS ? "https://itunes.apple.com/br/app/song-folder-lite/id304878510?mt=8"
               : "https://play.google.com/store/search?q=Song%20Folder%20Lite&c=apps";
        
            Launcher.OpenAsync(new Uri(url));
        }

        public void SettingsAction()
        {
            // Navigate to settings page
        }

        public void PrivacyPolicyAction()
        {
            Navigation.PushAsync(new PrivacyPolicyPage());
        }

        public async void LogoutAction()
        {
            var result = await Application.Current.MainPage.DisplayAlert(AppResources.AreYouSureYouWantToLogout, string.Empty, AppResources.Yes, AppResources.Cancel);

            if (result)
            {
                Preferences.Clear();
                LoggedUserHelper.ResetLoggedUser();

                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        public void NavigateToAdminPageAction()
        {
            Navigation.PushAsync(new AdminPage());
        }
        #endregion

        #region [Public Methods]
        public void OnAppearing()
        {
            Name = LoggedUserHelper.LoggedUser.Name;
            Email = LoggedUserHelper.LoggedUser.Email;
            IsSinger = LoggedUserHelper.LoggedUser.IsSinger;
            IsEditing = false;

            if (Email.Equals(GlobalVariables.AdminEmail) && !MenuList.Contains(GlobalVariables.AdminLabel))
            {
                MenuList.Add(GlobalVariables.AdminLabel);
            }
        }

        public void SelectedItemChangedAction()
        {
            if (!string.IsNullOrEmpty(SelectedMenu))
            {
                if (SelectedMenu.Equals(AppResources.ChangePassword))
                {
                    ChangePasswordAction();
                }
                else if (SelectedMenu.Equals(AppResources.Feedback))
                {
                    FeedbackAction();
                }
                else if (SelectedMenu.Equals(AppResources.RateUs))
                {
                    RateAppAction();
                }
                else if (SelectedMenu.Equals(AppResources.PrivacyPolicy))
                {
                    PrivacyPolicyAction();
                }
                else if (SelectedMenu.Equals(AppResources.Logout))
                {
                    LogoutAction();
                }
                else if (SelectedMenu.Equals(GlobalVariables.AdminLabel))
                {
                    NavigateToAdminPageAction();
                }

                SelectedMenu = string.Empty;
            }
        }
        #endregion

        #region [Private Methods]
        private async void UpdateUserAsync()
        {
            try
            {
                var user = LoggedUserHelper.LoggedUser;
                user.IsSinger = IsSinger;

                await userService.UpdateUser(user);
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }
}
