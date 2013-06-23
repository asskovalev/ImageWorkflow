using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;

namespace ImageWorkflow.Model
{
	[Export(typeof(ITransformation))]
	public class GrayscaleTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "Grayscale"; }
		}

		public override Bitmap Apply(Bitmap source)
		{
			var result = new Bitmap(source);
			using (var read = new LockBitmap(source, ImageLockMode.ReadOnly))
			using (var write = new LockBitmap(result, ImageLockMode.WriteOnly))
				0.Upto(source.Height)
					.Foreach(y => 
						0.Upto(source.Width)
							.Foreach(x => {
								var srcPx = read.GetPixel(x, y);
								var grayval = (7 * srcPx.G + 2 * srcPx.R + srcPx.B) / 10;
								var newPx = Color.FromArgb(
									grayval,
									grayval,
									grayval);
								write.SetPixel(x, y, newPx);
							}));
			return result;
		}
	}
}
