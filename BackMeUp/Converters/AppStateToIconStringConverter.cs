using System;
using System.Globalization;
using System.Windows.Data;

namespace BackMeUp.Converters
{
    public class AppStateToIconStringConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Enum.TryParse(value?.ToString(), out AppState appState))
            {
                return "/Icons/BackMeUp_red.ico";
            }

            string icon;
            switch (appState)
            {
                case AppState.Nothing:
                    icon = "/Icons/BackMeUp_white.ico";
                    break;

                case AppState.BackedUp:
                    icon = "/Icons/BackMeUp_blue.ico";
                    break;

                case AppState.Error:
                    icon = "/Icons/BackMeUp_red.ico";
                    break;

                default:
                    icon = "/Icons/BackMeUp_white.ico";
                    break;
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}