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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class PreviewMusicPageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private Music music;
        private MusicService musicService;
        private KeyService keyService;

        #region [Properties]
        private ObservableCollection<UserKey> userList = new ObservableCollection<UserKey>();
        public ObservableCollection<UserKey> UserList
        {
            get { return userList; }
            set { userList = value; }
        }

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

        private string author;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Author"));
            }
        }

        private string lyrics;
        public string Lyrics
        {
            get { return lyrics; }
            set
            {
                lyrics = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Lyrics"));
            }
        }

        private string chords;
        public string Chords
        {
            get { return chords; }
            set
            {
                chords = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Chords"));
            }
        }

        private bool hasSingers = false;
        public bool HasSingers
        {
            get { return hasSingers; }
            set
            {
                hasSingers = value;
                PropertyChanged(this, new PropertyChangedEventArgs("HasSingers"));
            }
        }

        private bool isPlayMusicVisible = false;
        public bool IsPlayMusicVisible
        {
            get { return isPlayMusicVisible; }
            set
            {
                isPlayMusicVisible = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsPlayMusicVisible"));
            }
        }

        private ObservableCollection<string> keyList = new ObservableCollection<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public ObservableCollection<string> KeyList
        {
            get { return keyList; }
            set { keyList = value; }
        }

        private string selectedKey;
        public string SelectedKey
        {
            get { return selectedKey; }
            set
            {
                selectedKey = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedKey"));
                SetChords();
            }
        }
        #endregion

        #region [Commands]
        public ICommand ShowMusicDetailsCommand
        {
            get => new Command(async () =>
            {
                await ShowMusicDetailsAction();
            });
        }

        public ICommand EditMusicCommand
        {
            get => new Command(() =>
            {
                EditMusicAction();
            });
        }

        public ICommand RemoveMusicCommand
        {
            get => new Command(async () =>
            {
                await RemoveMusicAction();
            });
        }

        public ICommand PlayMusicCommand
        {
            get => new Command(() =>
            {
                PlayMusicAction();
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

        public PreviewMusicPageViewModel(INavigation navigation, Music music)
        {
            Navigation = navigation;
            this.music = music;

            musicService = new MusicService();
            keyService = new KeyService();
        }

        #region [Actions]
        private async Task ShowMusicDetailsAction()
        {
            if(music != null)
            {
                Name = music.Name;
                Author = music.Author;
                SelectedKey = music.Key;
                Lyrics = music.Lyrics;

                IsPlayMusicVisible = !string.IsNullOrEmpty(music.Version);

                await GetMusicKeys();
            }

            HasSingers = UserList.Any();
        }

        private void EditMusicAction()
        {
            if(music != null)
            {
                Navigation.PushAsync(new AddEditMusicPage(music));
            }
        }

        private async Task RemoveMusicAction()
        {
            if(music != null)
            {
                try
                {
                    var result = await Application.Current.MainPage.DisplayAlert(AppResources.AreYouShure, AppResources.AreYouShureSongRemoved, AppResources.Yes, AppResources.No);

                    if (result)
                    {
                        //await App.Database.DeleteMusic(music.Id);
                        await musicService.DeleteMusic(music);
                    }
                }
                catch (Exception)
                {
                    await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotRemoveSong, AppResources.Ok);
                }
            }

            await Navigation.PopAsync();
        }

        private void PlayMusicAction()
        {
            string url = music.Version;

            Launcher.OpenAsync(new Uri(url));
        }

        private async Task TutorialAction()
        {
            string message = string.Empty;
            message += "- " + AppResources.PreviewMusicPlayTutorial;
            message += "\n\n- " + AppResources.PreviewMusicKeyChangeTutorial;
            message += "\n\n- " + AppResources.PreviewMusicSingersKeysTutorial;
            message += "\n\n- " + AppResources.PreviewMusicEditTutorial;
            message += "\n\n- " + AppResources.PreviewMusicRemoveTutorial;
            message += "\n\n- " + AppResources.PreviewMusicSharedListTutorial;

            await Application.Current.MainPage.DisplayAlert(AppResources.Tutorial, message, AppResources.Ok);
        }
        #endregion

        #region [Private Methods]
        private async Task GetMusicKeys()
        {
            try
            {
                var usersToAdd = await keyService.GetKeysByOwner(LoggedUserHelper.GetEmail(), Name);
                
                UserList.Clear();
                usersToAdd.ForEach(i => UserList.Add(i));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.UnableToLoadKeys, AppResources.Ok);
            }
        }

        private void SetChords()
        {
            if (music != null)
            {
                if (string.IsNullOrEmpty(selectedKey))
                {
                    Chords = music.Chords;
                }
                else
                {
                    if (selectedKey.Equals(music.Key))
                    {
                        Chords = music.Chords;
                    }
                    else
                    {
                        Chords = Utils.GetChordsAccordingKey(music.Key, music.Chords, SelectedKey);
                    }
                }
            }
        }
        #endregion
    }
}
