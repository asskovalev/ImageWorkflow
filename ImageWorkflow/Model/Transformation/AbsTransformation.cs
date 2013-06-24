using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;
using Imglib;

namespace ImageWorkflow.Model
{
	[Export(typeof(ITransformation))]
	public class AbsTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "Abs"; }
		}

		public override Bitmap Apply(Bitmap source)
		{
			var result = new Bitmap(source);
			return result
				.toPixels()
				.abs()
				.toBitmap();
		}
	}
}
