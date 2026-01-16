using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Todo.Entities;
using Desktop.Repository;

namespace Desktop
{
    public partial class Registration : Window
    {
        private UserRepository _userRepository;

        public Registration()
        {
            InitializeComponent();
            _userRepository = new UserRepository();

            // Обработчики для placeholder
            Name.GotFocus += RemovePlaceholder;
            Name.LostFocus += AddPlaceholder;
            Mail.GotFocus += RemovePlaceholder;
            Mail.LostFocus += AddPlaceholder;
            Pass.GotFocus += RemovePlaceholder;
            Pass.LostFocus += AddPlaceholder;
            PassChange.GotFocus += RemovePlaceholder;
            PassChange.LostFocus += AddPlaceholder;

            AddPlaceholder(null, null);
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Foreground.ToString() == "#FFBBB9B9")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Name.Text))
            {
                Name.Text = "Введите имя пользователя";
                Name.Foreground = System.Windows.Media.Brushes.Gray;
            }

            if (string.IsNullOrWhiteSpace(Mail.Text))
            {
                Mail.Text = "exam@yandex.ru";
                Mail.Foreground = System.Windows.Media.Brushes.Gray;
            }

            if (string.IsNullOrWhiteSpace(Pass.Text))
            {
                Pass.Text = "Введите пароль";
                Pass.Foreground = System.Windows.Media.Brushes.Gray;
            }

            if (string.IsNullOrWhiteSpace(PassChange.Text))
            {
                PassChange.Text = "Повторите пароль";
                PassChange.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Регистрация
            StringBuilder errors = new StringBuilder();

            string username = Name.Foreground.ToString() == "#FFBBB9B9" ? "" : Name.Text.Trim();
            string email = Mail.Foreground.ToString() == "#FFBBB9B9" ? "" : Mail.Text.Trim();
            string password = Pass.Foreground.ToString() == "#FFBBB9B9" ? "" : Pass.Text;
            string confirmPassword = PassChange.Foreground.ToString() == "#FFBBB9B9" ? "" : PassChange.Text;

            // Валидация
            if (string.IsNullOrEmpty(username))
                errors.AppendLine("• Поле 'Имя' не может быть пустым");
            else if (username.Length < 3)
                errors.AppendLine("• Имя должно содержать минимум 3 символа");

            if (string.IsNullOrEmpty(email))
                errors.AppendLine("• Поле 'Почта' не может быть пустым");
            else if (!email.Contains("@") || !email.Contains(".") || email.Length < 5)
                errors.AppendLine("• Введите корректный email адрес");

            if (string.IsNullOrEmpty(password))
                errors.AppendLine("• Поле 'Пароль' не может быть пустым");
            else if (password.Length < 6)
                errors.AppendLine("• Пароль должен содержать минимум 6 символов");

            if (string.IsNullOrEmpty(confirmPassword))
                errors.AppendLine("• Подтвердите пароль");
            else if (password != confirmPassword)
                errors.AppendLine("• Пароли не совпадают");

            if (errors.Length > 0)
            {
                MessageBox.Show($"Обнаружены ошибки:\n\n{errors}", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Создаем пользователя и регистрируем
                var newUser = new UserModel
                {
                    Username = username,
                    Email = email,
                    Password = password
                };

                bool success = _userRepository.RegisterUser(newUser);

                if (success)
                {
                    MessageBox.Show($"Пользователь '{username}' успешно зарегистрирован!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Назад
            this.DialogResult = false;
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }
    }
}