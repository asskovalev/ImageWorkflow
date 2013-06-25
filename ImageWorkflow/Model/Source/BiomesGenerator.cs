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

	[Export(typeof(ISourceGenerator))]
	public class BiomesGenerator : ISourceGenerator
	{
		public string Name { get { return "Biomes"; } }
		public Size Size { get; private set; }

		private Random rand = new Random();

		public Bitmap Generate()
		{
			var seed = 1;
			var basis = Size
				.noise(1, 0.7, seed, 8)
				.norm(0, 1);

			var temperature = Size
				.noise(1, 0.5, seed + 1, 3)
				.norm(0, 1)
				.gamma(0.6)
				.each(x => (int)(x * 10))
				.norm(0, 1);


			var rainfall = Size
				.noise(1, 0.5, seed + 2, 8)
				.each(x => (int)(x * 10))
				.norm(0, 1);

			var land = basis
				.threshold(0.35);

			var tThr = 0.5;

			Func<double, double, double> p_desert = (r, t) => t >= 0.50                           && r <= 0.25 ? 1.0 : 0.0;
			Func<double, double, double> p_woods  = (r, t) => t >= 0.50              && r >= 0.25 && r <= 0.50 ? 1.0 : 0.0;
			Func<double, double, double> p_forest = (r, t) => t >= 0.50              && r >= 0.50 && r <= 0.80 ? 1.0 : 0.0;
			Func<double, double, double> p_swamp  = (r, t) => t >= 0.50              && r >= 0.80              ? 1.0 : 0.0;

			Func<double, double, double> p_tundra = (r, t) =>              t <= 0.25              && r <= 0.6  ? 1.0 : 0.0;
			Func<double, double, double> p_plains = (r, t) => t >= 0.25 && t <= 0.50              && r <= 0.6  ? 1.0 : 0.0;
			Func<double, double, double> p_snow   = (r, t) =>              t <= 0.50 && r >= 0.60              ? 1.0 : 0.0;

			Func<double, double, double> p_ice    = (r, t) =>              t <= 0.30                           ? 1.0 : 0.0;

			var ocean = land.invert();
			var rivers = land;
			//land = land - river;
			var rocks = basis.range(0.9, 1, x => 1.0);
			var desert = rainfall.zip(temperature, p_desert).mul(land);
			var woods  = rainfall.zip(temperature, p_woods ).mul(land);
			var forest = rainfall.zip(temperature, p_forest).mul(land);
			var swamp  = rainfall.zip(temperature, p_swamp ).mul(land);
			var tundra = rainfall.zip(temperature, p_tundra).mul(land);
			var plains = rainfall.zip(temperature, p_plains).mul(land);
			var snow   = rainfall.zip(temperature, p_snow  ).mul(land);
			var ice    = rainfall.zip(temperature, p_ice   ).mul(ocean/*.max(rivers)*/);

			return ImageBlend.combine(
				ocean .layer(Color.Blue),
				plains.layer(Color.FromArgb(0x00, 0xa0, 0x00)),
				forest.layer(Color.FromArgb(0x00, 0x40, 0x00)),
				woods .layer(Color.FromArgb(0x00, 0x80, 0x00)),
				desert.layer(Color.FromArgb(0xff, 0xff, 0x00)),
				swamp .layer(Color.FromArgb(0x40, 0x40, 0x00)),
				tundra.layer(Color.FromArgb(0x10, 0x40, 0x10)),
				rocks .layer(Color.Silver),
				snow  .layer(Color.White),
				ice   .layer(Color.White));

		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
		}
	}
}
