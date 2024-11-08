using System;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;


namespace VideoVirtualCamRouter
{
    public static class GifBackground
    {
        private static int frame_delay = -1;
        public static int FrameDelay
        {
            get {  return frame_delay <= 0 ? 0 : frame_delay; }
            set { frame_delay = value <= 0 ? 0 : value; }
        }
        
        private static DateTime begin_at = DateTime.UtcNow;

        public static void ResetNextFrame(bool withRate = true)
        {
            if(withRate) frame_delay = -1;
            begin_at = DateTime.UtcNow;
        }

        public static SixLabors.ImageSharp.Image GetNextFrame(SixLabors.ImageSharp.Image image)
        {
            
            if (frame_delay <= 0)
            {
                GifFrameMetadata meta = image.Frames.RootFrame.Metadata.GetGifMetadata();
                frame_delay = 800 / meta.FrameDelay; // instead if 1000 cause cpu delays
            };

            int frames = image.Frames.Count();
            if (frames == 1) return image;            
            TimeSpan elapsed = DateTime.UtcNow - begin_at;
            int frame = (int)(elapsed.TotalMilliseconds / frame_delay);
            frame = frame % frames;

            return image.Frames.CloneFrame(frame);
        }
    }
}
