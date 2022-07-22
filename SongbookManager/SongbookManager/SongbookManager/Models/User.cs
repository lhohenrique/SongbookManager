using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SongbookManager.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SharedList { get; set; }
        public bool IsSinger { get; set; }
    }
}
