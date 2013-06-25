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
	public class TestGenerator : ISourceGenerator
	{
		public string Name { get { return "Test"; } }
		public Size Size { get; private set; }

		private Random rand = new Random();

		public Bitmap Generate()
		{
			var basis = Size
				.noise(5, 0.5, 1, 4);

            var gradient = basis.prewitt();

            var rock1 = basis
                .toChain()
                .range(0.6, 1, x => x * x / 50)
                .apply()
                .ip_bw_rand()
                .dilate()
                .randian()
                //.max()
                //.randian()
                ;

            var rock2 = gradient
                .toChain()
                .gamma(1.5)
                .apply()
                .ip_bw_rand(77)
                .dilate()
                .randian()
                ;
            var trees = gradient
                .toChain()
                .range(0, 0.05, it => 1 - it)
                .gamma(5)
                .custom(x => x / 200)
                .apply()
                .ip_bw_rand(1)
                .dilate()
                .randian()
                //.max()
                //.randian()
                ;

            var shrub = gradient
                .toChain()
                .range(0, 0.3, it => 1 - it)
                .custom(x => x * x / 60)
                .apply()
                .ip_bw_rand(2)
                .dilate()
                .randian()
                ;

            var grass1 = basis
                .toChain()
                .range(0.0, 0.8)
                .custom(x => x * x / 5)
                .apply()
                .ip_bw_rand(3)
                .dilate()
                .randian()
                ;

            var lakes = basis
                .norm(0,1)
                .toChain()
                .range(0, 0.1, x => 1 - x)
                .apply()
                .threshold(0.3)
                .dilate()
                .dilate()
                .randian()
                //.max()
                //.median()
                ;
            var h = basis
                .each(x => (int)(50 * x))
                
                .prewitt().norm(0, 1);

            return ImageBlend.combine(
            //    basis.layer(Color.Red),
            //    dirt.layer_binary(Color.FromArgb(128, 64, 0)),
            //    sand.layer_binary(Color.Yellow),
            //    shrub.layer_binary(Color.FromArgb(0x22, 0x8b, 0x22)),
            //    trees.layer(Color.FromArgb(0x00, 0x66, 0x00)),
            //    water.layer_binary(Color.FromArgb(0x00, 0x7f, 0xff)),
            //    river.layer(Color.Blue),
                grass1.layer(Color.FromArgb(0x32, 0xcd, 0x32)),
                shrub.layer(Color.FromArgb(0x8b, 0x8b, 0x22)),
                rock2.layer(Color.Red),
                trees.layer(Color.FromArgb(0x00, 0x66, 0x00)),
                lakes.layer(Color.Blue)
                //,                h.layer(Color.White)
              
            //    snow.layer_binary(Color.White)
                );


            return trees.toBitmap();
		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
		}
	}
}
