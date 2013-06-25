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

			switch (tag)
			{
				case "fullmap":
				{
					var layerMap = ParseLayerMap(data.Item[2]);
					return FullMap(settings, layerMap);
				}
				case "biome":   
					return BiomeMap(settings);
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

		static class Biome
		{
			public static Bert Snow   = Bert.NewAtom("snow");
			public static Bert Plains = Bert.NewAtom("plains");
			public static Bert Tundra = Bert.NewAtom("tundra");
			public static Bert Swamp  = Bert.NewAtom("swamp");
			public static Bert Forest = Bert.NewAtom("forest");
			public static Bert Woods  = Bert.NewAtom("woods");
			public static Bert Desert = Bert.NewAtom("desert");
			public static Bert Rocks  = Bert.NewAtom("rocks");
			public static Bert Ocean  = Bert.NewAtom("water");
			public static Bert Unknown  = Bert.NewAtom("unk");
		}

		static Bert BiomeMap(Dictionary<string, Bert> settings)
		{
			var w = settings["w"].BertCast<Bert.Integer>().Item;
			var h = settings["h"].BertCast<Bert.Integer>().Item;
			var seed = settings["seed"].BertCast<Bert.Integer>().Item;

			var size = new Size(w, h);

			var basis = size
				.noise(1, 0.7, seed, 8)
				.norm(0, 1);

			var temperature = size
				.noise(1, 0.5, seed + 1, 3)
				.norm(0, 1)
				.gamma(0.6)
				.each(x => (int)(x * 10))
				.norm(0, 1);


			var rainfall = size
				.noise(1, 0.5, seed + 2, 8)
				.each(x => (int)(x * 10))
				.norm(0, 1);

			var land = basis
				.threshold(0.35);
			
			// правая часть											    
			Func<double, double, double> p_desert = (r, t) => t >= 0.50                           && r <= 0.25 ? 1.0 : 0.0;
			Func<double, double, double> p_woods  = (r, t) => t >= 0.50              && r >= 0.25 && r <= 0.50 ? 1.0 : 0.0;
			Func<double, double, double> p_forest = (r, t) => t >= 0.50              && r >= 0.50 && r <= 0.80 ? 1.0 : 0.0;
			Func<double, double, double> p_swamp  = (r, t) => t >= 0.50              && r >= 0.80              ? 1.0 : 0.0;

			// левая часть
			Func<double, double, double> p_tundra = (r, t) =>              t <= 0.25              && r <= 0.6  ? 1.0 : 0.0;
			Func<double, double, double> p_plains = (r, t) => t >= 0.25 && t <= 0.50              && r <= 0.6  ? 1.0 : 0.0;
			Func<double, double, double> p_snow   = (r, t) =>              t <= 0.50 && r >= 0.60              ? 1.0 : 0.0;

			// айсберги
			Func<double, double, double> p_ice    = (r, t) =>              t <= 0.30                           ? 1.0 : 0.0;

			var ocean = land.invert();
			var rocks = basis.range(0.9, 1, x => 1.0);
			var desert = rainfall.zip(temperature, p_desert).mul(land);
			var woods  = rainfall.zip(temperature, p_woods ).mul(land);
			var forest = rainfall.zip(temperature, p_forest).mul(land);
			var swamp  = rainfall.zip(temperature, p_swamp ).mul(land);
			var tundra = rainfall.zip(temperature, p_tundra).mul(land);
			var plains = rainfall.zip(temperature, p_plains).mul(land);
			var snow   = rainfall.zip(temperature, p_snow  ).mul(land);
			var ice    = rainfall.zip(temperature, p_ice   ).mul(ocean);


			var result = Enumerable
				.Range(0, h)
				.Select(y => Enumerable
					.Range(0, w)
					.Select(x => Bert.Nil)
					.ToArray())
				.ToArray();

			basis
				.iter((x, y, _) =>
				{
					if      (ice   [x, y] > 0) result[y][x] = Biome.Snow;
					else if (snow  [x, y] > 0) result[y][x] = Biome.Snow;
					else if (plains[x, y] > 0) result[y][x] = Biome.Plains;
					else if (tundra[x, y] > 0) result[y][x] = Biome.Tundra;
					else if (swamp [x, y] > 0) result[y][x] = Biome.Swamp;
					else if (forest[x, y] > 0) result[y][x] = Biome.Forest;
					else if (woods [x, y] > 0) result[y][x] = Biome.Woods;
					else if (desert[x, y] > 0) result[y][x] = Biome.Desert;
					else if (rocks [x, y] > 0) result[y][x] = Biome.Rocks;
					else if (ocean [x, y] > 0) result[y][x] = Biome.Ocean;
					else                       result[y][x] = Biome.Unknown;
				});

			return Bert.NewList(result
				.Select(Bert.NewList)
				.ToArray());
		}

	}
}
