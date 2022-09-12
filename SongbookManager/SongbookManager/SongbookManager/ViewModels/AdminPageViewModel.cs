using SongbookManager.Models;
using SongbookManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class AdminPageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private UserService userService;
        private MusicService musicService;

        #region [Properties]
        private ObservableCollection<Music> musicList = new ObservableCollection<Music>();
        public ObservableCollection<Music> MusicList
        {
            get { return musicList; }
            set { musicList = value; }
        }

        private ObservableCollection<User> userList = new ObservableCollection<User>();
        public ObservableCollection<User> UserList
        {
            get { return userList; }
            set { userList = value; }
        }

        private int totalUsers;
        public int TotalUsers
        {
            get { return totalUsers; }
            set
            {
                totalUsers = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TotalUsers"));
            }
        }

        private int totalMusics;
        public int TotalMusics
        {
            get { return totalMusics; }
            set
            {
                totalMusics = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TotalMusics"));
            }
        }
        #endregion

        #region [Commands]
        #endregion

        public AdminPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            userService = new UserService();
            musicService = new MusicService();
        }

        #region [Action]
        #endregion

        #region [Public Methods]
        public async Task LoadingPageAsync()
        {
            await GetUsersAsync();
            await GetMusicsAsync();

            TotalUsers = UserList.Count;
            TotalMusics = MusicList.Count;
        }

        public async Task RemoveMusicAction(Music music)
        {
            try
            {
                await musicService.DeleteMusic(music);

                await GetMusicsAsync();
                TotalMusics = MusicList.Count;
            }
            catch (Exception)
            {

            }
        }

        public async Task RemoveUserAction(User user)
        {
            try
            {
                await userService.DeleteUser(user);

                await GetUsersAsync();
                TotalUsers = UserList.Count;
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region [Private Methods]
        public async Task GetUsersAsync()
        {
            try
            {
                List<User> users = await userService.GetUsers();

                UserList.Clear();
                users.ForEach(u => UserList.Add(u));
            }
            catch (Exception)
            {

            }
        }

        public async Task GetMusicsAsync()
        {
            try
            {
                List<Music> musics = await musicService.GetMusics();

                MusicList.Clear();
                musics.ForEach(m => MusicList.Add(m));
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }
}
