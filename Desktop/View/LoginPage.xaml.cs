using Desktop.Repository;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Todo.Entities;

namespace Desktop.View
{
    public partial class LoginPage : Page
    {
        private UserRepository _userRepository;

        // События
        public event EventHandler<(UserModel user, bool hasTasks)> LoginSuccess;
        public event EventHandler RegistrationRequested;

        public LoginPage()
        {
            InitializeComponent();
            _userRepository = new UserRepository();

            // Обработчики для placeholder
            Mail.GotFocus += RemovePlaceholder;
            Mail.LostFocus += AddPlaceholder;
            Pass.GotFocus += RemovePlaceholder;
            Pass.LostFocus += AddPlaceholder;

            // Устанавливаем плейсхолдеры при запуске
            AddPlaceholder(null, null);
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Foreground.ToString() == "#FFC5BEBE")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Mail.Text))
            {
                Mail.Text = "pangcheo1210@gmail.com";
                Mail.Foreground = System.Windows.Media.Brushes.Gray;
            }

            if (string.IsNullOrWhiteSpace(Pass.Text))
            {
                Pass.Text = "Введите пароль";
                Pass.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Вход
            StringBuilder errors = new StringBuilder();

            string email = Mail.Foreground.ToString() == "#FFC5BEBE" ? "" : Mail.Text.Trim();
            string password = Pass.Foreground.ToString() == "#FFC5BEBE" ? "" : Pass.Text;

            // Валидация
            if (string.IsNullOrEmpty(email))
                errors.AppendLine("• Поле 'Почта' не может быть пустым");
            else if (!email.Contains("@") || !email.Contains(".") || email.Length < 5)
                errors.AppendLine("• Введите корректный email адрес");

            if (string.IsNullOrEmpty(password))
                errors.AppendLine("• Поле 'Пароль' не может быть пустым");
            else if (password.Length < 6)
                errors.AppendLine("• Пароль должен содержать минимум 6 символов");

            if (errors.Length > 0)
            {
                MessageBox.Show($"Обнаружены ошибки:\n\n{errors}", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Авторизация через репозиторий
                UserModel user = _userRepository.Login(email, password);

                // Проверяем, есть ли у пользователя задачи
                bool hasTasks = _userRepository.UserHasTasks(user.Id);

                // Вызываем событие успешного входа
                LoginSuccess?.Invoke(this, (user, hasTasks));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка входа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Регистрация - вызываем событие
            RegistrationRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}