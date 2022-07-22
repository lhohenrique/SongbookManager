using SongbookManager.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SongbookManager.Helpers
{
    public class SQLiteDataBaseHelper
    {
        readonly SQLiteAsyncConnection _db;

        public SQLiteDataBaseHelper(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);

            _db.CreateTableAsync<Music>().Wait();
            _db.CreateTableAsync<User>().Wait();
        }

        #region [Music Methods]
        public Task<List<Music>> GetAllMusics()
        {
            return _db.Table<Music>().OrderByDescending(i => i.Id).ToListAsync();
        }

        public Task<Music> GetMusicById(int id)
        {
            return _db.Table<Music>().FirstAsync(i => i.Id == id);
        }

        public Task<int> InsertMusic(Music music)
        {
            return _db.InsertAsync(music);
        }

        public Task<List<Music>> UpdateMusic(Music music)
        {
            string sql = "UPDATE Music SET Name=?, Author=?, Key=?, Lyrics=?, Chords=? WHERE Id=?";

            return _db.QueryAsync<Music>(
                sql,
                music.Name,
                music.Author,
                music.Key,
                music.Lyrics,
                music.Chords,
                music.Id
            );
        }

        public Task<int> DeleteMusic(int id)
        {
            return _db.Table<Music>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Music>> SearchMusic(string searchText)
        {
            string sql = "SELECT * FROM Music WHERE NAME LIKE '%" + searchText + "%'";

            return _db.QueryAsync<Music>(sql);
        }

        public Task<int> DeleteAll()
        {
            return _db.Table<Music>().DeleteAsync(i => i.Id >= 0);
        }
        #endregion

        #region [User Methods]
        //public Task<bool> IsUserExists(string email)
        //{
        //    return _db.Table<User>().FirstAsync(i => i.Email.Equals(email)) != null;
        //}
        public Task<int> RegisterUser(User user)
        {
            return _db.InsertAsync(user);
        }
        
        public Task<User>LoginUser(string email, string password)
        {
            return _db.Table<User>().FirstOrDefaultAsync(i => i.Email.Equals(email) && i.Password.Equals(password));
        }

        public Task<User> GetUserById(int id)
        {
            return _db.Table<User>().FirstOrDefaultAsync(i => i.Id == id);
        }
        #endregion
    }
}
