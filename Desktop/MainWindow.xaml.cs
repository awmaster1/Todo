using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Todo.Entities;
using Desktop.Repository;
using Lab4;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private UserRepository _userRepository;

        public MainWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();

            // Обработчики для placeholder
            Mail.GotFocus += RemovePlaceholder;
            Mail.LostFocus += AddPlaceholder;
            Pass.GotFocus += RemovePlaceholder;
            Pass.LostFocus += AddPlaceholder;

            // Обработчик закрытия главного окна
            this.Closing += MainWindow_Closing;

            // Устанавливаем плейсхолдеры при запуске
            AddPlaceholder(null, null);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Проверяем, открыты ли другие окна
            if (Application.Current.Windows.Count <= 1)
            {
                Application.Current.Shutdown();
            }
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

                MessageBox.Show($"Добро пожаловать, {user.Username}!", "Успешный вход",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Открываем главное окно
                Main mainWindow = new Main();
                mainWindow.SetUserName(user.Username); // Устанавливаем имя пользователя
                mainWindow.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка входа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Registration registration = new Registration();
                registration.Owner = this;

                bool? result = registration.ShowDialog();

                if (result == true)
                {
                    MessageBox.Show("Теперь вы можете войти в систему", "Регистрация успешна",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) { }

      
    }
}