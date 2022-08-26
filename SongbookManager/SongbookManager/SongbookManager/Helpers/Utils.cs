using SongbookManager.Models;
using SongbookManager.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SongbookManager.Helpers
{
    public class Utils
    {
        private const string sustenido = "#";
        private const string bemol = "b";

        private static Dictionary<string, int> chordsTable = new Dictionary<string, int>()
        {
            { "C", 1 },
            { "C#", 2 },
            { "Db", 2 },
            { "D", 3 },
            { "D#", 4 },
            { "Eb", 4 },
            { "E", 5 },
            { "F", 6 },
            { "F#", 7 },
            { "Gb", 7 },
            { "G", 8 },
            { "G#", 9  },
            { "Ab", 9  },
            { "A", 10 },
            { "A#", 11 },
            { "Bb", 11 },
            { "B", 12 }
        };

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

                foreach (MusicRep music in repertoire.Musics)
                {
                    musicList += "\n- " + music.Name;
                    if (!string.IsNullOrEmpty(music.Author))
                    {
                        musicList += " - " + music.Author;
                    }
                    if (!string.IsNullOrEmpty(music.SingerKey))
                    {
                        string key = music.SingerKey.Replace("#", "%23");
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

        public static string GetChordsAccordingKey(string originalKey, string originalChords, string newKey)
        {
            string newChords = string.Empty;

            if (!string.IsNullOrEmpty(originalKey) && !string.IsNullOrEmpty(newKey) && !string.IsNullOrEmpty(originalChords))
            {
                string newChord = string.Empty;
                int originalKeyId = GetChordId(originalKey);
                int newKeyId = GetChordId(newKey);
                int keyDelta = newKeyId - originalKeyId;

                for (int i = 0; i < originalChords.Length; i++)
                {
                    string chord = originalChords[i].ToString();
                    if (IsChord(chord))
                    {
                        // Check next position is valid
                        if (i + 1 < originalChords.Length)
                        {
                            // Get next character to check if is a chord with sustenido or bemol
                            string nextChord = originalChords[i + 1].ToString();
                            if (nextChord.Equals(sustenido) || nextChord.Equals(bemol))
                            {
                                chord += nextChord;

                                newChord = CalculateNewChord(chord, keyDelta);

                                newChords += newChord;

                                i++;
                            }
                            else
                            {
                                newChord = CalculateNewChord(chord, keyDelta);

                                newChords += newChord;
                            }
                        }
                        else // Last chord
                        {
                            newChord = CalculateNewChord(chord, keyDelta);

                            newChords += newChord;
                        }
                    }
                    else // If is not a chord just add to new chords text
                    {
                        newChords += chord;
                    }
                }
            }
            
            return newChords;
        }

        private static bool IsChord(string text)
        {
            return text.Equals("C") ||
                   text.Equals("D") ||
                   text.Equals("E") ||
                   text.Equals("F") ||
                   text.Equals("G") ||
                   text.Equals("A") ||
                   text.Equals("B");
        }

        private static string CalculateNewChord(string chord, int keyDelta)
        {
            int chordId = GetChordId(chord);
            int newChordId = chordId + keyDelta;

            if (newChordId > 12)
            {
                newChordId -= 12;
            }
            else if (newChordId < 1)
            {
                newChordId += 12;
            }

            return GetChordById(newChordId);
        }

        private static int GetChordId(string chord)
        {
            return chordsTable[chord];
        }

        private static string GetChordById(int chordId)
        {
            return chordsTable.FirstOrDefault(c => c.Value.Equals(chordId)).Key;
            
        }
    }
}
