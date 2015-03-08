using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UPravda.Helpers.Converters
{
	public class HighlighterValueToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
            if (System.Convert.ToInt32(value) == 1)
            {
                if (App.Theme == Windows.UI.Xaml.ElementTheme.Light)
                {
                    return new SolidColorBrush(new Color() { A = 185, R = 200, G = 0, B = 32 });
                }
                return new SolidColorBrush(new Color() { A = 255, R = 200, G = 0, B = 32 });
            }
            else
            {
                if (App.Theme == Windows.UI.Xaml.ElementTheme.Light)
                {
                    return new SolidColorBrush(new Color() { A = 255, R = 0, G = 0, B = 0 });
                }
                return new SolidColorBrush(new Color() { A = 255, R = 255, G = 255, B = 255 });
            }
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return null;
		}
	}
}
