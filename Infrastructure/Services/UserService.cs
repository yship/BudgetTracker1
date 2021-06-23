using ApplicationCore.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAsyncRepository<Expenditure> _expendRepository;
        private readonly IAsyncRepository<Income> _incomeRepository;
        public UserService(IUserRepository userRepository, IAsyncRepository<Expenditure> expendRepository, IAsyncRepository<Income> IncomeRepository) {
            _userRepository = userRepository;
            _incomeRepository = IncomeRepository;
            _expendRepository = expendRepository;
        }
        public async Task<UserLoginResponseModel> ValidateUser(string email, string password) {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null) return null;
            var isSuccess = user.HashedPassword == this.CreateHashedPassword(password,user.Salt);
            var response = new UserLoginResponseModel();
            response.Email = user.Email;
            response.FullName = user.FullName;
            response.Id = user.Id;
            response.Incomes = user.Incomes;
            response.Expenses = user.Expenses;
            return isSuccess ? response: null;

        }
        public async Task<UserRegisterResponseModel> CreateUser(UserRegisterRequestModel requestModel) {
            var dbUser = await this.GetUser(requestModel.Email);
            if (dbUser != null &&
                string.Equals(dbUser.Email, requestModel.Email, StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("Email Already Exits");
            var salt = CreateSalt();
            var hashedPswd = CreateHashedPassword(requestModel.Password, salt);
            var user = new Users { 
                Email = requestModel.Email, 
                Salt = salt, 
                HashedPassword = hashedPswd, 
                FullName = requestModel.FirstName+" "+requestModel.LastName, 
                Joinedon = DateTime.Now,
                Password = requestModel.Password
                
            };
            var newUser = await _userRepository.AddAsync(user);
            var response = new UserRegisterResponseModel(){
                FullName = newUser.FullName,
                Email = newUser.Email,
                Joinedon =newUser.Joinedon,
                Id=newUser.Id
            };
            return response;
        }

        private string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private string CreateHashedPassword(string password, string salt)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));
            return hashed;
        }

        public async Task<Users> GetUser(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<UserRegisterResponseModel> GetUserDetails(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            var response = new UserRegisterResponseModel() { 
             Email = user.Email,
             FullName = user.FullName,
             Id = user.Id,
             Joinedon = user.Joinedon

            };
            return response;
        }





    }
}
