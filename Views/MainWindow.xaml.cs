using System.Windows;
using System.Windows.Controls;
using TaskManager.WPF.ViewModels;

namespace TaskManager.WPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddTask();
        }

        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateTask();
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteTask();
        }

        private void ClearTask_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearEditingTask();
        }

        private void OnTaskSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // A seleção já é tratada pelo binding no ViewModel
        }
    }
}
