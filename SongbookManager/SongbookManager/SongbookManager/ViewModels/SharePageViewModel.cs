using SongbookManager.Helpers;
using SongbookManager.Models;
using SongbookManager.Resx;
using SongbookManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class SharePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        
        private UserService userService;
        private KeyService keyService;

        #region [Properties]
        private ObservableCollection<User> userList = new ObservableCollection<User>();
        public ObservableCollection<User> UserList
        {
            get { return userList; }
            set { userList = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));
                HandleShareButton();
            }
        }

        private string sharedName;
        public string SharedName
        {
            get { return sharedName; }
            set
            {
                sharedName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SharedName"));
            }
        }

        private string sharedEmail;
        public string SharedEmail
        {
            get { return sharedEmail; }
            set
            {
                sharedEmail = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SharedEmail"));
            }
        }

        private bool isShareEnabled = false;
        public bool IsShareEnabled
        {
            get { return isShareEnabled; }
            set
            {
                isShareEnabled = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsShareEnabled"));
            }
        }

        private bool isUpdating = false;
        public bool IsUpdating
        {
            get { return isUpdating; }
            set
            {
                isUpdating = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsUpdating"));
            }
        }

        private bool isSharedList = false;
        public bool IsSharedList
        {
            get { return isSharedList; }
            set
            {
                isSharedList = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsSharedList"));
            }
        }
        #endregion

        #region [Command]
        public Command ShareCommand { get; set; }
        public Command UpdateUserListCommand { get; set; }
        public Command<User> RemoveShareCommand { get; set; }
        public Command UnshareCommand { get; set; }
        #endregion

        public SharePageViewModel(INavigation navigation)
        {
            ShareCommand = new Command(async () => await ShareAction());
            UpdateUserListCommand = new Command(async () => await UpdateUserListAction());
            RemoveShareCommand = new Command<User>(async (User user) => await RemoveShareAction(user));
            UnshareCommand = new Command(async () => await UnshareAction());

            Navigation = navigation;
            userService = new UserService();
            keyService = new KeyService();
        }

        #region [Actions]
        private async Task ShareAction()
        {
            try
            {
                var userToShare = await userService.GetUser(Email);

                if (userToShare == null)
                {
                    await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UserNotFound, AppResources.Ok);
                }
                else if (!string.IsNullOrEmpty(userToShare.SharedList))
                {
                    await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UserAlreadyAccessingSharedSongList, AppResources.Ok);
                }
                else
                {
                    userToShare.SharedList = LoggedUserHelper.GetEmail();
                    await userService.UpdateUser(userToShare);

                    Email = string.Empty;

                    await UpdateUserListAction();
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnableShareSongList, AppResources.Ok);
            }
        }

        private async Task UpdateUserListAction()
        {
            try
            {
                if (IsUpdating)
                {
                    return;
                }

                IsUpdating = true;

                var userEmail = LoggedUserHelper.GetEmail();
                List<User> userListUpdated = await userService.GetSharedUsers(userEmail);

                UserList.Clear();

                userListUpdated.ForEach(i => UserList.Add(i));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotUpdateSharedUserList, AppResources.Ok);
            }
            finally
            {
                IsUpdating = false;
            }
        }

        private async Task RemoveShareAction(User user)
        {
            try
            {
                var result = await Application.Current.MainPage.DisplayAlert(AppResources.AreYouShure, AppResources.ThisUserWillBeRemovedFromYourShareList, AppResources.Yes, AppResources.No);

                if (result)
                {
                    var userToRemove = await userService.GetUser(user.Email);
                    userToRemove.SharedList = string.Empty;

                    await userService.UpdateUser(userToRemove);

                    await UpdateUserListAction();

                    // Remove all keys from this user
                    await keyService.ClearUserKeys(user.Email);
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnableRemoveShare, AppResources.Ok);
            }
        }

        private async Task UnshareAction()
        {
            try
            {
                var result = await Application.Current.MainPage.DisplayAlert(AppResources.AreYouShure, AppResources.YouWillNoLongerHaveAccessToThisSongList, AppResources.Yes, AppResources.No);

                if (result)
                {
                    LoggedUserHelper.LoggedUser.SharedList = string.Empty;
                    await userService.UpdateUser(LoggedUserHelper.LoggedUser);

                    await HandlePageState();
                    
                    // Remove all keys from this user
                    await keyService.ClearUserKeys(LoggedUserHelper.LoggedUser.Email);
                }
            }
            catch(Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnableToUnshare, AppResources.Ok);
            }
        }
        #endregion

        #region [Public Methods]
        public void HandleShareButton()
        {
            IsShareEnabled = !string.IsNullOrEmpty(Email);   
        }

        public async Task HandlePageState()
        {
            var sharedEmail = LoggedUserHelper.LoggedUser.SharedList;

            if (string.IsNullOrEmpty(sharedEmail))
            {
                await UpdateUserListAction();

                HandleShareButton();

                IsSharedList = false;
            }
            else
            {
                var sharedUser = await userService.GetUser(sharedEmail);
                if(sharedUser != null)
                {
                    SharedName = sharedUser.Name;
                    SharedEmail = sharedUser.Email;
                }

                IsSharedList = true;
            }
        }
        #endregion
    }
}
