using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Linq.Expressions;

namespace ImageWorkflow.Model
{
	public delegate double PixelMap(double px);

	public class RawData<T> where T : struct
	{
		protected T[][] data;
		public T this[int x, int y]
		{
			get { return data[y][x]; }
			set { data[y][x] = value; }
		}

		public Size Size { get; set; }

		public RawData(int w, int h) : this(w, h, default(T)) { }
		public RawData(int w, int h, T initial)
		{
			Size = new System.Drawing.Size(w, h);
			data = new T[h][];

			Enumerable
				.Range(0, h)
				.Foreach((r, i) => data[i] = new T[w]);
		}

		public T[][] CopyData()
		{
			var h = Size.Height;
			var w = Size.Width;
			var result = new T[h][];
			Enumerable
				.Range(0, h)
				.Foreach(i =>
				{
					result[i] = new T[w];
					Array.Copy(data[i], result[i], w);
				});
			return result;
		}

		public T[][] CopyData(Func<T, T> fn)
		{
			var h = Size.Height;
			var w = Size.Width;
			var result = new T[h][];
			Enumerable
				.Range(0, h)
				.Foreach(i => result[i] = data[i].Select(fn).ToArray());
			return result;
		}
	}

	public class AlphaData : RawData<byte>
	{
		public AlphaData(int w, int h) : base(w, h, byte.MaxValue) { }
		public AlphaData(AlphaData original)
			: this(original.Size.Width, original.Size.Height)
		{
			data = original.CopyData();
		}
	}

	public class ImageData : RawData<double>
	{
		public ImageData(int w, int h) : base(w, h, 0.0) { }
		public ImageData(ImageData original)
			: this(original.Size.Width, original.Size.Height)
		{
			data = original.CopyData();
		}

		public ImageData(ImageData original, PixelMap fn)
			: this(original.Size.Width, original.Size.Height)
		{
			data = original.CopyData(x => fn(x));
		}
	}


	public class ImageWithAlphaData : ImageData
	{
		public AlphaData Alpha { get; set; }

		public ImageWithAlphaData(int w, int h)
			: base(w, h)
		{
			Alpha = new AlphaData(w, h);
		}

		public ImageWithAlphaData(ImageData original)
			: this(original.Size.Width, original.Size.Height)
		{
			data = original.CopyData();
			Alpha = new AlphaData(original.Size.Width, original.Size.Height);
		}

		public ImageWithAlphaData(ImageWithAlphaData original)
			: this(original.Size.Width, original.Size.Height)
		{
			data = original.CopyData();
			Alpha = new AlphaData(original.Alpha);
		}
	}
}
