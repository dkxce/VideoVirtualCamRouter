//
// ChromakeyRemover (C#)
// dkxce.ChromakeyRemover
// v 0.3, 22.10.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System.Drawing;
using System;
using System.Linq;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace dkxce
{
    public class RGBChromakeyRemover
    {
        public enum Channel: int { Red = 0, Green = 1, Blue = 2 }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image, 
            int velocity = 8, int threshold = 96,
            Channel? channel = Channel.Green, Color? transparentColor = null, 
            int? min_threshold = null)
        {
            float delta_threshold = min_threshold.HasValue ? 1.0f / ((float)threshold - (float)min_threshold) : 0.0f;

            int width = image.Width; 
            int height = image.Height;
            byte[] rgba;
            
            // Get Bitmap Array
            {
                BitmapData iData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int bytes = Math.Abs(iData.Stride) * image.Height;
                rgba = new byte[bytes];
                Marshal.Copy(iData.Scan0, rgba, 0, bytes);
                image.UnlockBits(iData);
            };

            if (transparentColor.HasValue)
            {
                var pixels = Enumerable.Range(0, rgba.Length / 4).Select(x => new
                {
                    B = rgba[x * 4],
                    G = rgba[(x * 4) + 1],
                    R = rgba[(x * 4) + 2],
                    A = rgba[(x * 4) + 3],
                    MakeTransparent = new Action<float>((alpha) =>
                    {
                        if (alpha >= 1)
                        {
                            rgba[(x * 4)] = transparentColor.Value.B;
                            rgba[(x * 4) + 1] = transparentColor.Value.G;
                            rgba[(x * 4) + 2] = transparentColor.Value.R;
                            rgba[(x * 4) + 3] = transparentColor.Value.A;
                        }
                        else
                        {
                            rgba[(x * 4)] = (byte)(transparentColor.Value.B * alpha + rgba[x * 4] * (1 - alpha));
                            rgba[(x * 4) + 1] = (byte)(transparentColor.Value.G * alpha + rgba[(x * 4) + 1] * (1 - alpha));
                            rgba[(x * 4) + 2] = (byte)(transparentColor.Value.R * alpha + rgba[(x * 4) + 2] * (1 - alpha));
                            rgba[(x * 4) + 3] = (byte)(transparentColor.Value.A * alpha + rgba[(x * 4) + 3] * (1 - alpha));
                        };
                    })
                });
                pixels.AsParallel().ForAll(pixel =>
                {
                    byte max = Math.Max(Math.Max(pixel.R, pixel.G), pixel.B);
                    byte min = Math.Min(Math.Min(pixel.R, pixel.G), pixel.B);
                    byte pColor = pixel.G;
                    if (channel.HasValue)
                    {
                        if (channel == Channel.Red) pColor = pixel.R;
                        if (channel == Channel.Blue) pColor = pixel.B;
                    };
                    if (pColor != min && (pColor == max || max - pColor < velocity))
                    {
                        byte mm = (byte)(max - min);
                        if (mm > threshold) pixel.MakeTransparent(1.0f);
                        else if (min_threshold.HasValue && mm >= min_threshold) pixel.MakeTransparent((mm - min_threshold.Value) * delta_threshold);
                    };
                });
            }
            else
            {
                var pixels = Enumerable.Range(0, rgba.Length / 4).Select(x => new
                {
                    B = rgba[x * 4],
                    G = rgba[(x * 4) + 1],
                    R = rgba[(x * 4) + 2],
                    A = rgba[(x * 4) + 3],
                    //MakeTransparent = new Action(() => rgba[(x * 4) + 3] = 0),
                    MakeTransparent = new Action<float>((a) => rgba[(x * 4) + 3] = (byte)((float)rgba[(x * 4) + 3] * a)) // or a/2 .. a/5
                });
                pixels.AsParallel().ForAll(pixel =>
                {
                    byte max = Math.Max(Math.Max(pixel.R, pixel.B), pixel.G);
                    byte min = Math.Min(Math.Min(pixel.R, pixel.B), pixel.G);
                    byte pColor = pixel.G;
                    if (channel.HasValue)
                    {
                        if (channel == Channel.Red) pColor = pixel.R;
                        if (channel == Channel.Blue) pColor = pixel.B;
                    };
                    if (pColor != min && (pColor == max || max - pColor < velocity))
                    {
                        byte mm = (byte)(max - min);
                        if (mm > threshold) pixel.MakeTransparent(0.0f);
                        else if (min_threshold.HasValue && mm >= min_threshold) pixel.MakeTransparent(1.0f - ((mm - min_threshold.Value) * delta_threshold));
                    };
                });
            };

            return (width, height, rgba);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image, 
            int velocity = 8, int threshold = 96,
            Channel? channel = Channel.Green, Color? transparentColor = null,
            int? min_threshold = null)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, velocity, threshold, channel, transparentColor, min_threshold);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }

    // http://gc-films.com/chromakey.html
    public class YCbCrChromakeyRemover
    {
        private static int rgb2y(int r, int g, int b) => (int)Math.Round(0.299 * (double)r + 0.587 * (double)g + 0.114 * (double)b);

        private static int rgb2cb(int r, int g, int b) => (int)Math.Round(128.0 + -0.168736 * (double)r - 0.331264 * (double)g + 0.5 * (double)b);

        private static int rgb2cr(int r, int g, int b) => (int)Math.Round(128.0 + 0.5 * (double)r - 0.418688 * (double)g - 0.081312 * (double)b);

        private static double colorclose(int Cb_p, int Cr_p, int Cb_key, int Cr_key, int tola, int tolb)
        {
            double d = Math.Sqrt(Math.Pow(Cb_key - Cb_p, 2) + Math.Pow(Cr_key - Cr_p, 2));
            return 1 - (d < tola ? 0 : d > tolb ? 1 : (d - tola) / (tolb - tola));
        }

        private static double colorclose(Color remColor, int Cb_key, int Cr_key, int tola, int tolb)
        {
            int Cb_p = rgb2cb(remColor.R, remColor.G, remColor.B);
            int Cr_p = rgb2cr(remColor.R, remColor.G, remColor.B);
            double d = Math.Sqrt(Math.Pow(Cb_key - Cb_p, 2) + Math.Pow(Cr_key - Cr_p, 2));
            return 1 - (d < tola ? 0 : d > tolb ? 1 : (d - tola) / (tolb - tola));
        }

        private static double colorclose(int R, int G, int B, int Cb_key, int Cr_key, int tola, int tolb)
        {
            int Cb_p = rgb2cb(R, G, B);
            int Cr_p = rgb2cr(R, G, B);
            double d = Math.Sqrt(Math.Pow(Cb_key - Cb_p, 2) + Math.Pow(Cr_key - Cr_p, 2));
            return 1 - (d < tola ? 0 : d > tolb ? 1 : (d - tola) / (tolb - tola));
        }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image,
            int min_treshold = 50, int max_treshold = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            if (!rbga_mask.HasValue) rbga_mask = Color.FromArgb(0, 255, 0);
            if (!full_mask.HasValue) full_mask = false;
            int cb_mask = rgb2cb(rbga_mask.Value.R, rbga_mask.Value.G, rbga_mask.Value.B);
            int cr_mask = rgb2cr(rbga_mask.Value.R, rbga_mask.Value.G, rbga_mask.Value.B);

            int width = image.Width;
            int height = image.Height;
            byte[] rgba_foreground;

            // Get Bitmap Array
            {
                BitmapData iData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int bytes = Math.Abs(iData.Stride) * image.Height;
                rgba_foreground = new byte[bytes];
                Marshal.Copy(iData.Scan0, rgba_foreground, 0, bytes);
                image.UnlockBits(iData);
            };

            if (transparentColor.HasValue)
            {
                var pixels = Enumerable.Range(0, rgba_foreground.Length / 4).Select(x => new
                {
                    B = rgba_foreground[(x * 4)],
                    G = rgba_foreground[(x * 4) + 1],
                    R = rgba_foreground[(x * 4) + 2],
                    MakeTransparent = new Action<double>((mask) =>
                    {
                        if (full_mask.Value)
                        {
                            rgba_foreground[(x * 4)] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4)] - mask * rbga_mask.Value.B, 0) + mask * transparentColor.Value.B), 255); //B
                            rgba_foreground[(x * 4) + 1] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 1] - mask * rbga_mask.Value.G, 0) + mask * transparentColor.Value.G), 255); //G
                            rgba_foreground[(x * 4) + 2] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 2] - mask * rbga_mask.Value.R, 0) + mask * transparentColor.Value.R), 255); //R
                        };
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * rbga_mask.Value.A, 0) + mask * transparentColor.Value.A), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, cb_mask, cr_mask, min_treshold, max_treshold)));
            }
            else
            {
                var pixels = Enumerable.Range(0, rgba_foreground.Length / 4).Select(x => new
                {
                    B = rgba_foreground[(x * 4)],
                    G = rgba_foreground[(x * 4) + 1],
                    R = rgba_foreground[(x * 4) + 2],
                    MakeTransparent = new Action<double>((mask) =>
                    {
                        if (full_mask.Value)
                        {
                            rgba_foreground[(x * 4)] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4)] - mask * rbga_mask.Value.B, 0) + mask * 0), 255); //B
                            rgba_foreground[(x * 4) + 1] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 1] - mask * rbga_mask.Value.G, 0) + mask * 0), 255); //G
                            rgba_foreground[(x * 4) + 2] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 2] - mask * rbga_mask.Value.R, 0) + mask * 0), 255); //R
                        };
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * 255,               0) + mask * 0), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, cb_mask, cr_mask, min_treshold, max_treshold)) );
            };

            return (width, height, rgba_foreground);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image,
            int min_treshold = 50, int max_treshold = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, min_treshold, max_treshold, rbga_mask, transparentColor, full_mask);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }
}
