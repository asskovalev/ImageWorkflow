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
	public class AutoLevelTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "AutoLevel"; }
		}

		public override Bitmap Apply(Bitmap source)
		{
			var result = new Bitmap(source);
			var min = 255.0;
			var max = 0.0;
			using (var read = new LockBitmap(source, ImageLockMode.ReadOnly))
			using (var write = new LockBitmap(result, ImageLockMode.WriteOnly))
			{
				0.Upto(source.Height)
					.Foreach(y =>
						0.Upto(source.Width)
							.Foreach(x =>
							{
								var srcPx = read.GetPixel(x, y);
								var grayval = (7 * srcPx.G + 2 * srcPx.R + srcPx.B) / 10;
								if (grayval < min) min = grayval;
								if (grayval > max) max = grayval;
							}));

				var offset = 0 - min;
				var k = 255 / (max + offset);


				0.Upto(source.Height)
					.Foreach(y =>
						0.Upto(source.Width)
							.Foreach(x =>
							{
								var srcPx = read.GetPixel(x, y);
								var newPx = Color.FromArgb(
									Convert.ToInt32(k * (srcPx.R + offset)),
									Convert.ToInt32(k * (srcPx.G + offset)),
									Convert.ToInt32(k * (srcPx.B + offset)));
								write.SetPixel(x, y, newPx);
							}));
				return result;
			}
		}
	}
}
