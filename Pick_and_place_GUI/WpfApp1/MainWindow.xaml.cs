using System.Windows;
using Microsoft.Expression.Encoder.Devices;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Collection<EncoderDevice> VideoDevices { get; set; }
        public Collection<EncoderDevice> AudioDevices { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            AudioDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);
        }
        public void StartCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebcamViewer.StartPreview();
            }
            catch (Microsoft.Expression.Encoder.SystemErrorException ex)
            {
                MessageBox.Show("Device is in use by another application");
            }
        }
        private void StopCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            WebcamViewer.StopPreview();
        }
        private void StartRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            // Start recording of webcam video to harddisk.
            WebcamViewer.StartRecording();
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop recording of webcam video to harddisk.
            WebcamViewer.StopRecording();
        }
    }
}
