using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace CyberSecurityBotAwareness
{
    public partial class TaskManagerWindow : Window
    {
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=cybersecurity_bot;Trusted_Connection=True;";

        private string userName;
        private DateTime? selectedReminderDate = null;

        public TaskManagerWindow(string userName)
        {
            InitializeComponent();
            this.userName = userName;
            InitialiseDatabase();
            LoadTasks();
        }

        #region Database Setup

        private void InitialiseDatabase()
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string createTable = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tasks' AND xtype='U')
                        CREATE TABLE tasks (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title VARCHAR(200) NOT NULL,
                            Description VARCHAR(500),
                            ReminderDate DATE NULL,
                            Status VARCHAR(50) DEFAULT 'Pending',
                            CreatedAt DATETIME DEFAULT GETDATE()
                        );";
                    using (var cmd = new SqlCommand(createTable, conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection failed.\n\nError: {ex.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Load Tasks

        private void LoadTasks()
        {
            var tasks = new List<CyberTask>();
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Description, ReminderDate, Status FROM tasks ORDER BY CreatedAt DESC;";
                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new CyberTask
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                ReminderDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                Status = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ActionStatus.Text = $"Could not load tasks: {ex.Message}";
            }
            TaskListView.ItemsSource = tasks;
        }

        #endregion

        #region Add Task

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string description = DescriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                FormStatus.Text = "⚠️ Please enter a task title.";
                FormStatus.Foreground = System.Windows.Media.Brushes.Orange;
                return;
            }

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string insert = @"INSERT INTO tasks (Title, Description, ReminderDate, Status)
                                      VALUES (@title, @desc, @reminder, 'Pending');";
                    using (var cmd = new SqlCommand(insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@reminder",
                            selectedReminderDate.HasValue ? (object)selectedReminderDate.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                string logEntry = $"Task added: '{title}'";
                if (selectedReminderDate.HasValue)
                    logEntry += $" (Reminder: {selectedReminderDate.Value:dd MMM yyyy})";
                MainWindow.AddToActivityLog(logEntry);

                FormStatus.Text = $"✅ Task '{title}' added successfully!";
                FormStatus.Foreground = System.Windows.Media.Brushes.LimeGreen;

                TitleBox.Clear();
                DescriptionBox.Clear();
                ReminderCheckBox.IsChecked = false;
                ReminderPanel.Visibility = Visibility.Collapsed;
                selectedReminderDate = null;

                LoadTasks();
            }
            catch (Exception ex)
            {
                FormStatus.Text = $"❌ Error: {ex.Message}";
                FormStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        #endregion

        #region Complete & Delete

        private void MarkCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = TaskListView.SelectedItem as CyberTask;
            if (selected == null) { ActionStatus.Text = "⚠️ Please select a task first."; return; }
            if (selected.Status == "Completed") { ActionStatus.Text = "ℹ️ Already completed."; return; }

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string update = "UPDATE tasks SET Status = 'Completed' WHERE Id = @id;";
                    using (var cmd = new SqlCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                MainWindow.AddToActivityLog($"Task marked complete: '{selected.Title}'.");
                ActionStatus.Text = $"✅ '{selected.Title}' marked as completed!";
                LoadTasks();
            }
            catch (Exception ex) { ActionStatus.Text = $"❌ Error: {ex.Message}"; }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = TaskListView.SelectedItem as CyberTask;
            if (selected == null) { ActionStatus.Text = "⚠️ Please select a task first."; return; }

            var confirm = MessageBox.Show($"Delete task '{selected.Title}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string delete = "DELETE FROM tasks WHERE Id = @id;";
                    using (var cmd = new SqlCommand(delete, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                MainWindow.AddToActivityLog($"Task deleted: '{selected.Title}'.");
                ActionStatus.Text = $"🗑️ '{selected.Title}' deleted.";
                LoadTasks();
            }
            catch (Exception ex) { ActionStatus.Text = $"❌ Error: {ex.Message}"; }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
            ActionStatus.Text = "🔄 Tasks refreshed.";
        }

        #endregion

        #region Reminder Helpers

        private void ReminderCheckBox_Checked(object sender, RoutedEventArgs e)
            => ReminderPanel.Visibility = Visibility.Visible;

        private void ReminderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReminderPanel.Visibility = Visibility.Collapsed;
            selectedReminderDate = null;
        }

        private void ReminderDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
            => selectedReminderDate = ReminderDatePicker.SelectedDate;

        #endregion
    }

    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public string Status { get; set; } = "";
        public string ReminderDisplay =>
            ReminderDate.HasValue ? ReminderDate.Value.ToString("dd MMM yyyy") : "None";
    }
}