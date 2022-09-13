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
using System.Windows.Input;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class RepertoirePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private RepertoireService repertoireService;
        private bool pageLoaded = false;

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

        public ICommand SearchCommand
        {
            get => new Command(async () =>
            {
                await SearchAction();
            });
        }

        public ICommand DataCommand
        {
            get => new Command(() =>
            {
                DataAction();
            });
        }

        public ICommand TutorialCommand
        {
            get => new Command(async () =>
            {
                await TutorialAction();
            });
        }
        #endregion

        public RepertoirePageViewModel(INavigation navigation)
        {
            this.Navigation = navigation;

            repertoireService = new RepertoireService();
        }

        #region [Actions]
        private void SelectedRepertoireAction()
        {
            if (SelectedRepertoire != null)
            {
                Navigation.PushAsync(new PreviewRepertoirePage(selectedRepertoire));
            }
        }

        private void NewRepertoireAction()
        {
            Navigation.PushAsync(new AddEditRepertoirePage(null));
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

                var owner = LoggedUserHelper.GetEmail();
                List<Repertoire> repertoireListUpdated = await repertoireService.GetRepertoiresByUser(owner);

                RepertoireList.Clear();

                repertoireListUpdated.ForEach(i => RepertoireList.Add(i));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotUpdateRepertoireList, AppResources.Ok);
            }
            finally
            {
                IsUpdating = false;
            }
        }

        private async Task SearchAction()
        {
            try
            {
                if (!pageLoaded)
                {
                    return;
                }

                var userEmail = LoggedUserHelper.GetEmail();
                List<Repertoire> repertoireListUpdated = await repertoireService.SearchRepertoire(SearchText, userEmail);

                RepertoireList.Clear();

                repertoireListUpdated.ForEach(i => RepertoireList.Add(i));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnablePerformSearch, AppResources.Ok);
            }
        }

        private void MostRecentOrderAction()
        {
            var orderedList = RepertoireList.OrderByDescending(r => r.Date).ToList();

            RepertoireList.Clear();
            orderedList.ForEach(i => RepertoireList.Add(i));
        }

        private void OldestOrderAction()
        {
            var orderedList = RepertoireList.OrderBy(r => r.Date).ToList();

            RepertoireList.Clear();
            orderedList.ForEach(i => RepertoireList.Add(i));
        }

        private void DataAction()
        {
            Navigation.PushAsync(new DataPage());
        }

        private async Task TutorialAction()
        {
            string message = string.Empty;
            message += "- " + AppResources.RepertoiresPageTutorial;
            message += "\n\n- " + AppResources.RepertoiresAddRepertoireTutorial;
            message += "\n\n- " + AppResources.RepertoiresPreviewRepertoireTutorial;
            message += "\n\n- " + AppResources.RepertoiresSearchTutorial;

            await Application.Current.MainPage.DisplayAlert(AppResources.Tutorial, message, AppResources.Ok);
        }
        #endregion

        #region [Public Methods]
        public async Task LoadingPage()
        {
            await UpdateRepertoireListAction();
            pageLoaded = true;
        }
        #endregion
    }
}
