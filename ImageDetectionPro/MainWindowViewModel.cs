using AForge.Video.DirectShow;
using Alturos.Yolo;
using ImageDetectionPro.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace ImageDetectionPro
{
    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        readonly FilterInfoCollection filterInfoCollection;
        public MainWindowViewModel()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            cameras = new ObservableCollection<string>();
            videoCaptureDevice = new VideoCaptureDevice();

            capturedImage = new Image();
        }

        public void LoadConfig()
        {
            foreach (FilterInfo filter in filterInfoCollection)
            {
                cameras.Add(filter.Name);
            }

            if (cameras.Count > 0)
            {
                selectedIndex = 1;
                videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[selectedIndex].MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                videoCaptureDevice.Start();
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            using (var newFrame = (System.Drawing.Bitmap)eventArgs.Frame.Clone())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var result = newFrame.BitmapToImageSourceWithDetection();
                    var img = result.Item1;
                    var detectedObject = result.Item2;
                    capturedImage.Source = img;

                });
            }

        }

        public void Dispose()
        {
            if (videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame;
                videoCaptureDevice.Stop();
            }
        }

        private ObservableCollection<string> _cameras;
        public ObservableCollection<string> cameras
        {
            get { return _cameras; }
            set { Set(ref _cameras, value); }
        }

        private VideoCaptureDevice _videoCaptureDevice;
        public VideoCaptureDevice videoCaptureDevice
        {
            get { return _videoCaptureDevice; }
            set { Set(ref _videoCaptureDevice, value); }
        }

        private Image _capturedImage;
        public Image capturedImage
        {
            get { return _capturedImage; }
            set { Set(ref _capturedImage, value); }
        }

        private int _selectedIndex;
        public int selectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }


    }
}
