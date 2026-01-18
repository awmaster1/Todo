using Desktop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Todo.Entities;

namespace Desktop.View
{
    public partial class MainPage : Page
    {
        private List<UserTask> userTasks = new List<UserTask>();
        private UserTask selectedTask = null;
        private string currentCategory = "Все";
        private bool showHistory = false;
        private int currentUserId;
        private UserRepository _repository;

        // Событие для создания новой задачи
        public event EventHandler CreateTaskRequested;

        public MainPage()
        {
            InitializeComponent();
            _repository = new UserRepository();
        }

        public void SetUserName(string userName)
        {
            UserNameText.Text = userName;
        }

        public void LoadUserTasks(int userId)
        {
            currentUserId = userId;
            RefreshTasks();
        }

        // Публичный метод
        public void RefreshTasks()
        {
            userTasks = _repository.GetUserTasks(currentUserId);
            DisplayTasks(); // Важно: вызывает DisplayTasks внутри
        }

        private void DisplayTasks()
        {
            TasksList.Children.Clear();

            // Фильтруем задачи
            var tasksToShow = userTasks.Where(t =>
                t.IsCompleted == showHistory &&
                (currentCategory == "Все" || t.Category == currentCategory)
            ).ToList();

            if (tasksToShow.Count == 0)
            {
                var noTasksText = new TextBlock
                {
                    Text = showHistory
                        ? "Нет завершенных задач"
                        : $"Нет задач в категории '{currentCategory}'",
                    FontSize = 16,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                TasksList.Children.Add(noTasksText);
                return;
            }

            foreach (var task in tasksToShow)
            {
                var taskBorder = new Border
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    Padding = new Thickness(10),
                    Background = task.IsCompleted ? Brushes.LightGray : Brushes.White,
                    CornerRadius = new CornerRadius(10),
                    Cursor = Cursors.Hand,
                    Tag = task
                };

                if (TasksList.Children.Count % 2 == 1 && !task.IsCompleted)
                {
                    taskBorder.Background = new SolidColorBrush(Color.FromArgb(255, 238, 240, 255));
                }

                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

                var checkBox = new CheckBox
                {
                    Margin = new Thickness(0, 0, 10, 0),
                    IsChecked = task.IsCompleted,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsEnabled = !task.IsCompleted
                };

                checkBox.Checked += (s, e) =>
                {
                    task.IsCompleted = true;
                    _repository.UpdateTask(task);
                    RefreshTasks();
                    UpdateSelectedTask();
                };

                checkBox.Unchecked += (s, e) =>
                {
                    task.IsCompleted = false;
                    _repository.UpdateTask(task);
                    RefreshTasks();
                    UpdateSelectedTask();
                };

                var textStack = new StackPanel();
                var titleText = new TextBlock
                {
                    Text = task.Title,
                    FontWeight = FontWeights.Bold,
                    Foreground = task.IsCompleted ? Brushes.Gray : Brushes.Black
                };

                var categoryText = new TextBlock
                {
                    Text = $"{task.Category} • {task.Time}",
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                textStack.Children.Add(titleText);
                textStack.Children.Add(categoryText);

                stackPanel.Children.Add(checkBox);
                stackPanel.Children.Add(textStack);

                taskBorder.Child = stackPanel;

                taskBorder.MouseLeftButtonDown += (s, e) =>
                {
                    selectedTask = task;
                    UpdateTaskDetails(task);
                    UpdateButtonsState(true);
                };

                TasksList.Children.Add(taskBorder);
            }
        }

        private void UpdateTaskDetails(UserTask task)
        {
            TaskTitle.Text = task.Title;
            TaskTime.Text = task.Time;
            TaskDate.Text = task.Date.ToString("dd MMMM yyyy");
            TaskDescription.Text = task.Description;
            CompleteButton.Content = task.IsCompleted ? "Возобновить" : "Готово";
        }

        private void UpdateSelectedTask()
        {
            if (selectedTask != null)
            {
                var updatedTask = userTasks.FirstOrDefault(t => t.Id == selectedTask.Id);
                if (updatedTask != null)
                {
                    selectedTask = updatedTask;
                    UpdateTaskDetails(updatedTask);
                }
            }
        }

        private void UpdateButtonsState(bool enabled)
        {
            CompleteButton.IsEnabled = enabled;
            DeleteButton.IsEnabled = enabled;
        }

        private void TasksText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showHistory = false;
            DisplayTasks();
            selectedTask = null;
            UpdateButtonsState(false);
        }

        private void HistoryText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showHistory = true;
            DisplayTasks();
            selectedTask = null;
            UpdateButtonsState(false);
        }

        private void Category_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock categoryText)
            {
                currentCategory = categoryText.Tag as string;
                DisplayTasks();
                selectedTask = null;
                UpdateButtonsState(false);
            }
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null)
            {
                selectedTask.IsCompleted = !selectedTask.IsCompleted;
                _repository.UpdateTask(selectedTask);
                RefreshTasks();
                UpdateSelectedTask();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null)
            {
                var result = MessageBox.Show($"Удалить задачу '{selectedTask.Title}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _repository.DeleteTask(selectedTask.Id, currentUserId);
                    userTasks.Remove(selectedTask);
                    DisplayTasks();

                    ResetTaskDetails();
                    UpdateButtonsState(false);
                    selectedTask = null;
                }
            }
        }

        private void ResetTaskDetails()
        {
            TaskTitle.Text = "Заголовок";
            TaskTime.Text = "18:00";
            TaskDate.Text = DateTime.Now.ToString("dd MMMM yyyy");
            TaskDescription.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing.";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTaskRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}