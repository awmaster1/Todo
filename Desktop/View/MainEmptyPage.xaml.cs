using System;
using System.Windows;
using System.Windows.Controls;

namespace Desktop.View
{
    public partial class MainEmptyPage : Page
    {
        private int _userId;
        private string _userName;

        // Событие для создания задачи
        public event EventHandler CreateTaskRequested;

        public MainEmptyPage()
        {
            InitializeComponent();
        }

        public void SetUserInfo(string userName, int userId)
        {
            _userName = userName;
            _userId = userId;

            // Обновляем приветствие
            WelcomeLabel.Content = $"Добро пожаловать, {userName}!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Запрашиваем создание задачи
            CreateTaskRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}