using System;
using System.Collections.Generic;
using System.Text;

namespace SongbookManager.Models
{
    public class Repertoire
    {
        public string Owner { get; set; }
        public List<Music> Musics { get; set; }
        public List<UserKey> Keys { get; set; }
        public string SingerName { get; set; }
        public string SingerEmail { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public string DateFormated
        {
            get
            {
                return Date.ToString("dd MMM");
            }
        }

        public string TimeFormated
        {
            get
            {
                return Time.ToString(@"hh\:mm");
            }
        }
    }
}
