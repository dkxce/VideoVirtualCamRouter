//
// RealCamToVirtualCamRouter (C#)
// dkxce.RealCamToVirtualCamRouter
// v 0.4, 23.10.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

// using OpenCVSharp
using OpenCvSharp; 

// using SixLabors.ImageSharp
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

namespace dkxce.Chromakeys
{
    public static class RealCamToVirtualCamRouter
    {
        private static Bitmap buffer_frame = null;
        private static Mutex buffer_mutex = new Mutex();
        private static SixLabors.ImageSharp.Image background;
        private static Mutex background_mutex = new Mutex();
        private static System.Drawing.Image preview_image = null;
        private static Mutex preview_mutex = new Mutex();

        // Using:
        //  https://github.com/webcamoid/akvirtualcamera
        // Need Camera Settings:
        //  AkVCamManager add-device "VirtualCamera0"
        //  AkVCamManager set-description VirtualCamera0 "dkxce.RealCamToVirtualCamRouter"
        //  AkVCamManager supported-formats --output
        //  AkVCamManager add-format VirtualCamera0 RGB24 720 480 25
        //  AkVCamManager add-format VirtualCamera0 RGB24 640 480 25        
        //  AkVCamManager set-picture "C:\Disk2\Images\il_1588xN.4792548473_b52j_640x480.png"
        //  AkVCamManager update
        //  AkVCamManager devices

        public static string akvcamman_path = @"C:\Program Files\AkVirtualCamera\x64\AkVCamManager.exe";
        public static string background_image = @"C:\Disk2\Images\il_1588xN.4792548473_b52j_640x480.png";
        public static bool use_background = false;
        public static bool use_ovelay = false;
        public static bool use_preview = true;
        public static bool use_virtcam = true;
        public static int use_method = 0;
        public static int ycbcr_color = -9109505;
        public static System.Drawing.Color background_color = System.Drawing.Color.Black;

        public static bool chromakey_remove = false;
        public static byte min_treshold = 8;    // recommended value: 8
        public static byte max_treshold = 60;   // recommended value: 96
        public static RGBChromakeyRemover.Channel chromakey_channel = RGBChromakeyRemover.Channel.Green;
        
        public static int virtualcam_fps = 25;
        public static int realcam_num = 0;       
        public static string virtualcam_name = "VirtualCamera0";

        public static int default_width = 720;
        public static int default_height = 480;
        public static bool in_progress = false;

        public static int curr_wi = 720;
        public static int curr_he = 480;

        //Async Entry Point
        public static void Start()
        {
            new Thread(() => Route()).Start();
        }

        public static void Stop()
        {
            in_progress = false;
        }

        public static void ChangeValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (name == "ycbcr_color") ycbcr_color = (int)value;
            if (name == "method") use_method = (int)value;
            if (name == "use_virtcam") use_virtcam = (bool)value;
            if (name == "use_preview") use_preview = (bool)value;
            if (name == "ovelay") use_ovelay = (bool)value;
            if (name == "chromakey_remove") chromakey_remove = (bool)value;
            if (name == "min_treshold") min_treshold = (byte)value;
            if (name == "max_treshold") max_treshold = (byte)value;
            if (name == "channel") chromakey_channel = (RGBChromakeyRemover.Channel)(int)value;            
            if (name == "use_background") use_background = (bool)value;
            if (name == "background_color") background_color = (System.Drawing.Color)value;
            if (name == "background")
            {
                string bg = (string)value;
                background_mutex.WaitOne();
                if (background != null) background.Dispose();
                if (!string.IsNullOrEmpty(bg) && File.Exists(bg))
                {
                    background = SixLabors.ImageSharp.Image.Load(background_image = bg);
                    int[] wehi = GetNewSizeKeepAspectRatio(background.Width, background.Height, curr_wi, curr_he);
                    background.Mutate(i => i.Resize(wehi[0], wehi[1]));
                    if (curr_wi < background.Width || background.Height < wehi[1])
                    {
                        int cx = (background.Width - curr_wi) / 2;
                        int cy = (background.Height - curr_he) / 2;
                        background.Mutate(i => i.Crop(new SixLabors.ImageSharp.Rectangle(cx, cy, curr_wi, curr_he)));
                    };
                };
                background_mutex.ReleaseMutex();
            };
        }

