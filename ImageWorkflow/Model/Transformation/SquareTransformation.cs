using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;

namespace ImageWorkflow.Model
{
	[Export(typeof(ITransformation))]
	public class SquareTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "Square"; }
		}

		public override Bitmap Apply(Bitmap source)
		{
			var result = new Bitmap(source);
			0.Upto(source.Height)
				.Foreach(y => 
					0.Upto(source.Width)
						.Foreach(x => {
							var srcPx = source.GetPixel(x, y);
							var grayval = (7 * srcPx.G + 2 * srcPx.R + srcPx.B) / 10;
							var newval = grayval * grayval / 255;
							var newPx = Color.FromArgb(
								newval,
								newval,
								newval);
							result.SetPixel(x, y, newPx);
						}));
			return result;
		}
	}
}
