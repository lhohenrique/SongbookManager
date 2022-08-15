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

        private ObservableCollection<Music> musicList = new ObservableCollection<Music>();
        public ObservableCollection<Music> MusicList
        {
            get { return musicList; }
            set { musicList = value; }
        }

        private ObservableCollection<Music> selectedMusics = new ObservableCollection<Music>();
        public ObservableCollection<Music> SelectedMusics
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

        private Music selectedMusic;
        public Music SelectedMusic
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
        #endregion

        #region [Commands]
        public Command SaveRepertoireCommand { get; set; }
        #endregion

        public AddEditRepertoirePageViewModel(INavigation navigation, Repertoire repertoire)
        {
            Navigation = navigation;

            repertoireService = new RepertoireService();
            musicService = new MusicService();
            keyService = new KeyService();
            userService = new UserService();

            SaveRepertoireCommand = new Command(async () => await SaveRepertoireActionAsync());

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

                foreach (Music music in SelectedMusics)
                {
                    if (!string.IsNullOrEmpty(SelectedSinger))
                    {
                        var key = await keyService.GetKeyByUser(userEmail, music.Name);
                        if (key != null)
                        {
                            music.Key = key.Key;
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
                }

                await Navigation.PopAsync();
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotSaveRepertoire, AppResources.Ok);
            }
        }

        public void SelectionChangedAction(List<Music> musics)
        {
            SelectedMusics.Clear();
            musics.ForEach(m => SelectedMusics.Add(m));
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
        }
        #endregion

        #region [Private Methods]
        private void LoadRepertoire()
        {
            SelectedSinger = repertoire.SingerName;
            Date = repertoire.Date;
            Time = repertoire.Time;

            // TODO: Fix musics selected state when seting from source
            //if(repertoire.Musics != null)
            //{
                //SelectedMusic = repertoire.Musics.FirstOrDefault();
                //repertoire.Musics.ForEach(m => SelectedMusics.Add(m));
            //}
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
                List<Music> musicListUpdated = await musicService.GetMusicsByUser(userEmail);

                MusicList.Clear();

                musicListUpdated.ForEach(i => MusicList.Add(i));
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
