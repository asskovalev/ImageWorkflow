using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decoder = Filbert.Decoder;
using Encoder = Filbert.Encoder;
using System.IO;
using System.Drawing;
using Imglib;
using Filbert.Core;

namespace MapGen
{
	static class Program
	{
		static void Main(string[] args)
		{
			using (var inp = Console.OpenStandardInput())
			using (var outp = Console.OpenStandardOutput())
			using (var sin = new BinaryReader(inp))
			using (var sout = new BinaryWriter(outp))
				while (true)
				{
					var length = ToUInt32(sin.ReadBytes(4));
					var tIn = Decoder.decode(sin.BaseStream);

					byte[] data = null;
					using (var ms = new MemoryStream())
					{
						if (tIn.IsTuple)
						{
							Bert result;
							try
							{
								result = Dispatch(tIn);
							}
							catch (Exception ex)
							{
								result = Error(ex);
							}
							Encoder.encode(result, ms);
							ms.Flush();
							ms.Seek(0, SeekOrigin.Begin);
							data = ms.ToArray();
						}
					}
					sout.Write(ToBytes(data.Length));
					sout.Write(data);

				}
		}

		static Int32 ToUInt32(byte[] src)
		{
			return src.Aggregate((Int32)0, (acc, b) => acc * 256 + b);
		}

		static byte[] ToBytes(Int32 src)
		{
			byte[] result = new byte[4];
			var i = 3;
			while (i >= 0)
			{
				result[i] = (byte)(src & 0xFF);
				src = src >> 8;
				i--;
			}

			return result;
		}

		static Bert Error(Exception err)
		{
			return Error(err.ToString());
		}

		static Bert Error(string err)
		{
			return Bert.NewTuple(new[]{
					Bert.NewAtom("error"),
					Bert.NewByteList(System.Text.Encoding.UTF8.GetBytes(err))
				});
		}

		static Bert Dispatch(Bert inp)
		{
			var data = (Bert.Tuple)inp;

			var tag = data.Item[0].BertCast<Bert.Atom>().Item;
			var settings = data.Item[1].BertDict();
			var layerMap = ParseLayerMap(data.Item[2]);

			switch (tag)
			{
				case "fullmap": return FullMap(settings, layerMap);
				default: throw new NotImplementedException();
			}
		}

		static T BertCast<T>(this Bert src) where T: Bert
		{
			var value = src as T;
			return value;
		}

		static Dictionary<string, Bert> BertDict(this Bert src)
		{
			var rawItems = src is Bert.List
				? ((Bert.List)src).Item
				: new Bert[] {};

			var items = rawItems
				.OfType<Bert.Tuple>()
				.Select(BertCast<Bert.Tuple>)
				.Where(it => it.Item.Length == 2)
				.Where(it => it.Item[0] is Bert.Atom)
				.ToList();

			return items
				.ToDictionary(
					it => it.Item[0].BertCast<Bert.Atom>().Item,
					it => it.Item[1]
				);
		}

		static Dictionary<string, byte> ParseLayerMap(Bert map)
		{
			return ((Bert.List)map).Item
				.Select(BertCast<Bert.Tuple>)
				.ToDictionary(
					it => ((Bert.Atom)it.Item[0]).Item,
					it => (byte)((Bert.Integer)it.Item[1]).Item);
		}


		static Bert FullMap(Dictionary<string, Bert> settings, Dictionary<string, byte> layerMap)
		{
			var w = settings["w"].BertCast<Bert.Integer>().Item;
			var h = settings["h"].BertCast<Bert.Integer>().Item;
			var seed = settings["seed"].BertCast<Bert.Integer>().Item;

			var basis = new Size(w, h)
				.noise(5, 0.5, seed, 4);

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


			var result = Enumerable
				.Range(0, h)
				.Select(y => new byte[w])
				.ToArray();

			trees
				.iter((x, y, _) =>
					{
						result[y][x] = layerMap["none"];

						if (grass[x, y] > 0)
							result[y][x] = layerMap["grass"];

						if (rocks[x, y] > 0)
							result[y][x] = layerMap["rock"];

						if (shrub[x, y] > 0)
							result[y][x] = layerMap["shrub"];

						if (trees[x, y] > 0)
							result[y][x] = layerMap["tree"];
					});

			return Bert.NewList(result
				.Select(Bert.NewByteList)
				.ToArray());
		}
	}
}
