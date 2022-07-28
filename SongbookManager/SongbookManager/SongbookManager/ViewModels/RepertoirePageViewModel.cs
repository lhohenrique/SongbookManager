using SongbookManager.Helpers;
using SongbookManager.Models;
using SongbookManager.Resx;
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
    public class RepertoirePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation { get; set; }

        #region [Properties]
        private Repertoire selectedRepertoire;
        public Repertoire SelectedRepertoire
        {
            get => selectedRepertoire;
            set
            {
                selectedRepertoire = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedRepertoire"));
                SelectedRepertoireAction();
            }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set
            {
                date = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Time"));
            }
        }

        private ObservableCollection<Repertoire> repertoireList = new ObservableCollection<Repertoire>();
        public ObservableCollection<Repertoire> RepertoireList
        {
            get { return repertoireList; }
            set { repertoireList = value; }
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
        #endregion

        #region [Commands]
        public ICommand NewRepertoireCommand
        {
            get => new Command(() =>
            {
                NewRepertoireAction();
            });
        }

        public ICommand UpdateRepertoireListCommand
        {
            get => new Command(async () =>
            {
                await UpdateRepertoireListAction();
            });
        }

        public ICommand MostRecentOrderCommand
        {
            get => new Command(() =>
            {
                MostRecentOrderAction();
            });
        }

        public ICommand OldestOrderCommand
        {
            get => new Command(() =>
            {
                OldestOrderAction();
            });
        }
        #endregion

        public RepertoirePageViewModel(INavigation navigation)
        {
            this.Navigation = navigation;

            //repertoireService = new RepertoireService();
        }

        #region [Actions]
        private void SelectedRepertoireAction()
        {
            if (SelectedRepertoire != null)
            {
                //Navigation.PushAsync(new PreviewRepertoirePage(selectedRepertoire));
            }
        }

        private void NewRepertoireAction()
        {
            //Navigation.PushAsync(new AddEditRepertoirePage(null));
        }

        private async Task UpdateRepertoireListAction()
        {
            try
            {
                if (IsUpdating)
                {
                    return;
                }

                IsUpdating = true;

                await LoggedUserHelper.UpdateLoggedUserAsync();

                var userEmail = LoggedUserHelper.GetEmail();
                //List<Repertoire> repertoireListUpdated = await repertoireService.GetMusicsByUser(userEmail);

                RepertoireList.Clear();

                //repertoireListUpdated.ForEach(i => MusicList.Add(i));
            }
            catch (Exception)
            {
                //await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotUpdateSongList, AppResources.Ok);
            }
            finally
            {
                IsUpdating = false;
            }
        }

        private void MostRecentOrderAction()
        {
            //var orderedList = RepertoireList.OrderByDescending(m => m.CreationDate).ToList();

            RepertoireList.Clear();
            //orderedList.ForEach(i => RepertoireList.Add(i));
        }

        private void OldestOrderAction()
        {
            //var orderedList = RepertoireList.OrderBy(m => m.CreationDate).ToList();

            RepertoireList.Clear();
            //orderedList.ForEach(i => RepertoireList.Add(i));
        }
        #endregion
    }
}
