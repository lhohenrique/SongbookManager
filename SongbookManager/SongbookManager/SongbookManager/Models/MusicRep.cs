using System;
using System.Collections.Generic;
using System.Text;

namespace SongbookManager.Models
{
    public class MusicRep
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string SingerKey { get; set; }
        public bool IsSelected { get; set; }
        public bool IsReordering { get; set; }
    }
}
