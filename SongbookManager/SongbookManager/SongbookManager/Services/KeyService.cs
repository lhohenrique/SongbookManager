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
    public class KeyService
    {
        FirebaseClient client;

        public KeyService()
        {
            client = new FirebaseClient("https://songbookmanagerlite-default-rtdb.firebaseio.com/");
        }

        public async Task<bool> IsUserKeyExists(UserKey userKey)
        {
            var key = (await client.Child("Keys").OnceAsync<UserKey>()).Where(u => u.Object.UserEmail.Equals(userKey.UserEmail) && u.Object.MusicName.Equals(userKey.MusicName)).FirstOrDefault();

            return key != null;
        }

        public async Task InsertKey(UserKey key)
        {
            await client.Child("Keys").PostAsync(key);
        }

        public async Task<UserKey> GetKeyByUser(string userEmail, string musicName)
        {
            var key = (await client.Child("Keys").OnceAsync<UserKey>()).Select(item => new UserKey
            {
                Key = item.Object.Key,
                UserEmail = item.Object.UserEmail,
                UserName = item.Object.UserName,
                MusicName = item.Object.MusicName
            }).Where(k => k.UserEmail.Equals(userEmail) && k.MusicName.Equals(musicName)).FirstOrDefault();

            return key;
        }

        public async Task<List<UserKey>> GetKeysByOwner(string musicOwner, string musicName)
        {
            var keys = (await client.Child("Keys").OnceAsync<UserKey>()).Select(item => new UserKey
            {
                Key = item.Object.Key,
                UserEmail = item.Object.UserEmail,
                UserName = item.Object.UserName,
                MusicName = item.Object.MusicName,
                MusicOwner = item.Object.MusicOwner
            }).Where(k => k.MusicOwner.Equals(musicOwner) && k.MusicName.Equals(musicName)).ToList();

            return keys;
        }

        public async Task UpdateKey(UserKey key)
        {
            var keyToUpdate = (await client.Child("Keys").OnceAsync<UserKey>())
                                                .Where(k => k.Object.UserEmail.Equals(key.UserEmail) && k.Object.MusicName.Equals(key.MusicName)).FirstOrDefault();

            await client.Child("Keys").Child(keyToUpdate.Key).PutAsync(key);
        }

        public async Task RemoveUserKeyByMusic(string musicOwner, string musicName)
        {
            var keysToRemove = (await client.Child("Keys").OnceAsync<UserKey>())
                                                .Where(k => k.Object.MusicOwner.Equals(musicOwner) && k.Object.MusicName.Equals(musicName)).ToList();

            foreach (var key in keysToRemove)
            {
                await client.Child("Keys").Child(key.Key).DeleteAsync();
            }
        }

        public async Task ClearUserKeys(string userEmail)
        {
            var keysToRemove = (await client.Child("Keys").OnceAsync<UserKey>())
                                                .Where(k => k.Object.UserEmail.Equals(userEmail)).ToList();

            foreach(var key in keysToRemove)
            {
                await client.Child("Keys").Child(key.Key).DeleteAsync();
            }
        }
    }
}
