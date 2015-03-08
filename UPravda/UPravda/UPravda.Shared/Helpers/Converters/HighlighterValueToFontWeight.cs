using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace UPravda.Helpers.Converters
{
    public class HighlighterValueToFontWeight: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (System.Convert.ToInt32(value) == 0)
			{
				return FontWeights.Normal;
			}
			else
			{
				return FontWeights.Bold;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return null;
		}
	}
}
