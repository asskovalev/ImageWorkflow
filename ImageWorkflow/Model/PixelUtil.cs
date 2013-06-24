using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Imglib;

namespace ImageWorkflow.Model
{
	public static class PxOp
	{
		public class PixelMapChain
		{
			private ImageData image;
			private List<PixelMap> sequence = new List<PixelMap>();

			internal PixelMapChain(ImageData src)
			{
				image = src;
			}

			internal PixelMapChain Add(PixelMap processor)
			{
				sequence.Add(processor);
				return this;
			}

			internal PixelMap BuildMap()
			{
				if (sequence.Count == 0)
					return x => x;

				return x => sequence
					.Aggregate(x, (acc, fn) => fn(acc));
			}

			internal ImageData Build()
			{
				return new ImageData(image, BuildMap());
			}
		}

		public static PixelMap m_invert = x => 1 - x;
		public static PixelMap m_abs = px => 2.0 * Math.Abs(px - 0.5);
		public static Func<double, PixelMap> m_threshold = thr => px => px > thr ? 1.0 : 0.0;
		public static Func<double, PixelMap> m_gamma = g => px => Math.Pow(px, g);

		public static PixelMapChain toChain(this ImageData src)
		{
			return new PixelMapChain(src);
		}

		public static PixelMapChain invert(this PixelMapChain src)
		{
			src.Add(m_invert);
			return src;
		}

		public static PixelMapChain abs(this PixelMapChain src)
		{
			src.Add(m_abs);
			return src;
		}

		public static PixelMapChain threshold(this PixelMapChain src, double thr)
		{
			src.Add(m_threshold(thr));
			return src;
		}

		public static PixelMapChain gamma(this PixelMapChain src, double g)
		{
			src.Add(m_gamma(g));
			return src;
		}

		public static PixelMapChain custom(this PixelMapChain src, PixelMap map)
		{
			src.Add(map);
			return src;
		}

		public static PixelMapChain range(this PixelMapChain src, double left, double right, 
			Func<double, double> in_proc, Func<double, double> out_proc)
		{
			return src.custom(px => px > right || px < left 
				? out_proc(px)
				: in_proc(px));
		}

		public static PixelMapChain range(this PixelMapChain src, double left, double right,
			Func<double, double> in_proc)
		{
			return src.custom(px => px > right || px < left
				? 0
				: in_proc((px - left) / (right - left) ));
		}


		public static PixelMapChain range(this PixelMapChain src, double left, double right)
		{
			return src.custom(px => px > right || px < left
				? 0
				: (px - left) / (right - left));
		}

		public static ImageData apply(this PixelMapChain src)
		{
			return src.Build();
		}
	}
}
