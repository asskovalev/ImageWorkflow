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
	public class TreesGenerator : ISourceGenerator
	{
		public string Name { get { return "Trees"; } }
		public Size Size { get; private set; }

		private Random rand = new Random();

		public Bitmap Generate()
		{
			var basis = Size
				.noise(5, 0.5, 1, 4);

			var trees = basis
				.range(0.4, 0.7, it => 1 - it)
				.gamma(4)
				.each(x => x / 3)
				.ip_bw_rand(1);

			var shrub = basis
				.range(0.4, 0.7, it => 1 - it)
				.gamma(2)
				.each(x => x / 2)
				.ip_bw_rand(2)
				.shuffle(3, 100);

			var grass = basis
				.range(0.1, 0.8, x => x)
				.ip_bw_rand(3);

			var rocks = basis
				.range(0.1, 0.8, x => x)
				.gamma(5)
				.each(x => x / 5)
				.ip_bw_rand(4);


            return ImageBlend.combine(
				grass.layer(Color.Red),
            //    dirt.layer_binary(Color.FromArgb(128, 64, 0)),
            //    sand.layer_binary(Color.Yellow),
                shrub.layer(Color.FromArgb(0x22, 0x99, 0x22)),
            //    trees.layer(Color.FromArgb(0x00, 0x66, 0x00)),
            //    water.layer_binary(Color.FromArgb(0x00, 0x7f, 0xff)),
            //    river.layer(Color.Blue),

                trees.layer(Color.FromArgb(0x80, 0x66, 0x00))                ,       
				rocks.layer(Color.White)
              
            //    snow.layer_binary(Color.White)
                );


            return trees.toBitmap();

            //return sob.toBitmap();

            //var dirt = basis
            //    .each(px => 1);

            //var rock = basis
            //    .toChain()
            //    .range(0.6, 1, x => x + 0.2)
            //    .apply()
            //    ;

            //var snow = basis
            //    .toChain()
            //    .range(0.8, 1)
            //    .apply();

            //var trees = basis
            //    .toChain()
            //    .range(0.3, 0.7, x => 1 - PxOp.m_abs(x))
            //    .apply()
            //    ;

            //var grass = basis
            //    .toChain()
            //    .range(0.1, 0.8, x => 1 - PxOp.m_abs(x))
            //    .apply()
            //    ;

            //var river = basis
            //    .toChain()
            //    .range(0.4, 0.7, x => 1 - PxOp.m_abs(x))
            //    .range(0.9, 1)
            //    .gamma(1.7)
            //    .apply()
            //    ;

            //var tr = trees
            //    //.sub(river.gamma(0.2))
            //    .each(x => x / 4);

            //var shrub = basis
            //    .toChain()
            //    .range(0.4, 0.7, x => 1 - PxOp.m_abs(x))
            //    .custom(x => x / 4)
            //    .apply()
            //    ;

            //var water = basis
            //    .toChain()
            //    .range(0.0, 0.3, x => 1 - x)
            //    .apply()
            //    ;

            //var sand = basis
            //    .toChain()
            //    .range(0.2, 0.3, x => 1 - PxOp.m_abs(x))
            //    .apply()
            //    ;




            //return ImageBlend.combine(
            //    dirt.layer_binary(Color.FromArgb(128, 64, 0)),
            //    sand.layer_binary(Color.Yellow),
            //    grass.layer_binary(Color.FromArgb(0x32, 0xcd, 0x32)),
            //    shrub.layer_binary(Color.FromArgb(0x22, 0x8b, 0x22)),
            //    tr.layer_binary(Color.FromArgb(0xff, 0x00, 0x00)),
            //    water.layer_binary(Color.FromArgb(0x00, 0x7f, 0xff)),
            //    river.layer(Color.Blue),
            //    rock.layer_binary(Color.Silver),
            //    snow.layer_binary(Color.White)
            //    );
		}


		public void Configure(Size size, params object[] parameters)
		{
			Size = size;
		}
	}
}
