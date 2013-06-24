using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using Imglib;

namespace ImageWorkflow.Model
{
	[Export(typeof(ITransformation))]
	public class FloydSteinbergTransformation : BaseTransformation
	{
		public override string Name
		{
			get { return "FloydSteinberg"; }
		}

		public override Bitmap Apply(Bitmap source)
		{
			var w = source.Width;
			var h = source.Height;

			var pixels = source.toPixels();
			
			return pixels.floyd_steinberg().toBitmap();
		}
	}
}
