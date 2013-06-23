using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;

namespace ImageWorkflow.Model
{

	[Export(typeof(ISourceGenerator))]
	public class Test2Generator : ISourceGenerator
	{
		public string Name { get { return "Test2"; } }
		public Size Size { get; private set; }


		public Bitmap Generate()
		{
			Random rand = new Random(0);
			var n1 = Size.noise(5, 0.5, 0, 1).abs().norm(0, 1);
			var basis = new ImageData(Size.Width, Size.Height);

			var contParts = 2;
			var islParts = 3;

			var rad = Size.Height / contParts / 3;

			var cont = 0
				.Upto(contParts)
				.Aggregate(basis, (acc, part) =>
				{
					var x = rand.Next(Size.Width);
					var y = rand.Next(Size.Height);
					return acc.circle(new Point(x, y), rad);
				});

			var contAdd1 = 0
				.Upto(contParts * 3)
				.Aggregate(cont, (acc, part) =>
				{
					var x = rand.Next(Size.Width);
					var y = rand.Next(Size.Height);
					return acc.spatter(new Point(x, y), rad / 2);
				});

			var contAdd2 = 0
				.Upto(contParts * 3 + islParts * 4)
				.Aggregate(contAdd1, (acc, part) =>
				{
					var x = rand.Next(Size.Width);
					var y = rand.Next(Size.Height);
					return acc.circle(new Point(x, y), rad / 3);
				})
				//.distortx(n1, 50)
				.distorty(n1, 50);

			var contAdd3 = 0
				.Upto(islParts * 16)
				.Aggregate(contAdd2, (acc, part) =>
				{
					var x = rand.Next(Size.Width);
					var y = rand.Next(Size.Height);
					return acc.square(new Point(x, y), rad / 3);
				});

			var contAdd4 = 0
				.Upto(islParts * 32)
				.Aggregate(contAdd3, (acc, part) =>
				{
					var x = rand.Next(Size.Width);
					var y = rand.Next(Size.Height);
					return acc.square(new Point(x, y), rad / 6);
				});


			var n = Size.noise(70, 0.5, 0, 1).norm(0, 1);
			//var n = Size.noise(5, 0.5, 0, 1).abs().norm(0, 1);
			//var n = Size.noise(20, 0.5, 0, 1).norm(0, 1);


			//return n.toBitmap();

			return contAdd4
				.shuffle(1, 100000)
				.max()
				//.distortx(n, 10)
				.distorty(n, 10)
				.shuffle(1, 10000)
				//.ddd()
				//.mul(n)
				.toBitmap();

		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
		}
	}
}
