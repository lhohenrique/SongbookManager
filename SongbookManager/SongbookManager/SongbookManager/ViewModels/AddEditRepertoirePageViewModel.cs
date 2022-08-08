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

        #region [Properties]
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
        #endregion

        #region [Commands]
        public Command SaveRepertoireCommand { get; set; }
        #endregion

        public AddEditRepertoirePageViewModel(INavigation navigation, Repertoire repertoire)
        {
            Navigation = navigation;

            repertoireService = new RepertoireService();
            musicService = new MusicService();

            SaveRepertoireCommand = new Command(() => SaveRepertoireAction());

            this.repertoire = repertoire;

            if (repertoire != null)
            {
                oldDate = repertoire.Date;
                oldTime = repertoire.Time;
            }
        }

        #region [Actions]
        public void SaveRepertoireAction()
        {
            Repertoire newRepertoire = new Repertoire()
            {
                Date = Date,
                Time = Time,
                Musics = SelectedMusics.ToList()
            };
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
            if (repertoire != null)
            {
                //LoadRepertoire();
            }
            else
            {
                await LoadMusicsAsync();
            }

            //HasSingers = UserList.Any();
        }
        #endregion

        #region [Private Methods]
        private async Task HandleRepertoireFields()
        {
            await LoadMusicsAsync();
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
        #endregion
    }
}
