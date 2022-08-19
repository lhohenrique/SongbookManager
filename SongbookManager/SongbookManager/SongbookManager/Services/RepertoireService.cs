using Firebase.Database;
using Firebase.Database.Query;
using SongbookManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongbookManager.Services
{
    public class RepertoireService
    {
        FirebaseClient client;

        public RepertoireService()
        {
            client = new FirebaseClient("https://songbookmanagerlite-default-rtdb.firebaseio.com/");
        }

        public async Task<List<Repertoire>> GetRepertoires()
        {
            var repertoires = (await client.Child("Repertoires").OnceAsync<Repertoire>())
                .Select(item => new Repertoire
                {
                    Date = item.Object.Date,
                    Keys = item.Object.Keys,
                    Musics = item.Object.Musics,
                    Owner = item.Object.Owner,
                    SingerName = item.Object.SingerName,
                    SingerEmail = item.Object.SingerEmail,
                    Time = item.Object.Time
                }).ToList();

            return repertoires;
        }

        public async Task<List<Repertoire>> GetRepertoiresByUser(string owner)
        {
            var repertoires = (await client.Child("Repertoires").OnceAsync<Repertoire>()).Select(item => new Repertoire
            {
                Date = item.Object.Date,
                Keys = item.Object.Keys,
                Musics = item.Object.Musics,
                Owner = item.Object.Owner,
                SingerName = item.Object.SingerName,
                SingerEmail = item.Object.SingerEmail,
                Time = item.Object.Time
            }).Where(r => r.Owner.Equals(owner)).ToList();

            return repertoires;
        }

        public async Task<List<Repertoire>> GetRepertoiresBySinger(string singer)
        {
            var repertoires = (await client.Child("Repertoires").OnceAsync<Repertoire>()).Select(item => new Repertoire
            {
                Date = item.Object.Date,
                Keys = item.Object.Keys,
                Musics = item.Object.Musics,
                Owner = item.Object.Owner,
                SingerName = item.Object.SingerName,
                SingerEmail = item.Object.SingerEmail,
                Time = item.Object.Time
            }).Where(r => r.SingerEmail.Equals(singer)).ToList();

            return repertoires;
        }

        public async Task<bool> InsertRepertoire(Repertoire repertoire)
        {
            await client.Child("Repertoires").PostAsync(repertoire);

            return true;

        }

        public async Task UpdateRepertoire(Repertoire repertoire, DateTime oldDate, TimeSpan oldTime)
        {
            var repertoireToUpdate = (await client.Child("Repertoires").OnceAsync<Repertoire>())
                                                .Where(r => r.Object.Date.Equals(oldDate) && r.Object.Time.Equals(oldTime) && r.Object.Owner.Equals(repertoire.Owner)).FirstOrDefault();

            await client.Child("Repertoires").Child(repertoireToUpdate.Key).PutAsync(repertoire);
        }

        public async Task<List<Repertoire>> SearchRepertoire(string searchText, string owner)
        {
            var repertoires = (await client.Child("Repertoires").OnceAsync<Repertoire>()).Select(item => new Repertoire
            {
                Date = item.Object.Date,
                Keys = item.Object.Keys,
                Musics = item.Object.Musics,
                Owner = item.Object.Owner,
                SingerName = item.Object.SingerName,
                SingerEmail = item.Object.SingerEmail,
                Time = item.Object.Time
            }).Where(r => r.Owner.Equals(owner) &&
                          (r.Date.ToString("dd MMMM").ToUpper().Contains(searchText.ToUpper()) ||
                          r.SingerName.ToString().ToUpper().Contains(searchText.ToUpper()))).ToList();

            return repertoires;
        }

        public async Task DeleteRepertoire(Repertoire repertoire)
        {
            var repertoireToDelete = (await client.Child("Repertoires").OnceAsync<Repertoire>())
                                                .Where(r => r.Object.Date.Equals(repertoire.Date) && r.Object.Time.Equals(repertoire.Time) && r.Object.Owner.Equals(repertoire.Owner)).FirstOrDefault();

            await client.Child("Repertoires").Child(repertoireToDelete.Key).DeleteAsync();
        }

        public async Task DeleteAll()
        {
            await client.Child("Repertoires").DeleteAsync();
        }
    }
}
