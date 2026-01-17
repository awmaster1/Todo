using Desktop;
using Desktop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Todo.Entities;

namespace Lab4
{
    public partial class Main : Window
    {
        private List<UserTask> userTasks = new List<UserTask>();
        private UserTask selectedTask = null;
        private string currentCategory = "Все"; // Начинаем с "Все"
        private bool showHistory = false;
        private int currentUserId;
        private UserRepository _repository;

        public Main()
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

        private void RefreshTasks()
        {
            userTasks = _repository.GetUserTasks(currentUserId);
            DisplayTasks();
        }

        private void DisplayTasks()
        {
            TasksList.Children.Clear();

            // Фильтруем задачи
            var tasksToShow = userTasks.Where(t =>
                t.IsCompleted == showHistory && // История или активные
                (currentCategory == "Все" || t.Category == currentCategory) // Все или конкретная категория
            ).ToList();

            if (tasksToShow.Count == 0)
            {
                // Показываем сообщение, если задач нет
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
                // Создаем контейнер для задачи
                var taskBorder = new Border
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    Padding = new Thickness(10),
                    Background = task.IsCompleted ? Brushes.LightGray : Brushes.White,
                    CornerRadius = new CornerRadius(10),
                    Cursor = Cursors.Hand,
                    Tag = task
                };

                // Чередуем цвета фона
                if (TasksList.Children.Count % 2 == 1 && !task.IsCompleted)
                {
                    taskBorder.Background = new SolidColorBrush(Color.FromArgb(255, 238, 240, 255));
                }

                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

                // CheckBox
                var checkBox = new CheckBox
                {
                    Margin = new Thickness(0, 0, 10, 0),
                    IsChecked = task.IsCompleted,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsEnabled = !task.IsCompleted // Нельзя снять галочку с завершенных в режиме "История"
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

                // Текст задачи
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

                // Обработчик клика по задаче
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
                // Находим обновленную задачу в списке
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

        // Обработчики для переключения между "Задачи" и "История"
        private void TasksText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showHistory = false;

            // Подсветка активного пункта меню
            HighlightMenuItems(sender as TextBlock);

            DisplayTasks();

            // Сбрасываем выбранную задачу
            selectedTask = null;
            UpdateButtonsState(false);
        }

        private void HistoryText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showHistory = true;

            // Подсветка активного пункта меню
            HighlightMenuItems(sender as TextBlock);

            DisplayTasks();

            // Сбрасываем выбранную задачу
            selectedTask = null;
            UpdateButtonsState(false);
        }

        private void Category_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock categoryText)
            {
                // Если кликаем на уже выбранную категорию - сбрасываем на "Все"
                if (currentCategory == categoryText.Tag as string)
                {
                    currentCategory = "Все";
                    // Убираем подсветку со всех категорий
                    UnhighlightAllCategories();
                }
                else
                {
                    currentCategory = categoryText.Tag as string;
                    // Подсвечиваем выбранную категорию
                    HighlightSelectedCategory(categoryText);
                }

                DisplayTasks();

                // Сбрасываем выбранную задачу при смене категории
                selectedTask = null;
                UpdateButtonsState(false);
            }
        }

        private void HighlightMenuItems(TextBlock selectedItem)
        {
            // Находим все элементы меню в левой панели
            var leftPanel = ((Grid)Content).Children[0] as StackPanel;

            foreach (var child in leftPanel.Children)
            {
                if (child is TextBlock textBlock &&
                    (textBlock.Text == "Задачи" || textBlock.Text == "История"))
                {
                    if (textBlock == selectedItem)
                    {
                        textBlock.FontWeight = FontWeights.Bold;
                        textBlock.Foreground = Brushes.Black;
                    }
                    else
                    {
                        textBlock.FontWeight = FontWeights.Normal;
                        textBlock.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        private void HighlightSelectedCategory(TextBlock selectedCategory)
        {
            // Находим панель с категориями
            var categoriesPanel = ((Grid)Content).Children[1] as Grid;
            var stackPanel = categoriesPanel.Children[0] as StackPanel;

            // Сбрасываем подсветку у всех категорий
            foreach (var child in stackPanel.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.FontWeight = FontWeights.Normal;
                    textBlock.TextDecorations = null;
                }
            }

            // Подсвечиваем выбранную категорию
            selectedCategory.FontWeight = FontWeights.Bold;
            selectedCategory.TextDecorations = TextDecorations.Underline;
        }

        private void UnhighlightAllCategories()
        {
            var categoriesPanel = ((Grid)Content).Children[1] as Grid;
            var stackPanel = categoriesPanel.Children[0] as StackPanel;

            foreach (var child in stackPanel.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.FontWeight = FontWeights.Normal;
                    textBlock.TextDecorations = null;
                }
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

                    // Сбрасываем детали
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

        // Обработчик для кнопки "+" создания новой задачи
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTask createTaskWindow = new CreateTask(currentUserId, UserNameText.Text);
            createTaskWindow.Owner = this;
            createTaskWindow.TaskCreated += (s, args) =>
            {
                // Обновляем список задач после создания новой
                RefreshTasks();
            };
            createTaskWindow.ShowDialog();
        }
    }
}