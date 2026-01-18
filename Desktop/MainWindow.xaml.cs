using Desktop.Repository;
using Desktop.View;
using System.Windows;
using System.Windows.Navigation;
using Todo.Entities;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private UserRepository _userRepository;
        private int _currentUserId;
        private string _currentUserName;

        public MainWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();

            // Загружаем LoginPage при запуске
            LoadLoginPage();
        }

        private void LoadLoginPage()
        {
            var loginPage = new LoginPage();
            loginPage.LoginSuccess += OnLoginSuccess;
            loginPage.RegistrationRequested += OnRegistrationRequested;

            MainFrame.Navigate(loginPage);
        }

        private void OnLoginSuccess(object sender, (UserModel user, bool hasTasks) result)
        {
            // Сохраняем данные пользователя
            _currentUserId = result.user.Id;
            _currentUserName = result.user.Username;

            // Логика после входа
            if (!result.hasTasks)
            {
                var mainEmptyPage = new MainEmptyPage();
                mainEmptyPage.SetUserInfo(_currentUserName, _currentUserId);
                mainEmptyPage.CreateTaskRequested += OnCreateTaskRequested;
                MainFrame.Navigate(mainEmptyPage);
            }
            else
            {
                var mainPage = new MainPage();
                mainPage.SetUserName(_currentUserName);
                mainPage.LoadUserTasks(_currentUserId);
                mainPage.CreateTaskRequested += OnCreateTaskRequested;
                MainFrame.Navigate(mainPage);
            }
        }

        private void OnRegistrationRequested(object sender, System.EventArgs e)
        {
            var registrationPage = new RegistrationPage();
            registrationPage.RegistrationCompleted += OnRegistrationCompleted;
            MainFrame.Navigate(registrationPage);
        }

        private void OnRegistrationCompleted(object sender, bool success)
        {
            if (success)
            {
                MessageBox.Show("Регистрация успешна! Теперь войдите в систему.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Возвращаемся на страницу входа
                LoadLoginPage();
            }
            else
            {
                // Отмена регистрации - возвращаемся назад
                if (MainFrame.CanGoBack)
                {
                    MainFrame.GoBack();
                }
            }
        }

        private void OnCreateTaskRequested(object sender, System.EventArgs e)
        {
            var createTaskPage = new CreateTaskPage(_currentUserId, _currentUserName);
            createTaskPage.TaskCreated += OnTaskCreated;
            createTaskPage.CancelRequested += OnTaskCreationCanceled;
            MainFrame.Navigate(createTaskPage);
        }

        // ИСПРАВЛЕННЫЙ МЕТОД
        private void OnTaskCreated(object sender, System.EventArgs e)
        {
            // После создания задачи создаем новую главную страницу
            var mainPage = new MainPage();
            mainPage.SetUserName(_currentUserName);
            mainPage.LoadUserTasks(_currentUserId);
            mainPage.CreateTaskRequested += OnCreateTaskRequested;
            MainFrame.Navigate(mainPage);
        }

        private void OnTaskCreationCanceled(object sender, System.EventArgs e)
        {
            // Отмена создания задачи - возвращаемся назад
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }
    }
}