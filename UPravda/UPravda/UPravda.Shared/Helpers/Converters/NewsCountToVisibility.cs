using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UPravda.Models;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UPravda.Helpers.Converters
{
	public class NewsCountToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var newsItem = (ObservableCollection<NewsItem>)value;
			if (newsItem.Count == 0)
			{
				return Windows.UI.Xaml.Visibility.Collapsed;
			}
			else
			{
				return Windows.UI.Xaml.Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return null;
		}
	}
}
