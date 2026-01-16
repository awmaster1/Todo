using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Entities;

namespace Desktop.Repository
{
    public class UserRepository
    {
        private static List<UserModel> _users = new List<UserModel>();
        private static int _nextId = 1;

        static UserRepository()
        {
            // Добавляем тестового пользователя для демонстрации
            _users.Add(new UserModel
            {
                Id = _nextId++,
                Username = "TestUser",
                Email = "test@example.com",
                Password = "123456",
                RegistrationDate = DateTime.Now,
                IsActive = true
            });
        }

        public bool RegisterUser(UserModel newUser)
        {
            try
            {
                // Проверка на уникальность email
                if (_users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("Пользователь с таким email уже существует");
                }

                // Проверка на уникальность имени пользователя
                if (_users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("Пользователь с таким именем уже существует");
                }

                // Присваиваем ID и сохраняем
                newUser.Id = _nextId++;
                newUser.RegistrationDate = DateTime.Now;
                newUser.IsActive = true;

                _users.Add(newUser);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UserModel Login(string email, string password)
        {
            try
            {
                // Находим пользователя
                var user = _users.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);

                if (user == null)
                {
                    throw new Exception("Неверный email или пароль");
                }

                if (!user.IsActive)
                {
                    throw new Exception("Пользователь заблокирован");
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Метод для проверки существует ли пользователь
        public bool UserExists(string email)
        {
            return _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}