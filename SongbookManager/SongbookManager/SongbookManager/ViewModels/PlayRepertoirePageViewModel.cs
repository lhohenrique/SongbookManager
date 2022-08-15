﻿using SongbookManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SongbookManager.ViewModels
{
    public class PlayRepertoirePageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private Repertoire repertoire;
        private int musicNumber = 0;
        private int musicCount;

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

        private string key;
        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Key"));
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
            }
        }
        #endregion

        #region [Command]
        public Command NextMusicCommand { get; set; }
        public Command PreviousMusicCommand { get; set; }
        #endregion

        public PlayRepertoirePageViewModel(INavigation navigation, Repertoire repertoire)
        {
            Navigation = navigation;
            this.repertoire = repertoire;

            NextMusicCommand = new Command(() => NextMusicAction());
            PreviousMusicCommand = new Command(() => PreviousMusicAction());
        }

        #region [Actions]
        public void NextMusicAction()
        {
            int nextMusic = musicNumber + 1;

            if (repertoire.Musics != null && nextMusic <= musicCount - 1)
            {
                Music music = repertoire.Musics[nextMusic];
                if (music != null)
                {
                    Name = music.Name;
                    Author = music.Author;
                    SelectedKey = music.Key;
                    Key = music.Key;
                    Lyrics = music.Lyrics;
                    Chords = music.Chords;
                }

                musicNumber = nextMusic;
            }
        }

        public void PreviousMusicAction()
        {
            int previousMusic = musicNumber - 1;

            if (repertoire.Musics != null && previousMusic >= 0)
            {
                Music music = repertoire.Musics[previousMusic];
                if (music != null)
                {
                    Name = music.Name;
                    Author = music.Author;
                    SelectedKey = music.Key;
                    Key = music.Key;
                    Lyrics = music.Lyrics;
                    Chords = music.Chords;
                }

                musicNumber = previousMusic;
            }
        }
        #endregion

        #region [Public Methods]
        public void LoadPage()
        {
            if(repertoire.Musics != null)
            {
                musicCount = repertoire.Musics.Count;

                Music music = repertoire.Musics.FirstOrDefault();
                
                if (music != null)
                {
                    Name = music.Name;
                    Author = music.Author;
                    SelectedKey = music.Key;
                    Key = music.Key;
                    Lyrics = music.Lyrics;
                    Chords = music.Chords;
                }
            }
        }
        #endregion

        #region [Private Methods]
        #endregion
    }
}