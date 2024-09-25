using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace The_Weather_Observatory.Converters
{
    public class TextToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string weather = value as string;
            string BackgroundColor = string.Empty;
            switch (weather)
            {
                case "01d":
                    BackgroundColor = "#2980B9";
                    break;
                case "02d":
                    BackgroundColor = "#2980B9";
                    break;
                case "03d":
                    BackgroundColor = "#C0C0C0";
                    break;
                case "04d":
                    BackgroundColor = "#C0C0C0";
                    break;
                case "09d":
                    BackgroundColor = "#898484";
                    break;
                case "10d":
                    BackgroundColor = "#898484";
                    break;
                case "11d":
                    BackgroundColor = "#606060";
                    break;
                case "13d":
                    BackgroundColor = "#C4E2F3";
                    break;
                case "50d":
                    BackgroundColor = "#E8F0F5";
                    break;
            }
            return BackgroundColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
