using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageWorkflow.Model
{
	[Export(typeof(ISourceGenerator))]
	public class TurbulenceNoiseGenerator : ISourceGenerator
	{
		public string Name { get { return "TurbulenceNoise"; } }
		public Color Color { get; private set; }
		public Size Size { get; private set; }

		private Random rand = new Random();

		public Bitmap Generate()
		{
			var pixels = Size
                .noise(5, 0.5, 1, 4)
				.norm(0, 1)
				.gamma(1.8)
				//.threshold(0.5)
				//.gamma(0.7)
				//.threshold(0.5)
				;

			return pixels.toBitmap();
		}

		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
			Color = parameters.DefaultIfEmpty(Color.White).Cast<Color>().SingleOrDefault();
		}
	}
}
