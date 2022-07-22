using System;
using System.Collections.Generic;
using System.Text;

namespace SongbookManager.Models
{
    public class Repertoire
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<Music> Musics { get; set; }
        public int Singer { get; set; }
        public DateTime Date { get; set; }
    }
}
