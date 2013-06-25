using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Imglib
{
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
            Size = new Size(w, h);
            data = new T[h][];
            for (var i = 0; i < h; i++)
                data[i] = new T[w];
        }

        public T[][] CopyData()
        {
            var h = Size.Height;
            var w = Size.Width;
            var result = new T[h][];
			Enumerable.Range(0, h)
				.AsParallel()
				.ForAll(i =>
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
			Enumerable.Range(0, h)
				.AsParallel()
				.ForAll(i => result[i] = data[i].Select(fn).ToArray());
            return result;
        }
    }
}
