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
    public class MusicService
    {
        FirebaseClient client;

        public MusicService()
        {
            client = new FirebaseClient("https://songbookmanagerlite-default-rtdb.firebaseio.com/");
        }

        public async Task<List<Music>> GetMusics()
        {
            var musics = (await client.Child("Musics").OnceAsync<Music>())
                .Select(item => new Music
                {
                    Name = item.Object.Name,
                    Author = item.Object.Author,
                    Key = item.Object.Key,
                    Lyrics = item.Object.Lyrics,
                    Chords = item.Object.Chords,
                    Owner = item.Object.Owner,
                    Version = item.Object.Version,
                    CreationDate = item.Object.CreationDate
                }).ToList();

            return musics;
        }

        public async Task<List<Music>> GetMusicsByUser(string userEmail)
        {
            var musics = (await client.Child("Musics").OnceAsync<Music>()).Select(item => new Music
            {
                Name = item.Object.Name,
                Author = item.Object.Author,
                Key = item.Object.Key,
                Lyrics = item.Object.Lyrics,
                Chords = item.Object.Chords,
                Owner = item.Object.Owner,
                Version = item.Object.Version,
                CreationDate = item.Object.CreationDate
            }).Where(m => m.Owner.Equals(userEmail)).ToList();

            return musics;
        }

        public async Task<Music> GetMusicByNameAndAuthor(string name, string author, string owner)
        {
            var music = (await client.Child("Musics").OnceAsync<Music>()).Select(item => new Music
            {
                Name = item.Object.Name,
                Author = item.Object.Author,
                Key = item.Object.Key,
                Lyrics = item.Object.Lyrics,
                Chords = item.Object.Chords,
                Owner = item.Object.Owner,
                Version = item.Object.Version,
                CreationDate = item.Object.CreationDate
            }).Where(m => m.Owner.Equals(owner) && m.Name.Equals(name) && m.Author.Equals(author)).FirstOrDefault();

            return music;
        }

        public async Task<bool> InsertMusic(Music music)
        {
            await client.Child("Musics").PostAsync(music);

            return true;
            
        }

        public async Task UpdateMusic(Music music, string oldName)
        {
            var musicToUpdate = (await client.Child("Musics").OnceAsync<Music>())
                                                .Where(m => m.Object.Name.Equals(oldName) && m.Object.Owner.Equals(music.Owner)).FirstOrDefault();

            await client.Child("Musics").Child(musicToUpdate.Key).PutAsync(music);
        }

        public async Task DeleteMusic(Music music)
        {
            var musicToDelete = (await client.Child("Musics").OnceAsync<Music>())
                                                .Where(m => m.Object.Name.Equals(music.Name) && m.Object.Owner.Equals(music.Owner)).FirstOrDefault();

            await client.Child("Musics").Child(musicToDelete.Key).DeleteAsync();
        }

        public async Task<List<Music>> SearchMusic(string searchText, string userEmail)
        {
            var musics = (await client.Child("Musics").OnceAsync<Music>()).Select(item => new Music
            {
                Name = item.Object.Name,
                Author = item.Object.Author,
                Key = item.Object.Key,
                Lyrics = item.Object.Lyrics,
                Chords = item.Object.Chords,
                Owner = item.Object.Owner,
                Version = item.Object.Version,
                CreationDate = item.Object.CreationDate
            }).Where(m => m.Owner.Equals(userEmail) && m.Name.ToUpper().Contains(searchText.ToUpper())).ToList();

            return musics;
        }

        public async Task DeleteAll()
        {
            await client.Child("Musics").DeleteAsync();
        }
    }
}
