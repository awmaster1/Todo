using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Lab4.Main;

namespace Lab4
{
    public partial class Main : Window
    {
        public class TaskItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string Time { get; set; }
            public string Category { get; set; }
            public bool IsCompleted { get; set; }
        }

        private List<TaskItem> allTasks = new List<TaskItem>();
        private List<TaskItem> filteredTasks = new List<TaskItem>();
        private TaskItem selectedTask = null;
        private string currentCategory = "Все";

        public Main()
        {
            InitializeComponent();
            LoadSampleTasks();
            DisplayTasks();
        }

        private void LoadSampleTasks()
        {
            allTasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Go fishing with Stephen",
                    Description = "Встретиться со Стивеном на рыбалке у озера. Взять снаряжение.",
                    Date = DateTime.Now.AddDays(1),
                    Time = "9:00 am",
                    Category = "Отдых",
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Go fishing with Stephen",
                    Description = "Встретиться со Стивеном на рыбалке у озера. Взять снаряжение.",
                    Date = DateTime.Now.AddDays(1),
                    Time = "9:00 am",
                    Category = "Отдых",
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Read the book Zlatan",
                    Description = "Прочитать главы 3-5 из книги Златана Ибрагимовича.",
                    Date = DateTime.Now.AddDays(2),
                    Time = "9:00 am",
                    Category = "Учеба",
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 4,
                    Title = "Meet according with design team...",
                    Description = "Встреча с командой дизайнеров для обсуждения нового проекта.",
                    Date = DateTime.Now.AddDays(3),
                    Time = "9:00 am",
                    Category = "Работа",
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 5,
                    Title = "Meet according with design team...",
                    Description = "Встреча с командой дизайнеров для обсуждения нового проекта.",
                    Date = DateTime.Now.AddDays(3),
                    Time = "9:00 pm",
                    Category = "Работа",
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 6,
                    Title = "Meet accordin with design team...",
                    Description = "Встреча с командой дизайнеров для обсуждения нового проекта.",
                    Date = DateTime.Now.AddDays(4),
                    Time = "9:00 am",
                    Category = "Работа",
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 7,
                    Title = "Убраться в квартире",
                    Description = "Пропылесосить, протереть пыль, помыть полы.",
                    Date = DateTime.Now,
                    Time = "14:00",
                    Category = "Дом",
                    IsCompleted = true
                }
            };

            filteredTasks = allTasks.ToList();
        }

        private void DisplayTasks()
        {
            TasksList.Children.Clear();

            foreach (var task in filteredTasks)
            {
                // Создаем контейнер для задачи
                var taskBorder = new Border
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    Padding = new Thickness(10),
                    Background = task.IsCompleted ? Brushes.LightGray : Brushes.White,
                    CornerRadius = new CornerRadius(10),
                    Cursor = Cursors.Hand
                };

                // Создаем горизонтальный StackPanel
                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

                // CheckBox для отметки выполнения
                var checkBox = new CheckBox
                {
                    Margin = new Thickness(0, 0, 10, 0),
                    IsChecked = task.IsCompleted,
                    VerticalAlignment = VerticalAlignment.Center
                };
                checkBox.Checked += (s, e) =>
                {
                    task.IsCompleted = true;
                    DisplayTasks();
                    if (selectedTask?.Id == task.Id)
                    {
                        CompleteButton.Content = "Возобновить";
                    }
                };
                checkBox.Unchecked += (s, e) =>
                {
                    task.IsCompleted = false;
                    DisplayTasks();
                    if (selectedTask?.Id == task.Id)
                    {
                        CompleteButton.Content = "Готово";
                    }
                };

                // Текст задачи
                var textStack = new StackPanel();
                var titleText = new TextBlock
                {
                    Text = task.Title,
                    FontWeight = FontWeights.Bold,
                    Foreground = task.IsCompleted ? Brushes.Gray : Brushes.Black
                };

                var timeText = new TextBlock
                {
                    Text = task.Time,
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                textStack.Children.Add(titleText);
                textStack.Children.Add(timeText);

                stackPanel.Children.Add(checkBox);
                stackPanel.Children.Add(textStack);

                taskBorder.Child = stackPanel;

                // Обработчик клика по задаче
                taskBorder.MouseLeftButtonDown += (s, e) =>
                {
                    SelectTask(task);
                };

                // Добавляем задачу в список
                TasksList.Children.Add(taskBorder);
            }
        }

        private void SelectTask(TaskItem task)
        {
            selectedTask = task;

            TaskTitle.Text = task.Title;
            TaskTime.Text = task.Time;
            TaskDate.Text = task.Date.ToString("dd MMMM yyyy");
            TaskDescription.Text = task.Description;

            CompleteButton.IsEnabled = true;
            DeleteButton.IsEnabled = true;
            CompleteButton.Content = task.IsCompleted ? "Возобновить" : "Готово";
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null)
            {
                selectedTask.IsCompleted = !selectedTask.IsCompleted;
                DisplayTasks();
                SelectTask(selectedTask); // Обновляем отображение
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
                    allTasks.Remove(selectedTask);

                    // Обновляем фильтрованный список
                    FilterTasksByCategory(currentCategory);

                    // Сбрасываем детали задачи
                    TaskTitle.Text = "Заголовок";
                    TaskTime.Text = "18:00";
                    TaskDate.Text = "01 Января 2022";
                    TaskDescription.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing.";

                    CompleteButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    selectedTask = null;
                }
            }
        }

        private void Category_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock categoryText)
            {
                string category = categoryText.Tag as string;
                currentCategory = category;

                // Визуально выделяем выбранную категорию
                foreach (var child in ((StackPanel)categoryText.Parent).Children)
                {
                    if (child is TextBlock tb)
                    {
                        tb.FontWeight = FontWeights.Normal;
                    }
                }
                categoryText.FontWeight = FontWeights.Bold;

                // Фильтруем задачи
                FilterTasksByCategory(category);
            }
        }

        private void FilterTasksByCategory(string category)
        {
            if (category == "Все")
            {
                filteredTasks = allTasks.ToList();
            }
            else
            {
                filteredTasks = allTasks.Where(t => t.Category == category).ToList();
            }

            DisplayTasks();
        }

        // Метод для установки имени пользователя (вызывается из окна входа)
        public void SetUserName(string userName)
        {
            UserNameText.Text = userName;
        }
    }
}