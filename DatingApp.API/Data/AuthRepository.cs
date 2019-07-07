using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        public AuthRepository(DataContext DataContext)
        {
            this._dataContext = DataContext;
        }

        public DataContext _dataContext { get; }

        public async Task<User> Login(string userName, string password)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x=>x.Username == userName);

            if(user == null) 
            {
                return null;
            }
            if(!VerifyPasswordHash(password,user)){
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHash(string password, User user)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt)) 
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0;i< computedHash.Length; i++)
                {
                    if(computedHash[0] != user.PasswordHash[0]) return false;
                }
            }
            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            byte [] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _dataContext.Users.AddAsync(user);
            return user;
        }

        public async Task<bool> UserExists(string userName)
        {
            if(await _dataContext.Users.AnyAsync(a=>a.Username == userName))
                return true;
                
            return false;
        }
    }
}