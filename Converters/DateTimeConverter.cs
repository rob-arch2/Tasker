using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var today = DateTime.Today;
                var tomorrow = DateTime.Today.AddDays(1);

                string dayPrefix = "";
                if (dateTime.Date == today)
                {
                    dayPrefix = "Today ";
                }
                else if (dateTime.Date == tomorrow)
                {
                    dayPrefix = "Tomorrow ";
                }

                return $"{dayPrefix}{dateTime:h:mmtt}".ToUpper();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
