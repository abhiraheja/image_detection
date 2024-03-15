using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageDetectionPro.Helpers
{
    public static class ConvertBitmapToBitmapImage
    {
        static readonly YoloWrapper yolo;
        static ConvertBitmapToBitmapImage()
        {
            var configurationDetector = new YoloConfigurationDetector();
            var config = configurationDetector.Detect("D:\\image_detection\\ImageDetectionPro\\networks\\yolov3-tiny\\");
            yolo = new YoloWrapper(config);
        }
        public static BitmapImage BitmapToImageSourceWithDetection(this System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {

                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                var _items = yolo.Detect(memory.ToArray()).ToList();
                if(_items.Where(x=>x.Type == "cell phone").Any())
                {
                    string filename = "CAPTURE_PHONE-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                    bitmapimage.SaveImage($"D:\\CAPTURE\\{filename}");
                }
                return bitmapimage;
            }
        }

        public static BitmapImage BitmapToImageSource(this System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {

                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public static void SaveImage(this BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();            
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        public static void TakeScreenShot()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            using (Bitmap bmp = new Bitmap((int)screenWidth,
           (int)screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    string filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                    //  Opacity = .0;
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    string filePath = "D:\\Screenshots\\" + filename;
                    var bitmapImage = bmp.BitmapToImageSource();
                    bitmapImage.SaveImage(filePath);
                    //  Opacity = 1;
                }
            }
        }
    }
}
