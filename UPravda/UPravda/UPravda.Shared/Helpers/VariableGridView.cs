using System;
using System.Collections.Generic;
using System.Text;
using UPravda.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UPravda.Helpers
{
	public class VariableGridView : GridView
	{
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			var data = item as NewsItem;
			if (data != null)
			{
				element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, data.Size);
				element.SetValue(VariableSizedWrapGrid.RowSpanProperty, data.Size);
			}
			base.PrepareContainerForItemOverride(element, item);
		}
	}
}
