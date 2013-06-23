using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageWorkflow.Model
{
	public static class ImageUtils
	{
		public static List<List<double>> noise(this Size size, double scale, double persist, int octaves)
		{
			return size.noise(scale, persist, octaves, it => it);
		}
		public static List<List<double>> noise(this Size size, double scale, double persist, int octaves, Func<double, double> preprocessor)
		{
			var a = new double[octaves];
			var d = new double[octaves];

			0.Upto(octaves)
				.Foreach(n =>
				{
					a[n] = Math.Pow(persist, n);
					d[n] = Math.Pow(2, (octaves - n));
				});

			return
				0.Upto(size.Height)
					.Select(y =>
						0.Upto(size.Width)
							.Select(x => 0
								.Upto(octaves)
								.Aggregate(0.0, (acc, n) =>
									acc + a[n] * (NoiseGen.SimplexNoise.noise(
										scale * (float)x / d[n],
										scale * (float)y / d[n],
										1))
									.With(preprocessor)
								)
							)
							.ToList())
					.ToList();
		}

		public static List<List<double>> each(this List<List<double>> src, Func<double, double> fn)
		{
			return src
				.Select(row => row
					.Select(fn)
					.ToList())
				.ToList();
		}

		public static List<List<double>> threshold(this List<List<double>> src, double thr)
		{
			return src
				.Select(row => row
					.Select(px => px > thr ? 1.0 : 0.0)
					.ToList())
				.ToList();
		}

		public static List<List<double>> range(this List<List<double>> src, double left, double right, Func<double, double> proc)
		{
			return src
				.Select(row => row
					.Select(px => px > right || px < left ? 0.0 : proc(px))
					.ToList())
				.ToList();
		}

		public static List<List<double>> abs(this List<List<double>> src)
		{
			return src
				.Select(row => row
					.Select(px => 2.0 * Math.Abs(px - 0.5))
					.ToList())
				.ToList();
		}

		public static List<List<double>> gamma(this List<List<double>> src, double gamma)
		{
			return src
				.Select(row => row
					.Select(px => Math.Pow(px, gamma))
					.ToList())
				.ToList();
		}

		public static List<List<double>> invert(this List<List<double>> src)
		{
			return src
				.Select(row => row
					.Select(px => 1 - px)
					.ToList())
				.ToList();
		}

		public static List<List<double>> norm(this List<List<double>> src, double min, double max)
		{
			var mn = 1000.0;
			var mx = -1000.0;

			src
				.SelectMany(it => it)
				.Foreach(it =>
				{
					if (it < mn) mn = it;
					if (it > mx) mx = it;
				});

			var offset = min - mn;
			var k = (max - min) / (mx - mn);

			return src
				.Select(row => row
					.Select(px => k * (px + offset))
					.ToList())
				.ToList();
		}

		public static List<List<double>> add(this List<List<double>> src, List<List<double>> addition)
		{
			var size = new Size(src[0].Count, src.Count);
			var w = size.Width;
			var h = size.Height;

			var pixels = Enumerable
				.Range(0, size.Height)
				.Select(y => src[y].ToList())
				.ToList();

			0.Upto(size.Height)
				.Foreach(y =>
					0.Upto(size.Width)
						.Foreach(x =>
						{
							pixels[y][x] += addition[y][x];
						}));
			return pixels;
		}


		public static List<List<double>> floyd_steinberg(this List<List<double>> src)
		{
			var size = new Size(src[0].Count, src.Count);
			var w = size.Width;
			var h = size.Height;

			var pixels = Enumerable
				.Range(0, size.Height)
				.Select(y => src[y].ToList())
				.ToList();

			0.Upto(size.Height)
				.Foreach(y =>
					0.Upto(size.Width)
						.Foreach(x =>
						{
							var grayval = pixels[y][x];
							var newval = grayval < 0.5 ? 0.0 : 1.0;
							var error = grayval - newval;

							pixels[y][x] = newval;

							if (x < (w - 1)) pixels[y + 0][x + 1] = pixels[y + 0][x + 1] + (error * 7.0 / 16.0);
							if (x > 0 && y < (h - 1)) pixels[y + 1][x - 1] = pixels[y + 1][x - 1] + (error * 3.0 / 16.0);
							if (y < (h - 1)) pixels[y + 1][x + 0] = pixels[y + 1][x + 0] + (error * 5.0 / 16.0);
							if (x < (w - 1) && y < (h - 1)) pixels[y + 1][x + 1] = pixels[y + 1][x + 1] + (error * 1.0 / 16.0);
						}));
			return pixels;
		}

		public static Bitmap toBitmap(this List<List<double>> src, int bp = 0, int wp = 255)
		{
			var size = new Size(src[0].Count, src.Count);
			var format = PixelFormat.Format32bppArgb;
			var bitmap = new Bitmap(size.Width, size.Height, format);
			using (var lb = new LockBitmap(bitmap, ImageLockMode.WriteOnly))
			{
				src
					.norm(bp, wp)
					.Foreach((row, y) => row
						.Foreach((val, x) =>
							lb.SetPixel(x, y, Gray((int)val))));
			}
			return bitmap;
		}

		public static List<List<double>> toPixels(this Bitmap src)
		{
			var pixels = Enumerable
				.Range(0, src.Height)
				.Select(y => Enumerable.Repeat(0.0, src.Width).ToList())
				.ToList();

			using (var bd = new LockBitmap(src, ImageLockMode.ReadOnly))
			{
				0.Upto(src.Height)
					.Foreach(y => 0.Upto(src.Width)
						.Foreach(x => pixels[y][x] = Gray(bd.GetPixel(x, y))));
			}
			return pixels;
		}

		private static double Gray(Color color)
		{
			return ((double)(7 * color.G + 2 * color.R + color.B)) / 2550.0;
		}

		private static Color Gray(int value)
		{
			if (value < 0)
				value = 0;
			else if (value > 255)
				value = 255;

			return Color.FromArgb(value, value, value);
		}
	}

}
