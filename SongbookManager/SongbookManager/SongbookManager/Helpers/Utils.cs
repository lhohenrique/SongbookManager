using SongbookManager.Models;
using SongbookManager.Resx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SongbookManager.Helpers
{
    public class Utils
    {
        public static async Task SendRepertoire(Repertoire repertoire)
        {
            try
            {
                string date = repertoire.Date.ToString("dddd dd/MMM");
                
                string period = string.Empty;
                if(repertoire.Time.Hours < 12)
                {
                    period = GlobalVariables.MorningPeriod;
                }
                else if(repertoire.Time.Hours >= 12 && repertoire.Time.Hours < 18)
                {
                    period = GlobalVariables.AfternoonPeriod;
                }
                else
                {
                    period = GlobalVariables.NightPeriod;
                }

                string musicList = string.Empty;

                foreach (Music music in repertoire.Musics)
                {
                    musicList += "\n- " + music.Name;
                    if (!string.IsNullOrEmpty(music.Author))
                    {
                        musicList += " - " + music.Author;
                    }
                    if (!string.IsNullOrEmpty(music.Key))
                    {
                        string key = music.Key.Replace("#", "%23");
                        musicList += " (" + key + ")";
                    }
                }
                
                string message = "*" + GlobalVariables.SendRepMessageIntro + " " + date + ", " + period + "*" + "\n" + musicList;

                //string url = "https://api.whatsapp.com/send?phone=" + phoneNumberWithCountryCode + "&text=" + message;
                //Launcher.OpenAsync(new Uri(url));

                var uriString = "whatsapp://send?text=" + message;
                //uriString += "&text=" + message;

                await Launcher.OpenAsync(new Uri(uriString));
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.Error, AppResources.CouldNotSendRepertoire, AppResources.Ok);
            }
        }
    }
}
