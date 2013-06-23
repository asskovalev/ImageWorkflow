using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imglib
{
    public static class Uncat
    {
        public static ImageData noise(this Size size)
        {
            var rand = new Random(0);

            var image = new ImageData(size.Width, size.Height);
            image.iter((x, y, val) =>
                image[x, y] = rand.NextDouble());
            return image;
        }

        public static ImageData noise(this Size size, double scale, double persist, double variation, int octaves)
        {
            var a = new double[octaves];
            var d = new double[octaves];

            0.Upto(octaves)
                .Foreach(n =>
                {
                    a[n] = Math.Pow(persist, n);
                    d[n] = Math.Pow(2, (octaves - n));
                });

            var k = a.Sum() * 2;

            var image = new ImageData(size.Width, size.Height);

            image.iter((x, y, val) =>
                image[x, y] = 0
                    .Upto(octaves)
                    .Aggregate(0.0, (acc, n) =>
                        acc + a[n] * (SimplexNoise.noise(
                            scale * ((float)x / size.Width + 0.5) / d[n],
                            scale * ((float)y / size.Height + 0.5) / d[n],
                            variation))
                    ) / k + 0.5);
            return image;
        }

        public static ImageData iter(this ImageData src, Action<int, int, double> fn)
        {
            0.Upto(src.Size.Height)
                .Foreach(y =>
                    0.Upto(src.Size.Width)
                        .Foreach(x => fn(x, y, src[x, y])));
            return src;
        }

        public static ImageData each(this ImageData src, PixelMap fn)
        {
            return new ImageData(src, fn);
        }

        public static ImageData circle(this ImageData src, Point location, int radius)
        {
            var img = new ImageData(src);
            (location.X - radius)
                .Upto(radius * 2)
                .Where(x => x >= 0 && x < src.Size.Width)
                .Foreach(x => (location.Y - radius)
                    .Upto(radius * 2)
                    .Where(y => y >= 0 && y < src.Size.Height)
                    .Foreach(y =>
                    {
                        var dX = Math.Abs(x - location.X);
                        var dY = Math.Abs(y - location.Y);

                        var d = Math.Sqrt(dX * dX + dY * dY);
                        if (d <= radius)
                            img[x, y] = 1.0;
                    }));

            return img;
        }

        public static ImageData shuffle(this ImageData src, int radius, int amount)
        {
            var img = new ImageData(src);
            var rand = new Random(0);
            while (amount-- > 0)
            {
                var x1 = rand.Next(src.Size.Width);
                var y1 = rand.Next(src.Size.Height);

                var ox = rand.Next(radius * 2) - radius;
                var oy = rand.Next(radius * 2) - radius;

                var x2 = x1 + ox;
                var y2 = y1 + oy;

                if (x2 < 0 || x2 >= src.Size.Width ||
                    y2 < 0 || y2 >= src.Size.Height)
                    continue;

                var t = img[x2, y2];
                img[x2, y2] = img[x1, y1];
                img[x1, y1] = t;
            }
            return img;
        }

        public static ImageData min(this ImageData src)
        {
            var img = new ImageData(src.Size.Width, src.Size.Height);
            var w = src.Size.Width;
            var h = src.Size.Height;

            1.Upto(w - 2).Foreach(
                x => 1.Upto(h - 2).Foreach(
                    y =>
                        img[x, y] = 0.Upto(9).Min(a => src[x + (a % 3 - 1), y + (a / 3 - 1)])));

            return img;
        }

        public static ImageData opening(this ImageData src)
        {
            return src.min().max();
        }

        public static ImageData closing(this ImageData src)
        {
            return src.max().min();
        }

        public static ImageData blur(this ImageData src)
        {
            var img = new ImageData(src.Size.Width, src.Size.Height);
            var w = src.Size.Width;
            var h = src.Size.Height;

            1.Upto(w - 2).Foreach(
                x => 1.Upto(h - 2).Foreach(
                    y =>
                        img[x, y] = 0
                            .Upto(9)
                            .Average(a => src[x + (a % 3 - 1), y + (a / 3 - 1)])));

            return img;
        }

        public static ImageData max(this ImageData src)
        {
            var img = new ImageData(src.Size.Width, src.Size.Height);
            var w = src.Size.Width;
            var h = src.Size.Height;

            1.Upto(w - 2).Foreach(
                x => 1.Upto(h - 2).Foreach(
                    y =>
                        img[x, y] = 0.Upto(9).Max(a => src[x + (a % 3 - 1), y + (a / 3 - 1)])));

            return img;
        }

        public static ImageData median(this ImageData src)
        {
            var img = new ImageData(src.Size.Width, src.Size.Height);
            var w = src.Size.Width;
            var h = src.Size.Height;

            1.Upto(w - 2).Foreach(
                x => 1.Upto(h - 2).Foreach(
                    y =>
                        img[x, y] = 0.Upto(9)
                            .Select(a => src[x + (a % 3 - 1), y + (a / 3 - 1)])
                            .OrderBy(it => it)
                            .ElementAt(4)));

            return img;
        }

        public static ImageData randian(this ImageData src)
        {
            var rand = new Random(0);
            var img = new ImageData(src.Size.Width, src.Size.Height);
            var w = src.Size.Width;
            var h = src.Size.Height;

            1.Upto(w - 2).Foreach(
                x => 1.Upto(h - 2).Foreach(
                    y =>
                        img[x, y] = 0.Upto(9)
                            .Select(a => src[x + (a % 3 - 1), y + (a / 3 - 1)])
                            .ElementAt(rand.Next(9))));

            return img;
        }

        public static ImageData square(this ImageData src, Point location, int w)
        {
            var img = new ImageData(src);
            var radius = w / 2;

            (location.X - radius)
                .Upto(w)
                .Where(x => x >= 0 && x < src.Size.Width)
                .Foreach(x => (location.Y - radius)
                    .Upto(w)
                    .Where(y => y >= 0 && y < src.Size.Height)
                    .Foreach(y => img[x, y] = 1.0));

            return img;
        }

        public static ImageData spatter(this ImageData src, Point location, int radius)
        {
            var img = new ImageData(src);
            return img.ip_spatter(location, radius);
        }

        public static ImageData ip_bw_rand(this ImageData src, int offset = 0)
        {
            var rand = new Random(offset);
            src.iter((x, y, v) =>
            {
                var p = rand.NextDouble();


                src[x, y] = (p + v) > 1
                    ? 1 : 0;
            });
            return src;
        }

        public static ImageData ip_spatter(this ImageData src, Point location, int radius)
        {
            var rand = new Random(0);
            (location.X - radius)
                .Upto(radius * 2)
                .Where(x => x >= 0 && x < src.Size.Width)
                .Foreach(x => (location.Y - radius)
                    .Upto(radius * 2)
                    .Where(y => y >= 0 && y < src.Size.Height)
                    .Foreach(y =>
                    {
                        var dX = Math.Abs(x - location.X);
                        var dY = Math.Abs(y - location.Y);

                        var d = Math.Sqrt(dX * dX + dY * dY);
                        var p = rand.NextDouble();

                        if ((p + (1 - d / radius)) >= 0.9)
                            src[x, y] = 1;
                    }));

            return src;
        }

        public static ImageData threshold(this ImageData src, double thr)
        {
            return new ImageData(src, px => px > thr ? 1.0 : 0.0);
        }

        public static ImageData range(this ImageData src, double left, double right, Func<double, double> proc)
        {
            return new ImageData(src, px => px > right || px < left ? 0.0 : proc(px));
        }

        public static ImageData abs(this ImageData src)
        {
            return new ImageData(src, px => 2.0 * Math.Abs(px - 0.5));
        }

        public static ImageData gamma(this ImageData src, double gamma)
        {
            return new ImageData(src, px => Math.Pow(px, gamma));
        }

        public static ImageData invert(this ImageData src)
        {
            return new ImageData(src, px => 1 - px);
        }

        public static ImageData norm(this ImageData src, double min, double max)
        {
            var mn = 1000.0;
            var mx = -1000.0;

            src.iter((x, y, val) =>
            {
                if (val < mn) mn = val;
                if (val > mx) mx = val;
            });

            var offset = min - mn;
            var k = (max - min) / (mx - mn);

            return new ImageData(src, px => k * (px + offset));
        }

        public static ImageData add(this ImageData src, ImageData addition)
        {
            var pixels = new ImageData(src);
            pixels.iter((x, y, value) =>
                pixels[x, y] = fix(pixels[x, y] + addition[x, y]));
            return pixels;
        }

        public static ImageData mul(this ImageData src, ImageData addition)
        {
            var pixels = new ImageData(src);
            pixels.iter((x, y, value) =>
                pixels[x, y] = pixels[x, y] * addition[x, y]);
            return pixels;
        }

        public static ImageData distortx(this ImageData src, ImageData distortion, double scale)
        {
            var pixels = new ImageData(src);
            src.iter((x, y, value) =>
            {
                var offset = (int)Math.Round((scale * distortion[x, y]));
                var sample = x + offset >= src.Size.Width
                    ? src[src.Size.Width - 1, y]
                    : src[x + offset, y];

                pixels[x, y] = sample;
            });
            return pixels;
        }

        public static ImageData distorty(this ImageData src, ImageData distortion, int scale)
        {
            var pixels = new ImageData(src);
            src.iter((x, y, value) =>
            {
                var offset = (int)Math.Round((scale * distortion[x, y]));
                var sy = y + offset >= src.Size.Height
                    ? src.Size.Height - 1
                    : y + offset;

                var sx = x + offset >= src.Size.Width
                    ? src.Size.Width - 1
                    : x + offset;

                pixels[x, y] = src[src.Size.Width - 1 - sx, sy];
            });
            return pixels;
        }

        public static ImageData sobel(this ImageData src)
        {
            var pixels = new ImageData(src.Size.Width, src.Size.Height);
            src.iter((x, y, value) =>
            {
                if (x > 0 && x < src.Size.Width - 1 &&
                    y > 0 && y < src.Size.Height - 1)
                {
                    var a = src[x - 1, y - 1] - src[x + 1, y - 1]
                          + src[x - 1, y + 0] - src[x + 1, y + 0]
                          + src[x - 1, y + 1] - src[x + 1, y + 1];

                    var b = src[x + 1, y - 1] - src[x + 1, y + 1]
                          + src[x + 0, y - 1] - src[x + 0, y + 1]
                          + src[x - 1, y - 1] - src[x - 1, y + 1];

                    pixels[x, y] = Math.Sqrt(a * a + b * b);
                }
            });
            return pixels;
        }

        public static ImageData sub(this ImageData src, ImageData subtraction)
        {
            var pixels = new ImageData(src);
            pixels.iter((x, y, value) =>
                pixels[x, y] = fix(pixels[x, y] - subtraction[x, y]));
            return pixels;
        }

        private static double fix(double value)
        {
            if (value > 1) return 1;
            if (value < 0) return 0;
            return value;
        }


        public static ImageData floyd_steinberg(this ImageData src, double offset = 0.0)
        {
            var w = src.Size.Width;
            var h = src.Size.Height;

            var pixels = new ImageData(src);

            pixels
                .iter((x, y, value) =>
                {
                    var newval = value < 0.5 ? 0.0 : 1.0;
                    var error = (x == 0 && y == 0)
                        ? value - newval + offset
                        : value - newval;

                    pixels[x, y] = newval;

                    if (x < (w - 1)) pixels[x + 1, y + 0] += error * 7.0 / 16.0;
                    if (x > 0 && y < (h - 1)) pixels[x - 1, y + 1] += error * 3.0 / 16.0;
                    if (y < (h - 1)) pixels[x + 0, y + 1] += error * 5.0 / 16.0;
                    if (x < (w - 1) && y < (h - 1)) pixels[x + 1, y + 1] += error * 1.0 / 16.0;
                });

            return pixels;
        }

        public static Bitmap toBitmap(this ImageData src)
        {
            var bitmap = new Bitmap(src.Size.Width, src.Size.Height, PixelFormat.Format32bppArgb);
            using (var lb = new LockBitmap(bitmap, ImageLockMode.WriteOnly))
            {
                src
                    .each(x => x * 255)
                    .iter((x, y, value) =>
                        lb.SetPixel(x, y, Gray((int)value)));
            }
            return bitmap;
        }

        public static ImageData toPixels(this Bitmap src)
        {
            var pixels = new ImageData(src.Width, src.Height);

            using (var bd = new LockBitmap(src, ImageLockMode.ReadOnly))
            {
                pixels.iter((x, y, value) =>
                        pixels[x, y] = Gray(bd.GetPixel(x, y)));
            }
            return pixels;
        }

        private static double Gray(Color color)
        {
            return ((double)(7 * color.G + 2 * color.R + color.B)) / 2550.0;
        }

        private static Color Gray(int value)
        {
            if (value < 0)
                value = 0;
            else if (value > 255)
                value = 255;

            return Color.FromArgb(value, value, value);
        }

    }
}
