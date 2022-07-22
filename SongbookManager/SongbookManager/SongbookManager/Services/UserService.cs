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
    public class UserService
    {
        FirebaseClient client;

        public UserService()
        {
            client = new FirebaseClient("https://songbookmanagerlite-default-rtdb.firebaseio.com/");
        }

        public async Task<bool> IsUserExists(string email)
        {
            var user = (await client.Child("Users").OnceAsync<User>()).Where(u => u.Object.Email == email).FirstOrDefault();

            return user != null;
        }

        public async Task<bool> RegisterUSer(string name, string email, string password)
        {
            if(await IsUserExists(email) == false)
            {
                await client.Child("Users")
                    .PostAsync(new User()
                    { 
                        Name = name,
                        Email = email,
                        Password = password
                    });
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> LoginUser(string email, string password)
        {
            var user = (await client.Child("Users").OnceAsync<User>()).Where(u => u.Object.Email == email && u.Object.Password == password).FirstOrDefault();

            return user != null;
        }

        public async Task<User> GetUser(string email)
        {
            var user = (await client.Child("Users").OnceAsync<User>()).Select(item => new User
            {
                Name = item.Object.Name,
                Email = item.Object.Email,
                Password = item.Object.Password,
                SharedList = item.Object.SharedList,
                IsSinger = item.Object.IsSinger
            }).Where(u => u.Email.Equals(email)).FirstOrDefault();

            return user;
        }

        public async Task<List<User>> GetSharedUsers(string email)
        {
            var users = (await client.Child("Users").OnceAsync<User>()).Select(item => new User
            {
                Name = item.Object.Name,
                Email = item.Object.Email,
                Password = item.Object.Password,
                SharedList = item.Object.SharedList,
                IsSinger = item.Object.IsSinger
            }).Where(u => !string.IsNullOrEmpty(u.SharedList) && u.SharedList.Equals(email)).ToList();

            return users;
        }

        public async Task<List<User>> GetSingers(string email)
        {
            var users = (await client.Child("Users").OnceAsync<User>()).Select(item => new User
            {
                Name = item.Object.Name,
                Email = item.Object.Email,
                Password = item.Object.Password,
                SharedList = item.Object.SharedList,
                IsSinger = item.Object.IsSinger
            }).Where(u => u.IsSinger && (u.Email.Equals(email) || !string.IsNullOrEmpty(u.SharedList) && u.SharedList.Equals(email))).ToList();

            return users;
        }

        public async Task UpdateUser(User user)
        {
            var userToUpdate = (await client.Child("Users").OnceAsync<User>())
                                                .Where(m => m.Object.Email.Equals(user.Email)).FirstOrDefault();

            await client.Child("Users").Child(userToUpdate.Key).PutAsync(user);
        }
    }
}
