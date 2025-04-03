using System.Configuration;
using System.Data;
using System.Windows;
using System;

namespace TaskManager.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erro não tratado: {args.ExceptionObject}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erro não tratado: {args.Exception.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        base.OnStartup(e);
    }
}

