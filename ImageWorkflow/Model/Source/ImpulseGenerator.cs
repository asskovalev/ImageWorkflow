using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;

namespace ImageWorkflow.Model
{
	[Export(typeof(ISourceGenerator))]
	public class ImpulseGenerator : ISourceGenerator
	{
		public string Name { get { return "Impulse"; } }
		public Size Size { get; private set; }

		public Bitmap Generate()
		{
			var result = new Bitmap(Size.Width, Size.Height);
			var bg = Brushes.Black;
			var fg = Color.White;
			var fillFrom = new Point(0, 0);

			using (var g = Graphics.FromImage(result))
				g.FillRectangle(bg, new Rectangle(fillFrom, Size));

			result.SetPixel(Size.Width / 2, Size.Height / 2, fg);

			return result;
		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
		}
	}
}
