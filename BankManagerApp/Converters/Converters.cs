using BankManager.Data.Enums;
using System.Globalization;

namespace BankManagerApp.Converters
{
    public class TransactionIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type == TransactionType.Deposit ? "💰" : "💸";
            }
            return "❓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TransactionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type == TransactionType.Deposit ? Color.FromArgb("#2E7D32") : Color.FromArgb("#C62828");
            }
            return Color.FromArgb("#999");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PersianDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var pc = new System.Globalization.PersianCalendar();
                int year = pc.GetYear(dateTime);
                int month = pc.GetMonth(dateTime);
                int day = pc.GetDayOfMonth(dateTime);
                string hour = dateTime.Hour.ToString("00");
                string minute = dateTime.Minute.ToString("00");
                
                string[] monthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", 
                                       "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
                
                return $"{day} {monthNames[month - 1]} {year} - {hour}:{minute}";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