        // Entry Point
        public static void Route()
        {
            in_progress = true;
            using (var vid = new VideoCapture(realcam_num, VideoCaptureAPIs.DSHOW))
            {
                vid.Set(VideoCaptureProperties.FrameWidth, default_width);
                vid.Set(VideoCaptureProperties.FrameHeight, default_height);

                if (!vid.IsOpened())
                    throw new System.Exception("Failed to open camera!");

                bool firstRun = true;
                while (in_progress)
                {
                    using (Mat frame = new Mat())
                    {
                        if (!vid.Read(frame))   break;

                        Bitmap img;
                        using (var ms = new MemoryStream(frame.ToBytes()))
                        {
                            ms.Position = 0;
                            img = (Bitmap)Bitmap.FromStream(ms);
                        };

                        if (firstRun)
                        {
                            LaunchThread(img.Width, img.Height);
                            firstRun = false;
                        };

                        buffer_mutex.WaitOne();
                        if (buffer_frame != null) buffer_frame.Dispose();
                        buffer_frame = img;
                        buffer_mutex.ReleaseMutex();                                             
                    };
                };
            };
        }
        
        private static void LaunchThread(int width, int height)
        {
            Thread thr = new Thread(() => RouteThread((object)(new int[] { width, height })));
            thr.Start();
        }

