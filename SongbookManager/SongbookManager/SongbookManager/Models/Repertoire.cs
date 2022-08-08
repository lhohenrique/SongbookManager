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
        public int SingerName { get; set; }
        public int SingerEmail { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
