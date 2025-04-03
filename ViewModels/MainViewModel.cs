using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Linq;
using TaskManager.WPF.Models;
using TaskManager.WPF.Services;

namespace TaskManager.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        private TaskItem? _selectedTask;
        private bool _isLoading;
        private string _errorMessage;
        private int _selectedFilterStatus = 0; // 0 significa "Pendente"

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<TaskItem> Tasks { get; } = new();
        public ObservableCollection<TaskItem> FilteredTasks { get; } = new();

        public TaskItem EditingTask { get; private set; } = new()
        {
            Titulo = string.Empty,
            Descricao = string.Empty,
            Status = 0,
            DataCriacao = DateTime.Now
        };

        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
                if (value != null)
                {
                    StartEditing();
                }
            }
        }

        public int SelectedFilterStatus
        {
            get => _selectedFilterStatus;
            set
            {
                _selectedFilterStatus = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> TaskStatuses { get; } = new()
        {
            "Pendente",
            "Em Progresso",
            "Concluída"
        };

        public MainViewModel()
        {
            _taskService = new TaskService();
            LoadTasks();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplyFilter()
        {
            FilteredTasks.Clear();
            foreach (var task in Tasks.Where(t => t.Status == SelectedFilterStatus))
            {
                FilteredTasks.Add(task);
            }
        }

        public async void LoadTasks()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                var tasks = await _taskService.GetTasksAsync();
                Tasks.Clear();
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar tarefas: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void StartEditing()
        {
            if (SelectedTask != null)
            {
                EditingTask = new TaskItem
                {
                    Id = SelectedTask.Id,
                    Titulo = SelectedTask.Titulo,
                    Descricao = SelectedTask.Descricao,
                    Status = SelectedTask.Status,
                    DataCriacao = SelectedTask.DataCriacao,
                    DataConclusao = SelectedTask.DataConclusao
                };
                OnPropertyChanged(nameof(EditingTask));
            }
        }

        public void ClearEditingTask()
        {
            EditingTask = new TaskItem
            {
                Titulo = string.Empty,
                Descricao = string.Empty,
                Status = 0,
                DataCriacao = DateTime.Now,
                DataConclusao = null
            };
            SelectedTask = null;
            OnPropertyChanged(nameof(EditingTask));
        }

        public async void AddTask()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditingTask.Titulo))
                {
                    ErrorMessage = "O título da tarefa não pode ser vazio.";
                    MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                var taskToAdd = new TaskItem
                {
                    Titulo = EditingTask.Titulo,
                    Descricao = EditingTask.Descricao,
                    Status = EditingTask.Status,
                    DataCriacao = DateTime.Now,
                    DataConclusao = EditingTask.Status == 2 ? EditingTask.DataConclusao : null
                };

                var newTask = await _taskService.AddTaskAsync(taskToAdd);
                Tasks.Add(newTask);
                ApplyFilter();
                ClearEditingTask();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao adicionar a tarefa: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void UpdateTask()
        {
            try
            {
                if (SelectedTask == null)
                {
                    ErrorMessage = "Nenhuma tarefa selecionada para atualizar.";
                    MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingTask.Titulo))
                {
                    ErrorMessage = "O título da tarefa não pode ser vazio.";
                    MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                var taskToUpdate = new TaskItem
                {
                    Id = EditingTask.Id,
                    Titulo = EditingTask.Titulo,
                    Descricao = EditingTask.Descricao,
                    Status = EditingTask.Status,
                    DataCriacao = EditingTask.DataCriacao,
                    DataConclusao = EditingTask.Status == 2 ? EditingTask.DataConclusao : null
                };

                var updatedTask = await _taskService.UpdateTaskAsync(taskToUpdate);
                var index = Tasks.IndexOf(SelectedTask);
                if (index != -1)
                {
                    Tasks[index] = updatedTask;
                }
                ApplyFilter();
                ClearEditingTask();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao atualizar a tarefa: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ErrorMessage += $"\nDetalhes: {ex.InnerException.Message}";
                }
                MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void DeleteTask()
        {
            try
            {
                if (SelectedTask == null)
                {
                    ErrorMessage = "Nenhuma tarefa selecionada para excluir.";
                    MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Tem certeza que deseja excluir esta tarefa?",
                    "Confirmar exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    IsLoading = true;
                    ErrorMessage = string.Empty;
                    await _taskService.DeleteTaskAsync(SelectedTask.Id);
                    Tasks.Remove(SelectedTask);
                    ApplyFilter();
                    ClearEditingTask();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao excluir a tarefa: {ex.Message}";
                MessageBox.Show(ErrorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
