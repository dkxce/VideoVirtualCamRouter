//
// ChromakeyRemover (C#)
// dkxce.ChromakeyRemover
// v 0.5, 23.10.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System.Drawing;
using System;
using System.Linq;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Numerics;

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

    // 3D Vector
    public class RGB3DChromakeyRemover
    {
        private static double colorclose(int R, int G, int B, Color color, float fuzziness = 0.1f)
        {
            double maxDistance = fuzziness * 441.0f;
            double delta = Math.Sqrt(Math.Pow(R - color.R, 2) + Math.Pow(G - color.G, 2) + Math.Pow(B - color.B, 2));
            delta = delta / maxDistance;
            if (delta > 1) return 0;
            return 1 - delta;
        }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            if (!rbga_mask.HasValue) rbga_mask = Color.FromArgb(0, 255, 0);
            if (!full_mask.HasValue) full_mask = false;
            float fuzz = (float)fuziness / 255f;

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
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, rbga_mask.Value, fuzz)));
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
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * 255, 0) + mask * 0), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, rbga_mask.Value, fuzz)));
            };

            return (width, height, rgba_foreground);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, fuziness, rbga_mask, transparentColor, full_mask);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }

    // Grayscale proximity
    public class GrayScaleChromakeyRemover
    {
        private static double RGB2toGray(System.Drawing.Color c) => .11 * c.B + .59 * c.G + .30 * c.R;
        private static double RGB2toGray(int B, int G, int R) => .11 * B + .59 * G + .30 * R;
        private static double colorclose(double gray1, double gray2, float fuzziness = 0.1f)
        {
            double maxDistance = fuzziness * 255.0f;
            double delta = (gray1 - gray2) / maxDistance;
            if (delta > 1) return 0;
            return 1 - delta;
        }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            double mask_color = RGB2toGray(rbga_mask.HasValue ? rbga_mask.Value : Color.Green);
            if (!full_mask.HasValue) full_mask = false;
            float fuzz = (float)fuziness / 255f;

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
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(mask_color, RGB2toGray(pixel.R, pixel.G, pixel.B), fuzz)));
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
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * 255, 0) + mask * 0), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(mask_color, RGB2toGray(pixel.R, pixel.G, pixel.B), fuzz)));
            };

            return (width, height, rgba_foreground);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, fuziness, rbga_mask, transparentColor, full_mask);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }

    // https://www.compuphase.com/cmetric.htm
    public class ColorMetricChromakeyRemover
    {
        private static double colorclose(Color color1, Color color2, float fuzziness = 0.1f)
        {
            long rmean = ((long)color1.R + (long)color2.R) / 2;
            long r = (long)color1.R - (long)color2.R;
            long g = (long)color1.G - (long)color2.G;
            long b = (long)color1.B - (long)color2.B;
            double dist = Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
            return dist;
        }

        private static double colorclose(int R, int G, int B, Color color2, float fuzziness = 0.1f)
        {
            long rmean = ((long)R + (long)color2.R) / 2;
            long r = (long)R - (long)color2.R;
            long g = (long)G - (long)color2.G;
            long b = (long)B - (long)color2.B;
            
            double dist = Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));

            double maxDistance = fuzziness * 767;
            double delta = dist / maxDistance;
            if (delta > 1) return 0;
            return 1 - delta;
        }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            if (!rbga_mask.HasValue) rbga_mask = Color.FromArgb(0, 255, 0);
            if (!full_mask.HasValue) full_mask = false;
            float fuzz = (float)fuziness / 255f;

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
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, rbga_mask.Value, fuzz)));
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
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * 255, 0) + mask * 0), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, rbga_mask.Value, fuzz)));
            };

            return (width, height, rgba_foreground);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, fuziness, rbga_mask, transparentColor, full_mask);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }

    // http://www.brucelindbloom.com/
    public class LABChromakeyRemover
    {
        public static Vector4 RGBToXYZ(Color color)
        {
            double var_R = color.R / 255f;
            double var_G = color.G / 255f;
            double var_B = color.B / 255f;

            if (var_R > 0.04045) var_R = Math.Pow(((var_R + 0.055) / 1.055), 2.4f);
            else var_R = var_R / 12.92;
            if (var_G > 0.04045) var_G = Math.Pow(((var_G + 0.055) / 1.055), 2.4f);
            else var_G = var_G / 12.92;
            if (var_B > 0.04045) var_B = Math.Pow(((var_B + 0.055) / 1.055), 2.4f);
            else var_B = var_B / 12.92;

            var_R *= 100;
            var_G *= 100;
            var_B *= 100;

            //Observer. = 2°, Illuminant = D65
            double X = var_R * 0.4124 + var_G * 0.3576 + var_B * 0.1805;
            double Y = var_R * 0.2126 + var_G * 0.7152 + var_B * 0.0722;
            double Z = var_R * 0.0193 + var_G * 0.1192 + var_B * 0.9505;
            return new Vector4((float)X, (float)Y, (float)Z, color.A);
        }

        public static Vector4 RGBToXYZ(double var_R, double var_G, double var_B)
        {
            var_R = var_R / 255f;
            var_G = var_G / 255f;
            var_B = var_B / 255f;

            if (var_R > 0.04045) var_R = Math.Pow(((var_R + 0.055) / 1.055), 2.4f);
            else var_R = var_R / 12.92;
            if (var_G > 0.04045) var_G = Math.Pow(((var_G + 0.055) / 1.055), 2.4f);
            else var_G = var_G / 12.92;
            if (var_B > 0.04045) var_B = Math.Pow(((var_B + 0.055) / 1.055), 2.4f);
            else var_B = var_B / 12.92;

            var_R *= 100;
            var_G *= 100;
            var_B *= 100;

            //Observer. = 2°, Illuminant = D65
            double X = var_R * 0.4124 + var_G * 0.3576 + var_B * 0.1805;
            double Y = var_R * 0.2126 + var_G * 0.7152 + var_B * 0.0722;
            double Z = var_R * 0.0193 + var_G * 0.1192 + var_B * 0.9505;
            return new Vector4((float)X, (float)Y, (float)Z, 255);
        }

        public static Vector4 XYZToLAB(Vector4 xyz)
        {
            double var_X = xyz.X / 095.047; //ref_X = 095.047   Observer= 2°, Illuminant= D65
            double var_Y = xyz.Y / 100.000; //ref_Y = 100.000
            double var_Z = xyz.Z / 108.883; //ref_Z = 108.883

            if (var_X > 0.008856) var_X = Math.Pow(var_X, 1/3f);
            else var_X = (7.787 * var_X) + (16 / 116);
            if (var_Y > 0.008856) var_Y = Math.Pow(var_Y, 1/3f);
            else var_Y = (7.787 * var_Y) + (16 / 116);
            if (var_Z > 0.008856) var_Z = Math.Pow(var_Z, 1/3f);
            else var_Z = (7.787 * var_Z) + (16 / 116);

            double CIE_L = (116 * var_Y) - 16;
            double CIE_a = 500 * (var_X - var_Y);
            double CIE_b = 200 * (var_Y - var_Z);

            return new Vector4((float)CIE_L, (float)CIE_a, (float)CIE_b, xyz.W);
        }

        public static Vector4 RGBToLAB(Color color) => XYZToLAB(RGBToXYZ(color));

        public static Vector4 RGBToLAB(double var_R, double var_G, double var_B) => XYZToLAB(RGBToXYZ(var_R, var_G, var_B));


        public static double DeltaE2000(Vector4 Lab1, Vector4 Lab2)
        {
            double kL = 1.0f;
            double kC = 1.0f;
            double kH = 1.0f;
            double lBarPrime = 0.5 * (Lab1.X + Lab2.X);
            double c1 = Math.Sqrt(Lab1.Y * Lab1.Y + Lab1.Z * Lab1.Z);
            double c2 = Math.Sqrt(Lab2.Y * Lab2.Y + Lab2.Z * Lab2.Z);
            double cBar = 0.5 * (c1 + c2);
            double cBar7 = cBar * cBar * cBar * cBar * cBar * cBar * cBar;
            double g = 0.5 * (1.0 - Math.Sqrt(cBar7 / (cBar7 + 6103515625.0)));  /* 6103515625 = 25^7 */
            double a1Prime = Lab1.Y * (1.0 + g);
            double a2Prime = Lab2.Y * (1.0 + g);
            double c1Prime = Math.Sqrt(a1Prime * a1Prime + Lab1.Z * Lab1.Z);
            double c2Prime = Math.Sqrt(a2Prime * a2Prime + Lab2.Z * Lab2.Z);
            double cBarPrime = 0.5 * (c1Prime + c2Prime);
            double h1Prime = (Math.Atan2(Lab1.Z, a1Prime) * 180.0) / Math.PI;
            double dhPrime; // not initialized on purpose

            if (h1Prime < 0.0)
                h1Prime += 360.0;
            double h2Prime = (Math.Atan2(Lab2.Z, a2Prime) * 180.0) / Math.PI;
            if (h2Prime < 0.0)
                h2Prime += 360.0;
            double hBarPrime = (Math.Abs(h1Prime - h2Prime) > 180.0) ? (0.5 * (h1Prime + h2Prime + 360.0)) : (0.5 * (h1Prime + h2Prime));
            double t = 1.0 -
            0.17 * Math.Cos(Math.PI * (hBarPrime - 30.0) / 180.0) +
            0.24 * Math.Cos(Math.PI * (2.0 * hBarPrime) / 180.0) +
            0.32 * Math.Cos(Math.PI * (3.0 * hBarPrime + 6.0) / 180.0) -
            0.20 * Math.Cos(Math.PI * (4.0 * hBarPrime - 63.0) / 180.0);
            if (Math.Abs(h2Prime - h1Prime) <= 180.0)
                dhPrime = h2Prime - h1Prime;
            else
                dhPrime = (h2Prime <= h1Prime) ? (h2Prime - h1Prime + 360.0) : (h2Prime - h1Prime - 360.0);
            double dLPrime = Lab2.X - Lab1.Y;
            double dCPrime = c2Prime - c1Prime;
            double dHPrime = 2.0 * Math.Sqrt(c1Prime * c2Prime) * Math.Sin(Math.PI * (0.5 * dhPrime) / 180.0);
            double sL = 1.0 + ((0.015 * (lBarPrime - 50.0) * (lBarPrime - 50.0)) / Math.Sqrt(20.0 + (lBarPrime - 50.0) * (lBarPrime - 50.0)));
            double sC = 1.0 + 0.045 * cBarPrime;
            double sH = 1.0 + 0.015 * cBarPrime * t;
            double dTheta = 30.0 * Math.Exp(-((hBarPrime - 275.0) / 25.0) * ((hBarPrime - 275.0) / 25.0));
            double cBarPrime7 = cBarPrime * cBarPrime * cBarPrime * cBarPrime * cBarPrime * cBarPrime * cBarPrime;
            double rC = Math.Sqrt(cBarPrime7 / (cBarPrime7 + 6103515625.0));
            double rT = -2.0 * rC * Math.Sin(Math.PI * (2.0 * dTheta) / 180.0);
            double res = (Math.Sqrt(
                               (dLPrime / (kL * sL)) * (dLPrime / (kL * sL)) +
                               (dCPrime / (kC * sC)) * (dCPrime / (kC * sC)) +
                               (dHPrime / (kH * sH)) * (dHPrime / (kH * sH)) +
                               (dCPrime / (kC * sC)) * (dHPrime / (kH * sH)) * rT
                          )
             );
            return res;
        }

        private static double colorclose(int R, int G, int B, Color color, float fuzziness = 0.1f)
        {            
            double delta = DeltaE2000(RGBToLAB(R, G, B), RGBToLAB(color));

            double maxDistance = fuzziness * 512f;
            delta = delta / maxDistance;
            if (delta > 1) return 0;
            return 1 - delta;
        }

        private static double colorclose(int R, int G, int B, Vector4 color, float fuzziness = 0.1f)
        {
            double delta = DeltaE2000(RGBToLAB(R, G, B), color);

            double maxDistance = fuzziness * 441f;
            delta = delta / maxDistance;
            if (delta > 1) return 0;
            return 1 - delta;
        }

        private static double colorclose(int R, int G, int B, ColorMine.ColorSpaces.Rgb color, float fuzziness = 0.1f, byte method = 0)
        {
            ColorMine.ColorSpaces.Rgb rgb1 = new ColorMine.ColorSpaces.Rgb() { R = R, B = B, G = G };
            ColorMine.ColorSpaces.Rgb rgb2 = new ColorMine.ColorSpaces.Rgb() { R = color.R, B = color.B, G = color.G };
            
            double delta = 0;
            if (method == 0) delta = rgb1.Compare(rgb2, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison());
            if (method == 3) delta = rgb1.Compare(rgb2, new ColorMine.ColorSpaces.Comparisons.CmcComparison());

            double maxDistance = fuzziness * 255f;
            delta = delta / maxDistance;
            if (delta > 1) return 0;
            return 1 - delta;
        }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false, byte method = 3)
        {
            if (!rbga_mask.HasValue) rbga_mask = Color.FromArgb(0, 255, 0);
            object mask_color = null;
            if (method == 0 || method == 3) mask_color = new ColorMine.ColorSpaces.Rgb() { R = rbga_mask.Value.R, B = rbga_mask.Value.B, G = rbga_mask.Value.G };
            else mask_color = RGBToLAB(rbga_mask.Value);
            if (!full_mask.HasValue) full_mask = false;
            float fuzz = (float)fuziness / 255f;

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
                pixels.AsParallel().ForAll(pixel =>
                {
                    if (method == 0 || method == 3)
                        pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, (ColorMine.ColorSpaces.Rgb)mask_color, fuzz, method));
                    else
                        pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, (Vector4)mask_color, fuzz));
                });
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
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * 255, 0) + mask * 0), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel =>
                {
                    if (method == 0 || method == 3)
                        pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, (ColorMine.ColorSpaces.Rgb)mask_color, fuzz, method));
                    else
                        pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, (Vector4)mask_color, fuzz));
                });
            };

            return (width, height, rgba_foreground);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image,
            int fuziness = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, fuziness, rbga_mask, transparentColor, full_mask);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }

    public class HSVChromakeyRemover
    {
        private static float GetHue(int R, int G, int B)
        {
            if (R == G && G == B)
            {
                return 0f;
            }

            float num = (float)(int)R / 255f;
            float num2 = (float)(int)G / 255f;
            float num3 = (float)(int)B / 255f;
            float num4 = 0f;
            float num5 = num;
            float num6 = num;
            if (num2 > num5)
            {
                num5 = num2;
            }

            if (num3 > num5)
            {
                num5 = num3;
            }

            if (num2 < num6)
            {
                num6 = num2;
            }

            if (num3 < num6)
            {
                num6 = num3;
            }

            float num7 = num5 - num6;
            if (num == num5)
            {
                num4 = (num2 - num3) / num7;
            }
            else if (num2 == num5)
            {
                num4 = 2f + (num3 - num) / num7;
            }
            else if (num3 == num5)
            {
                num4 = 4f + (num - num2) / num7;
            }

            num4 *= 60f;
            if (num4 < 0f)
            {
                num4 += 360f;
            }

            return num4;
        }

        public static Vector4 rgbToHSV(Color color)
        {
            double[] output = new double[3];
            double hue, saturation, value;
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = GetHue(color.R, color.G, color.B);
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;

            Vector4 res = new Vector4((float)hue, (float)saturation, (float)value, 0f);
            return res;
        }

        public static Vector4 rgbToHSV(int R, int G, int B)
        {
            double[] output = new double[3];
            double hue, saturation, value;
            int max = Math.Max(R, Math.Max(G, B));
            int min = Math.Min(R, Math.Min(G, B));

            hue = GetHue(R, G, B);
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;

            Vector4 res = new Vector4((float)hue, (float)saturation, (float)value, 0f);
            return res;
        }

        private static double colorclose(int R, int G, int B, Vector4 c2, float min_fuz = 0, float max_fuz = 10, float fuzziness = 0.1f, byte mode = 1)
        {
            Vector4 c1 = rgbToHSV(R, G, B);
            double delta = Math.Min(Math.Abs(c1.X - c2.X), 360 - Math.Abs(c1.X - c2.X));

            if (mode == 3)
            {
                double dh = delta / 180.0f;
                double ds = Math.Abs(c1.Y - c2.Y);
                double dv = Math.Abs(c1.Z - c2.Z) / 255.0f;
                double distance = Math.Sqrt(dh * dh + ds * ds + dv * dv);
                delta = distance / fuzziness;
                if (delta <= min_fuz / 25.5f) return 1;
                if (delta >= max_fuz / 25.5f) return 0;
            }
            if (mode == 2)
            {
                if (delta <= min_fuz) return 1;
                if (delta >= max_fuz) return 0;
                delta = delta / 180f;
            }
            if (mode == 1)
            {
                double dh = delta / 180.0f;
                double ds = Math.Abs(c1.Y - c2.Y);
                double dv = Math.Abs(c1.Z - c2.Z) / 255.0f;
                double distance = Math.Sqrt(dh * dh + ds * ds + dv * dv);
                delta = distance / fuzziness;
            }
            else
            {
                double maxDistance = fuzziness * 180.0f;
                delta = delta / maxDistance;
            };

            if (delta > 1) return 0;
            return 1 - delta;
        }

        public static (int, int, byte[]) RemoveChromaKey2Bytes(Bitmap image,
            int min_fuzzi, int max_fuzzi = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false, byte mode = 2)
        {
            if (!rbga_mask.HasValue) rbga_mask = Color.FromArgb(0, 255, 0);
            if (!full_mask.HasValue) full_mask = false;
            float fuzz = (float)max_fuzzi / 255f;
            float min_fuzz = min_fuzzi / 10.0f;
            float max_fuzz = max_fuzzi / 10.0f;

            Vector4 mask_color = rgbToHSV(rbga_mask.Value);
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
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, mask_color, min_fuzz, max_fuzz, fuzz, mode)));
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
                        rgba_foreground[(x * 4) + 3] = (byte)Math.Min((Math.Max(rgba_foreground[(x * 4) + 3] - mask * 255, 0) + mask * 0), 255); //A
                    })
                });
                pixels.AsParallel().ForAll(pixel => pixel.MakeTransparent(colorclose(pixel.R, pixel.G, pixel.B, mask_color, min_fuzz, max_fuzz, fuzz, mode)));
            };

            return (width, height, rgba_foreground);
        }

        public static Bitmap RemoveChromaKey2Bitmap(Bitmap image,
            int min_fuzz, int max_fuzz = 96,
            Color? rbga_mask = null, Color? transparentColor = null,
            bool? full_mask = false, byte mode = 2)
        {
            int width, height;
            byte[] rgba;

            (width, height, rgba) = RemoveChromaKey2Bytes(image, min_fuzz, max_fuzz, rbga_mask, transparentColor, full_mask, mode);

            Bitmap output = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData oData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(rgba, 0, oData.Scan0, rgba.Length);
            output.UnlockBits(oData);
            return output;
        }
    }
}
