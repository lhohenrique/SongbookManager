using System;
using System.Collections.Generic;
using System.Text;

namespace SongbookManager.Models
{
    public class Music
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Key { get; set; }
        public string Lyrics { get; set; }
        public string Chords { get; set; }
        public string Tipo { get; set; }
        public string Version { get; set; }
        public string Notes { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
