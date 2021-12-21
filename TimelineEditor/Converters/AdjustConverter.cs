using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Timeline.Converters
{
	class AdjustConverter : StaticBaseConverter
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var src = (double)System.Convert.ChangeType(value, typeof(double));

			double.TryParse(parameter as string, out double adj);

			return src + adj;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
