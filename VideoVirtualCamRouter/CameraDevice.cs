//
// CameraDevice (C#)
// VideoVirtualCamRouter.CameraDevice
// v 0.1, 16.10.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System;
using System.Collections.Generic;
using System.Management;

namespace VideoVirtualCamRouter
{
    public class CameraDevice
    {
        public int OpenCvId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DeviceId { get; set; }
        public string Status { get; set; }


        public static List<CameraDevice> GetAllConnectedCameras()
        {
            List<CameraDevice> cameras = new List<CameraDevice>();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                int openCvIndex = 0;
                foreach (var device in searcher.Get())
                {
                    cameras.Add(new CameraDevice()
                    {
                        Name = device["Caption"].ToString(),
                        Description = device["Description"].ToString(),
                        Status = device["Status"].ToString(),
                        DeviceId = device["DeviceId"].ToString(),
                        OpenCvId = openCvIndex
                    });
                    ++openCvIndex;
                };
            };
            return cameras;
        }

        public override string ToString() => string.IsNullOrEmpty(Description) ? $"{Name} #{OpenCvId}" : $"{Name} ({Description}) #{OpenCvId}";
    }

    public class VirtualCameraDevice
    {
        public string Name;
        public string Description;
        public override string ToString() => string.IsNullOrEmpty(Description) ? Name : $"{Description} ({Name})";

    }
}
