using System.Globalization;
//using System.Diagnostics;

namespace SemesterProject.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {   
            //Debug.WriteLine($"setting button highlight to {value}");

            //if passed argument both is a bool, and is true
            if (value is bool b && b)
            {
                // Return a dynamic resource color for the true case
                return Application.Current.Resources["Accent"]; // Assuming "Primary" is a dynamic resource key
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}