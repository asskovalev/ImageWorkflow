using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;

namespace ImageWorkflow.Model
{
	[Export(typeof(ISourceGenerator))]
	public class RandomGenerator : ISourceGenerator
	{
		public string Name { get { return "Random"; } }
		public Size Size { get; private set; }

		private Random rand = new Random();

		public Bitmap Generate()
		{
			var result = new Bitmap(Size.Width, Size.Height);
			0.Upto(result.Height)
				.Foreach(y =>
					0.Upto(result.Width)
						.Foreach(x =>
						{
							var newPx = Color.FromArgb(
								rand.Next(255),
								rand.Next(255),
								rand.Next(255));
							result.SetPixel(x, y, newPx);
						}));
			return result;
		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
		}
	}
}
