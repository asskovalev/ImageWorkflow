using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;

namespace ImageWorkflow.Model
{
	[Export(typeof(ISourceGenerator))]
	public class SolidColorGenerator : ISourceGenerator
	{
		public string Name { get { return "SolidColor"; } }
		public Color Color { get; private set; }
		public Size Size { get; private set; }

		public Bitmap Generate()
		{
			var result = new Bitmap(Size.Width, Size.Height);
			var brush = new SolidBrush(Color);
			var fillFrom = new Point(0, 0);

			using (var g = Graphics.FromImage(result))
				g.FillRectangle(brush, new Rectangle(fillFrom, Size));

			return result;
		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
			Color = parameters.DefaultIfEmpty(Color.White).Cast<Color>().SingleOrDefault();
		}
	}
}
