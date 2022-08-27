using SongbookManager.Helpers;
using SongbookManager.Models;
using SongbookManager.Resx;
using SongbookManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class AddEditRepertoirePageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private Repertoire repertoire;
        private DateTime oldDate;
        private TimeSpan oldTime;
        private RepertoireService repertoireService;
        private MusicService musicService;
        private KeyService keyService;
        private UserService userService;
        private bool pageLoaded = false;

        private List<User> singerUserList = new List<User>();

        #region [Properties]
        private string selectedSinger;
        public string SelectedSinger
        {
            get { return selectedSinger; }
            set
            {
                selectedSinger = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedSinger"));
            }
        }

        private ObservableCollection<string> singers = new ObservableCollection<string>();
        public ObservableCollection<string> Singers
        {
            get { return singers; }
            set { Singers = value; }
        }

        private DateTime date = DateTime.Now;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }

        private TimeSpan time = TimeSpan.Zero;
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Time"));
            }
        }

        private ObservableCollection<MusicRep> musicList = new ObservableCollection<MusicRep>();
        public ObservableCollection<MusicRep> MusicList
        {
            get { return musicList; }
            set { musicList = value; }
        }

        private List<MusicRep> selectedMusics = new List<MusicRep>();
        public List<MusicRep> SelectedMusics
        {
            get
            {
                return selectedMusics;
            }
            set
            {
                if (selectedMusics != value)
                {
                    selectedMusics = value;
                }
            }
        }

        private MusicRep selectedMusic;
        public MusicRep SelectedMusic
        {
            get
            {
                return selectedMusic;
            }
            set
            {
                selectedMusic = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedMusic"));
            }
        }

        private string searchText = string.Empty;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
            }
        }
        #endregion

        #region [Commands]
        public Command SaveRepertoireCommand { get; set; }
        public Command SearchCommand { get; set; }
        #endregion

        public AddEditRepertoirePageViewModel(INavigation navigation, Repertoire repertoire)
        {
            Navigation = navigation;

            repertoireService = new RepertoireService();
            musicService = new MusicService();
            keyService = new KeyService();
            userService = new UserService();

            SaveRepertoireCommand = new Command(async () => await SaveRepertoireActionAsync());
            SearchCommand = new Command(async () => await SearchActionAsync());

            this.repertoire = repertoire;

            if (repertoire != null)
            {
                oldDate = repertoire.Date;
                oldTime = repertoire.Time;
            }
        }

        #region [Actions]
        public async Task SaveRepertoireActionAsync()
        {
            try
            {
                string userEmail = GetSingerEmail();

                foreach (MusicRep music in SelectedMusics)
                {
                    if (!string.IsNullOrEmpty(SelectedSinger))
                    {
                        var key = await keyService.GetKeyByUser(userEmail, music.Name);
                        if (key != null)
                        {
                            music.SingerKey = key.Key;
                        }
                    }
                }

                if (repertoire != null)
                {
                    repertoire.Date = Date;
                    repertoire.Time = Time;
                    repertoire.Musics = SelectedMusics.ToList();
                    repertoire.SingerName = SelectedSinger;
                    repertoire.SingerEmail = userEmail;

                    await repertoireService.UpdateRepertoire(repertoire, oldDate, oldTime);
                }
                else
                {
                    Repertoire newRepertoire = new Repertoire()
                    {
                        Date = Date,
                        Time = Time,
                        Musics = SelectedMusics.ToList(),
                        Owner = LoggedUserHelper.GetEmail(),
                        SingerName = SelectedSinger,
                        SingerEmail = userEmail
                    };

                    await repertoireService.InsertRepertoire(newRepertoire);

                    var result = await Application.Current.MainPage.DisplayAlert(AppResources.SendRepertoire, AppResources.WantToSendRepertoire, AppResources.Yes, AppResources.No);
                    if (result)
                    {
                        await Utils.SendRepertoire(newRepertoire);
                    }
                }

                await Navigation.PopAsync();
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotSaveRepertoire, AppResources.Ok);
            }
        }

        public void SelectionChangedAction(MusicRep musicTapped, int musicTappedIndex)
        {
            if (musicTapped.IsSelected)
            {
                musicTapped.IsSelected = false;

                SelectedMusics.Remove(musicTapped);

                MusicList.RemoveAt(musicTappedIndex);
                MusicList.Insert(musicTappedIndex, musicTapped);
            }
            else
            {
                musicTapped.IsSelected = true;

                SelectedMusics.Add(musicTapped);

                MusicList.RemoveAt(musicTappedIndex);
                MusicList.Insert(musicTappedIndex, musicTapped);
            }
        }

        private async Task SearchActionAsync()
        {
            try
            {
                if (!pageLoaded)
                {
                    return;
                }

                var userEmail = LoggedUserHelper.GetEmail();
                List<Music> musicListUpdated = await musicService.SearchMusic(SearchText, userEmail);

                MusicList.Clear();

                foreach (Music music in musicListUpdated)
                {
                    MusicList.Add(new MusicRep()
                    {
                        Name = music.Name,
                        Author = music.Author,
                        Owner = music.Owner
                    });
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnablePerformSearch, AppResources.Ok);
            }
        }
        #endregion

        #region [Public Methods]
        public async Task PopulateRepertoireFieldsAsync()
        {
            await LoadSingersAsync();
            await LoadMusicsAsync();

            if (repertoire != null)
            {
                LoadRepertoire();
            }
            else
            {    
                SetLoggedSinger();
            }

            pageLoaded = true;
        }
        #endregion

        #region [Private Methods]
        private void LoadRepertoire()
        {
            SelectedSinger = repertoire.SingerName;
            Date = repertoire.Date;
            Time = repertoire.Time;

            if(repertoire.Musics != null)
            {
                SelectedMusics = repertoire.Musics.ToList();

                foreach (MusicRep musicSelected in repertoire.Musics)
                {
                    MusicRep item = MusicList.FirstOrDefault(m => m.Name.Equals(musicSelected.Name) && m.Author.Equals(musicSelected.Author));
                    int index = MusicList.IndexOf(item);
                    
                    if(index != -1)
                    {
                        MusicList.RemoveAt(index);
                        MusicList.Insert(index, musicSelected);
                    }
                }
            }
        }

        private async Task LoadSingersAsync()
        {
            try
            {
                var musicOwner = LoggedUserHelper.GetEmail();
                singerUserList = await userService.GetSingers(musicOwner);

                Singers.Clear();

                singerUserList.ForEach(i => Singers.Add(i.Name));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotLoadSingers, AppResources.Ok);
            }
        }

        private async Task LoadMusicsAsync()
        {
            try
            {
                await LoggedUserHelper.UpdateLoggedUserAsync();

                var userEmail = LoggedUserHelper.GetEmail();
                List<Music> musicListUpdated = await musicService.GetMusicsByUserDescending(userEmail);

                MusicList.Clear();

                foreach (Music music in musicListUpdated)
                {
                    MusicList.Add(new MusicRep()
                    {
                        Name = music.Name,
                        Author = music.Author,
                        Owner = music.Owner,
                        IsSelected = false
                    });
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotUpdateSongList, AppResources.Ok);
            }
        }

        private void SetLoggedSinger()
        {
            if (LoggedUserHelper.LoggedUser.IsSinger)
            {
                SelectedSinger = LoggedUserHelper.LoggedUser.Name;
            }
        }

        private string GetSingerEmail()
        {
            string singerEmail = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSinger))
            {
                if (SelectedSinger.Equals(LoggedUserHelper.LoggedUser.Name))
                {
                    singerEmail = LoggedUserHelper.LoggedUser.Email;
                }
                else
                {
                    var user = singerUserList.FirstOrDefault(s => s.Name.Equals(SelectedSinger));
                    if(user != null)
                    {
                        singerEmail = user.Email;
                    }
                }
            }
            
            return singerEmail;
        }
        #endregion
    }
}
