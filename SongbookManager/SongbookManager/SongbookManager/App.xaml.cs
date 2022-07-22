using SongbookManager.Helpers;
using SongbookManager.Resx;
using SongbookManager.Views;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SongbookManager
{
    public partial class App : Application
    {
        private static SQLiteDataBaseHelper database;
        public static SQLiteDataBaseHelper Database
        {
            get
            {
                if(database == null)
                {
                    database = new SQLiteDataBaseHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XamSongbookManagerLite.db3"));
                }

                return database;
            }
        }


        public App()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            AppResources.Culture = CultureInfo.InstalledUICulture;

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
