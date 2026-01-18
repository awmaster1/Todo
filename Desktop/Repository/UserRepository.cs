using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Entities;

namespace Desktop.Repository
{
    public class UserRepository
    {
        private static List<UserModel> _users = new List<UserModel>();
        private static List<UserTask> _tasks = new List<UserTask>();
        private static int _nextUserId = 1;
        private static int _nextTaskId = 1;

        static UserRepository()
        {
            // Добавляем тестового пользователя для демонстрации
            _users.Add(new UserModel
            {
                Id = _nextUserId++,
                Username = "TestUser",
                Email = "pangcheo1210@gmail.com",
                Password = "123456",
                RegistrationDate = DateTime.Now,
                IsActive = true
            });

            // ЗАКОММЕНТИРОВАНО: тестовая задача удалена
            // чтобы новые пользователи попадали на MainEmptyPage
            /*
            _tasks.Add(new UserTask
            {
                Id = _nextTaskId++,
                UserId = 1,
                Title = "Пример задачи",
                Description = "Это пример задачи",
                Date = DateTime.Now,
                Time = "10:00",
                Category = "Работа",
                IsCompleted = false,
                CreatedAt = DateTime.Now
            });
            */
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
                newUser.Id = _nextUserId++;
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

        // ===== МЕТОДЫ ДЛЯ РАБОТЫ С ЗАДАЧАМИ =====

        // Получить все задачи пользователя
        public List<UserTask> GetUserTasks(int userId)
        {
            return _tasks.Where(t => t.UserId == userId).ToList();
        }

        // Проверить, есть ли у пользователя задачи
        public bool UserHasTasks(int userId)
        {
            return _tasks.Any(t => t.UserId == userId);
        }

        // Создать новую задачу
        public UserTask CreateTask(UserTask task)
        {
            try
            {
                task.Id = _nextTaskId++;
                task.CreatedAt = DateTime.Now;
                _tasks.Add(task);
                return task;
            }
            catch (Exception)
            {
                throw new Exception("Ошибка при создании задачи");
            }
        }

        // Обновить задачу
        public bool UpdateTask(UserTask task)
        {
            try
            {
                var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id && t.UserId == task.UserId);
                if (existingTask != null)
                {
                    existingTask.Title = task.Title;
                    existingTask.Description = task.Description;
                    existingTask.Date = task.Date;
                    existingTask.Time = task.Time;
                    existingTask.Category = task.Category;
                    existingTask.IsCompleted = task.IsCompleted;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw new Exception("Ошибка при обновлении задачи");
            }
        }

        // Удалить задачу
        public bool DeleteTask(int taskId, int userId)
        {
            try
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId);
                if (task != null)
                {
                    _tasks.Remove(task);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw new Exception("Ошибка при удалении задачи");
            }
        }

        // Получить выполненные задачи пользователя
        public List<UserTask> GetCompletedTasks(int userId)
        {
            return _tasks.Where(t => t.UserId == userId && t.IsCompleted).ToList();
        }

        // Получить невыполненные задачи пользователя
        public List<UserTask> GetPendingTasks(int userId)
        {
            return _tasks.Where(t => t.UserId == userId && !t.IsCompleted).ToList();
        }

        // Получить задачи по категории
        public List<UserTask> GetTasksByCategory(int userId, string category)
        {
            if (category == "Все")
            {
                return GetUserTasks(userId);
            }
            return _tasks.Where(t => t.UserId == userId && t.Category == category).ToList();
        }

        // Получить задачу по ID
        public UserTask GetTaskById(int taskId, int userId)
        {
            return _tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId);
        }

        // Новый метод: получить пользователя по ID
        public UserModel GetUserById(int userId)
        {
            return _users.FirstOrDefault(u => u.Id == userId);
        }

        // Новый метод: получить всех пользователей (для отладки)
        public List<UserModel> GetAllUsers()
        {
            return _users.ToList();
        }
    }
}