        private static void RouteThread(object wihe)
        {
            int width = curr_wi = ((int[])wihe)[0];
            int height = curr_he = ((int[])wihe)[1];            

            // Init & Resize Background Image
            background_mutex.WaitOne();
            if ((!string.IsNullOrEmpty(background_image)) && File.Exists(background_image))
            {
                if (background != null) background.Dispose();
                background = SixLabors.ImageSharp.Image.Load(background_image);
                int[] wehi = GetNewSizeKeepAspectRatio(background.Width, background.Height, curr_wi, curr_he);
                background.Mutate(i => i.Resize(wehi[0], wehi[1]));
                if (curr_wi < background.Width || background.Height < wehi[1])
                {
                    int cx = (background.Width -  curr_wi) / 2;
                    int cy = (background.Height -  curr_he) / 2;
                    background.Mutate(i => i.Crop(new SixLabors.ImageSharp.Rectangle(cx,cy,curr_wi, curr_he)));
                };
            };
            background_mutex.ReleaseMutex();

            // Init Frame Rate
            DateTime lastLoop = DateTime.MinValue;
            double delay = 1000 / virtualcam_fps;

            Process akvcamproc = null;

            // Result Video Loop
            while (in_progress)
            {
                if ((DateTime.UtcNow - lastLoop).TotalMilliseconds < delay) continue;       
                
                if(use_virtcam && akvcamproc == null)
                {
                    // Init AKVCamManager
                    ProcessStartInfo psi = new ProcessStartInfo(akvcamman_path, string.Format("stream --fps {0} {1} RGB24 {2} {3}", virtualcam_fps, virtualcam_name, width, height));
                    psi.RedirectStandardInput = true;
                    psi.UseShellExecute = false;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.CreateNoWindow = true;
                    akvcamproc = Process.Start(psi);
                };

                // Read Buffer
                buffer_mutex.WaitOne();
                Bitmap img = buffer_frame;
                buffer_frame = null;
                buffer_mutex.ReleaseMutex();
                if (img == null) continue;
                lastLoop = DateTime.UtcNow;

                curr_wi = img.Width;
                curr_he = img.Height;


                byte[] raw_data;
                int imline = 0;

                // Remove Background & Get Image Data
                if (chromakey_remove)
                {
                    if (use_method == 1)
                        (width, height, raw_data) = ChromakeyRemover<YCbCrChromakeyRemover>.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color));
                    else if (use_method == 2)
                        (width, height, raw_data) = ChromakeyRemover<RGB3DChromakeyRemover>.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color));
                    else if (use_method == 3)
                        (width, height, raw_data) = ChromakeyRemover<GrayScaleChromakeyRemover>.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color));
                    else if (use_method == 4)
                        (width, height, raw_data) = ColorMetricChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold,max_treshold, System.Drawing.Color.FromArgb(ycbcr_color));
                    else if (use_method == 5)
                        (width, height, raw_data) = LABChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 0);
                    else if (use_method == 6)
                        (width, height, raw_data) = LABChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 3); // 2 is bad
                    else if (use_method == 7)
                        (width, height, raw_data) = LABChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 3);
                    else if (use_method == 8)
                        (width, height, raw_data) = HSVChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 0);
                    else if (use_method == 9)
                        (width, height, raw_data) = HSVChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 1);
                    else if (use_method == 10)
                        (width, height, raw_data) = HSVChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 2);
                    else if (use_method == 11)
                        (width, height, raw_data) = HSVChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, System.Drawing.Color.FromArgb(ycbcr_color), null, false, 3);
                    else 
                        (width, height, raw_data) = RGBChromakeyRemover.RemoveChromaKey2Bytes(img, min_treshold, max_treshold, chromakey_channel);

                }
                else // Get Image Data
                {
                    width = img.Width;
                    height = img.Height;
                    BitmapData iData = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    raw_data = new byte[Math.Abs(iData.Stride) * img.Height];
                    Marshal.Copy(iData.Scan0, raw_data, 0, raw_data.Length);
                    img.UnlockBits(iData);
                };
                img.Dispose();

                // Get Result Frame
                SixLabors.ImageSharp.Image <Rgb24> result_frame = null;
                background_mutex.WaitOne();
                if (use_background && background != null)
                    result_frame = background.CloneAs<Rgb24>();
                else
                    result_frame = new SixLabors.ImageSharp.Image<Rgb24>(curr_wi, curr_he, 
                        SixLabors.ImageSharp.Color.FromRgba(background_color.R, background_color.G, background_color.B, background_color.A));
                background_mutex.ReleaseMutex();

                // Add Overlay
                if (use_ovelay)
                {
                    SixLabors.ImageSharp.Image ovelay = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(raw_data, width, height);
                    result_frame.Mutate(i => i.DrawImage(ovelay, new SixLabors.ImageSharp.Point(0, 0), 1f));
                    ovelay.Dispose();
                };

                // Get RAW Data
                imline = result_frame.Width * 3;
                raw_data = new byte[imline * result_frame.Height];
                result_frame.CopyPixelDataTo(raw_data);

                if(use_preview)
                {
                    preview_mutex.WaitOne();
                    if(preview_image != null) preview_image.Dispose();
                    using (MemoryStream ms = new MemoryStream()) 
                    {
                        result_frame.SaveAsPng(ms);
                        ms.Position = 0;
                        preview_image = System.Drawing.Image.FromStream(ms);
                    };
                    preview_mutex.ReleaseMutex();
                };

                // Clear Mem
                result_frame.Dispose();

                if (use_virtcam && akvcamproc != null)
                {
                    // Send Horizontal Lines to Virtual Camera
                    for (int i = 0; i < raw_data.Length; i += imline)
                        akvcamproc.StandardInput.BaseStream.Write(raw_data, i, imline);
                };
            };

            try { if(akvcamproc != null) akvcamproc.Kill(); } catch { };
        }

        public static System.Drawing.Image GetPreview()
        {
            System.Drawing.Image result = null;
            preview_mutex.WaitOne();
            result = preview_image;
            preview_mutex.ReleaseMutex();
            return result;
        }

        private static int[] GetNewSizeKeepAspectRatio(int sourceWidth, int sourceHeight, int destWidth, int destHeight, bool bigger = true)
        {
            double nPercent = 0;
            double nPercentW = ((double)destWidth / (double)sourceWidth);
            double nPercentH = ((double)destHeight / (double)sourceHeight);

            if (bigger)
            {
                if (nPercentH > nPercentW) nPercent = nPercentH;
                else nPercent = nPercentW;
            }
            else
            {
                if (nPercentH < nPercentW) nPercent = nPercentH;
                else nPercent = nPercentW;
            };

            return new int[] { (int)(sourceWidth * nPercent), (int)(sourceHeight * nPercent) };
        }
    }
}
