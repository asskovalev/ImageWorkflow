using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageWorkflow.Model
{
	public abstract class BaseTransformation : ITransformation
	{
		public abstract string Name { get; }

		public abstract Bitmap Apply(Bitmap source);

		protected static Color Gray(int value)
		{
			if (value < 0)
				value = 0;
			else if (value > 255)
				value = 255;

			return Color.FromArgb(value, value, value);
		}
	}
}
