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
	public class InvertTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "Invert"; }
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
							var newPx = Color.FromArgb(
								255 - srcPx.R,
								255 - srcPx.G,
								255 - srcPx.B);
							write.SetPixel(x, y, newPx);
						}));
			return result;
		}
	}
}
