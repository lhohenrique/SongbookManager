using SongbookManager.Models;
using SongbookManager.Resx;
using SongbookManager.Services;
using SongbookManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class PreviewRepertoirePageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private RepertoireService repertoireService;

        private Repertoire repertoire;

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

        private ObservableCollection<Music> musics = new ObservableCollection<Music>();
        public ObservableCollection<Music> Musics
        {
            get { return musics; }
            set { musics = value; }
        }
        #endregion

        #region [Commands]
        public Command EditRepertoireCommand { get; set; }
        public Command RemoveRepertoireCommand { get; set; }
        #endregion

        public PreviewRepertoirePageViewModel(INavigation navigation, Repertoire repertoire)
        {
            Navigation = navigation;
            this.repertoire = repertoire;

            EditRepertoireCommand = new Command(() => EditRepertoireAction());
            RemoveRepertoireCommand = new Command(async () => await RemoveRepertoireAction());

            repertoireService = new RepertoireService();
        }

        #region [Actions]
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
        #endregion

        #region [Public Methods]
        public void LoadPage()
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
                        repertoire.Musics.ForEach(m => Musics.Add(m));
                    }
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
