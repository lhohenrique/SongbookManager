using SongbookManager.Helpers;
using SongbookManager.Models;
using SongbookManager.Resx;
using SongbookManager.Services;
using SongbookManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class PreviewRepertoirePageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private RepertoireService repertoireService;
        private KeyService keyService;

        private Repertoire repertoire;
        private bool isReordering = false;
        private MusicRep musicToReorder;
        private int musicIndexToReorder = -1;

        #region [Properties]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string date;
        public string Date
        {
            get => date;
            set
            {
                date = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }

        private string time;
        public string Time
        {
            get => time;
            set
            {
                time = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Time"));
            }
        }

        private ObservableCollection<MusicRep> musics = new ObservableCollection<MusicRep>();
        public ObservableCollection<MusicRep> Musics
        {
            get { return musics; }
            set { musics = value; }
        }
        #endregion

        #region [Commands]
        public Command PlayRepertoireCommand { get; set; }
        public Command SendRepertoireCommand { get; set; }
        public Command EditRepertoireCommand { get; set; }
        public Command RemoveRepertoireCommand { get; set; }
        public Command TutorialCommand { get; set; }
        #endregion

        public PreviewRepertoirePageViewModel(INavigation navigation, Repertoire repertoire)
        {
            Navigation = navigation;
            this.repertoire = repertoire;

            PlayRepertoireCommand = new Command(() => PlayRepertoireAction());
            SendRepertoireCommand = new Command(async () => await SendRepertoireAction());
            EditRepertoireCommand = new Command(() => EditRepertoireAction());
            RemoveRepertoireCommand = new Command(async () => await RemoveRepertoireAction());
            TutorialCommand = new Command(async () => await TutorialActionAsync());

            repertoireService = new RepertoireService();
            keyService = new KeyService();
        }

        #region [Actions]
        public void PlayRepertoireAction()
        {
            if (repertoire != null)
            {
                Navigation.PushAsync(new PlayRepertoirePage(repertoire));
            }
        }

        public async Task SendRepertoireAction()
        {
            await Utils.SendRepertoire(repertoire);
        }

        public void EditRepertoireAction()
        {
            if (repertoire != null)
            {
                Navigation.PushAsync(new AddEditRepertoirePage(repertoire));
            }
        }

        public async Task RemoveRepertoireAction()
        {
            if (repertoire != null)
            {
                try
                {
                    var result = await Application.Current.MainPage.DisplayAlert(AppResources.AreYouShure, AppResources.AreYouShureRepertoireRemoved, AppResources.Yes, AppResources.No);

                    if (result)
                    {
                        await repertoireService.DeleteRepertoire(repertoire);
                    }
                }
                catch (Exception)
                {
                    await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotRemoveRepertoire, AppResources.Ok);
                }
            }

            await Navigation.PopAsync();
        }

        private async Task TutorialActionAsync()
        {
            string message = string.Empty;
            message += "- " + AppResources.PreviewRepertoirePageTutorial;
            message += "\n\n- " + AppResources.PreviewRepertoireReorderTutorial;
            message += "\n\n- " + AppResources.PreviewRepertoirePlayTutorial;
            message += "\n\n- " + AppResources.PreviewRepertoireSendTutorial;
            message += "\n\n- " + AppResources.PreviewRepertoireEditRemoveTutorial;

            await Application.Current.MainPage.DisplayAlert(AppResources.Tutorial, message, AppResources.Ok);
        }
        #endregion

        #region [Public Methods]
        public async Task LoadPageAsync()
        {
            try
            {
                if(repertoire != null)
                {
                    Name = repertoire.SingerName;
                    Date = repertoire.DateFormated;
                    Time = repertoire.TimeFormated;

                    Musics.Clear();

                    if (repertoire.Musics != null)
                    {
                        foreach (MusicRep music in repertoire.Musics)
                        {
                            if (!string.IsNullOrEmpty(repertoire.SingerEmail))
                            {
                                var key = await keyService.GetKeyByUser(repertoire.SingerEmail, music.Name);
                                if (key != null)
                                {
                                    music.SingerKey = key.Key;
                                }
                            }

                            Musics.Add(music);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task SelectionChangedAction(MusicRep musicTapped, int musicTappedIndex)
        {
            try
            {
                if (musicTapped.IsReordering)
                {
                    musicTapped.IsReordering = false;

                    Musics.RemoveAt(musicTappedIndex);
                    Musics.Insert(musicTappedIndex, musicTapped);

                    isReordering = false;
                    musicToReorder = null;
                    musicIndexToReorder = -1;
                }
                else if (isReordering)
                {
                    musicToReorder.IsReordering = false;

                    Musics.RemoveAt(musicIndexToReorder);
                    Musics.Insert(musicTappedIndex, musicToReorder);

                    isReordering = false;
                    musicToReorder = null;
                    musicIndexToReorder = -1;

                    // Update Repertoire
                    repertoire.Musics = Musics.ToList();
                    await repertoireService.UpdateRepertoire(repertoire, repertoire.Date, repertoire.Time);
                }
                else
                {
                    musicTapped.IsReordering = true;

                    Musics.RemoveAt(musicTappedIndex);
                    Musics.Insert(musicTappedIndex, musicTapped);

                    isReordering = true;
                    musicToReorder = musicTapped;
                    musicIndexToReorder = musicTappedIndex;
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region [Private Methods]
        #endregion
    }
}
