using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace The_Weather_Observatory.Converters
{
    public class TextToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string weather = value as string;
            string path = string.Empty;
            switch (weather)
            {
                case "01d":
                    path = "ClearSky";
                    break;
                case "02d":
                    path = "FewClouds";
                    break;
                case "03d":
                    path = "Clouds";
                    break;
                case "04d":
                    path = "BrokenClouds";
                    break;
                case "09d":
                    path = "ShowerRain";
                    break;
                case "10d":
                    path = "Rain";
                    break;
                case "11d":
                    path = "Thunderstorm";
                    break;
                case "13d":
                    path = "Snow";
                    break;
                case "50d":
                    path = "Mist";
                    break;
            }
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
