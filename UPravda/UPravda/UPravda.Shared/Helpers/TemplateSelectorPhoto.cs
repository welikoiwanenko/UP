using System;
using System.Collections.Generic;
using System.Text;
using UPravda.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UPravda.Helpers
{
	public class TemplateSelectorPhoto : DataTemplateSelector
	{
		public DataTemplate FirstPhotoNewsTemplate { get; set; }
		public DataTemplate SecondPhotoNewsTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			FrameworkElement element = container as FrameworkElement;
			var data = item as NewsItem;
			if (element != null && item != null)
			{
				if (data.Size == 1)
				{
					return FirstPhotoNewsTemplate;
				}
				else
				{
					return SecondPhotoNewsTemplate;
				}
			}
			return null; /* or a template of ur choice if it's an expected behavior */
		}
	}
}
