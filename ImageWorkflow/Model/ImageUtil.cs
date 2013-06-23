using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageWorkflow.Model
{
	public static class ImageBlend
	{
		public class ColorLayer
		{
			public ImageData image;
			public Color color;
			public ColorLayer(ImageData image, Color color)
			{
				this.image = image;
				this.color = color;
			}
		}

		public static ColorLayer layer(this ImageData image, Color color)
		{
			return new ColorLayer(image, color);
		}

		public static ColorLayer layer_binary(this ImageData image, Color color)
		{
			return new ColorLayer(image.floyd_steinberg(), color);
		}

		public static Bitmap combine(params ColorLayer[] layers)
		{
			if (layers.Select(l => l.image.Size.Width).Distinct().Count() > 1)
				throw new ArgumentOutOfRangeException("layers", "images should have same width");

			if (layers.Select(l => l.image.Size.Height).Distinct().Count() > 1)
				throw new ArgumentOutOfRangeException("layers", "images should have same height");

			var size = layers[0].image.Size;

			var parts = layers
				.Select(l => {
					var img = 	new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
					using(var lb = new LockBitmap(img, ImageLockMode.WriteOnly))
					{
						l.image
							.iter((x, y, v) => 
								lb.SetPixel(x, y, Color.FromArgb((int)(v * 255), l.color)));
						return img;
					}
				})
				.ToList();
				
			var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

			using(var gr = Graphics.FromImage(bitmap))
			{
				gr.FillRectangle(Brushes.Black, new Rectangle(new Point(), bitmap.Size));
				parts
					.Foreach(p => gr.DrawImage(p, new Point()));
			}

			//    layers[0].image
			//        .iter((x, y, v) =>
			//        {
			//            var colors = layers
			//                .Select(l => new 
			//                    {
			//                        a = l.image[x, y],
			//                        r = (float)l.color.R / 255.0,
			//                        g = (float)l.color.G / 255.0,
			//                        b = (float)l.color.B / 255.0
			//                    })
			//                .ToList();

			//            var blended = colors.Aggregate((p, n) => new 
			//                {
			//                    a = p.a + n.a,
			//                    r = p.r * p.a + n.r * n.a,
			//                    g = p.g * p.a + n.g * n.a,
			//                    b = p.b * p.a + n.b * n.a,
			//                });
			//            var k = layers.Count();
			//            var c = blended.a < 0.01 
			//                ? Color.Black
			//                : Color.FromArgb(
			//                    255,
			//                    (int)(nnn(blended.r, blended.a) * 255 / blended.a),
			//                    (int)(nnn(blended.g, blended.a) * 255 / blended.a),
			//                    (int)(nnn(blended.b, blended.a) * 255 / blended.a));
			//            lb.SetPixel(x, y, c);
			//        });
			//}
			return bitmap;
		}

		private static double nnn(double col, double al)
		{
			return (al > 0 && col > al)
				? al
				: col;
		}
	}

	public static class ImageOperations
	{
	}

}
