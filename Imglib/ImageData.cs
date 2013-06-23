using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imglib
{
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

}
