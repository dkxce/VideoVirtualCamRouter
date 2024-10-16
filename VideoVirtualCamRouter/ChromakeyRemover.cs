//
// ChromakeyRemover (C#)
// dkxce.ChromakeyRemover
// v 0.2, 16.10.2024
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
    public class ChromakeyRemover
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
}
