using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Add null check
            if (value == null)
                return Colors.Gray;

            var colorString = value.ToString();

            // Check if string is empty or whitespace
            if (string.IsNullOrWhiteSpace(colorString))
                return Colors.Gray;

            try
            {
                return Color.FromArgb(colorString);
            }
            catch
            {
                // If color parsing fails, return a default color
                return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}