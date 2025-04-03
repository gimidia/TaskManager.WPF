using System;
using System.Globalization;
using System.Windows.Data;

namespace TaskManager.WPF.Converters
{
    public class StatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                // Habilita o DatePicker quando o status for "Concluída" ou quando estiver adicionando uma nova tarefa
                return status == 2 || status == 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 