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
    public class DataPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private RepertoireService repertoireService;
        private UserService userService;
        List<User> singerUserList = new List<User>();
        private bool pageLoaded = false;

        public INavigation Navigation { get; set; }

        #region [Properties]
        private ObservableCollection<string> singers = new ObservableCollection<string>();
        public ObservableCollection<string> Singers
        {
            get { return singers; }
            set { Singers = value; }
        }

        private ObservableCollection<string> periodsList = new ObservableCollection<string>();
        public ObservableCollection<string> PeriodsList
        {
            get { return periodsList; }
            set { PeriodsList = value; }
        }

        private ObservableCollection<MusicData> musicDataList = new ObservableCollection<MusicData>();
        public ObservableCollection<MusicData> MusicDataList
        {
            get { return musicDataList; }
            set { MusicDataList = value; }
        }

        private string selectedSinger;
        public string SelectedSinger
        {
            get { return selectedSinger; }
            set
            {
                selectedSinger = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedSinger"));
                UpdateData();
            }
        }

        private string selectedPeriod;
        public string SelectedPeriod
        {
            get { return selectedPeriod; }
            set
            {
                selectedPeriod = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedPeriod"));
                UpdateData();
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("StartDate"));
            }
        }

        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("EndDate"));
            }
        }

        private bool isCustomPeriod = false;
        public bool IsCustomPeriod
        {
            get { return isCustomPeriod; }
            set
            {
                isCustomPeriod = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsCustomPeriod"));
            }
        }
        #endregion

        public DataPageViewModel()
        {
            repertoireService = new RepertoireService();
            userService = new UserService();
        }

        #region [Public Methods]
        public async Task LoadPageAsync()
        {
            await LoadSingersAsync();
            PopulatePeriods();

            SelectedSinger = AppResources.All;
            SelectedPeriod = AppResources.LastMonth;

            pageLoaded = true;

            await UpdateData();
        }

        public async Task UpdateData()
        {
            try
            {
                if (pageLoaded)
                {
                    List<Repertoire> repertoireList = new List<Repertoire>();

                    // Get period
                    DateTime start;
                    DateTime end;
                    if (SelectedPeriod.Equals(AppResources.LastMonth))
                    {
                        end = DateTime.Now;
                        start = end.Subtract(TimeSpan.FromDays(30));
                    }
                    else if (SelectedPeriod.Equals(AppResources.Last6Months))
                    {
                        end = DateTime.Now;
                        start = end.Subtract(TimeSpan.FromDays(180));
                    }
                    else if (SelectedPeriod.Equals(AppResources.LastYear))
                    {
                        end = DateTime.Now;
                        start = end.Subtract(TimeSpan.FromDays(365));
                    }
                    else // Custom period
                    {
                        start = StartDate;
                        end = EndDate;
                    }

                    if (SelectedSinger.Equals(AppResources.All))
                    {
                        await LoggedUserHelper.UpdateLoggedUserAsync();

                        var owner = LoggedUserHelper.GetEmail();
                        repertoireList = await repertoireService.GetRepertoiresByPeriod(owner, start, end);
                    }
                    else
                    {
                        string singerEmail = GetSingerEmail();
                        repertoireList = await repertoireService.GetRepertoiresBySingerAndPeriod(singerEmail, start, end);
                    }

                    List<MusicData> musicsOrdered = GetMostPlayedSongs(repertoireList);

                    MusicDataList.Clear();

                    musicsOrdered.ForEach(i => MusicDataList.Add(i));
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotLoadSongData, AppResources.Ok);
            }
        }

        private List<MusicData> GetMostPlayedSongs(List<Repertoire> repertoireList)
        {
            List<MusicData> musicsOrdered = new List<MusicData>();
            List<MusicData> musicDataList = new List<MusicData>();
            List<Music> musics = new List<Music>();

            repertoireList.ForEach(r => musics.AddRange(r.Musics));

            foreach (Music music in musics)
            {
                int index = musicDataList.FindIndex(m => m.Name.Equals(music.Name));
                if (index == -1)
                {
                    musicDataList.Add(new MusicData { Name = music.Name, Count = 1 });
                }
                else
                {
                    musicDataList[index].Count++;
                }
            }

            musicsOrdered = musicDataList.OrderByDescending(m => m.Count).ToList();

            return musicsOrdered;
        }
        #endregion

        #region [Private Methods]
        private async Task LoadSingersAsync()
        {
            try
            {
                var owner = LoggedUserHelper.GetEmail();
                singerUserList = await userService.GetSingers(owner);
                
                Singers.Clear();

                Singers.Add(AppResources.All);
                singerUserList.ForEach(i => Singers.Add(i.Name));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotLoadSingers, AppResources.Ok);
            }
        }

        private void PopulatePeriods()
        {
            PeriodsList.Add(AppResources.LastMonth);
            PeriodsList.Add(AppResources.Last6Months);
            PeriodsList.Add(AppResources.LastYear);
            //PeriodsList.Add(AppResources.Custom);
        }

        private string GetSingerEmail()
        {
            string singerEmail = string.Empty;

            if (!SelectedSinger.Equals(AppResources.All))
            {
                var user = singerUserList.FirstOrDefault(s => s.Name.Equals(SelectedSinger));
                if (user != null)
                {
                    singerEmail = user.Email;
                }
            }

            return singerEmail;
        }
        #endregion
    }
}
