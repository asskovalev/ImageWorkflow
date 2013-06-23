using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Linq.Expressions;

namespace ImageWorkflow.Model
{
	
	
	public class AlphaData : RawData<byte>
	{
		public AlphaData(int w, int h) : base(w, h, byte.MaxValue) { }
		public AlphaData(AlphaData original)
			: this(original.Size.Width, original.Size.Height)
		{
			data = original.CopyData();
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
