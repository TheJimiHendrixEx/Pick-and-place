using System.Windows;
using Microsoft.Expression.Encoder.Devices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;


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
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                MessageBoxResult result = MessageBox.Show("You hit the down key");
            }
        }

        private void GoButton(object sender, RoutedEventArgs e)
        {
            //var checkedButton = 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void AllCheckBoxes_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // Check of the raiser of the event is a checked Checkbox.
            // Of course we also need to to cast it first.
            RadioButton but = (RadioButton)sender;
            bool? x = but.IsChecked;
            bool newBool = x.HasValue ? x.Value : false;
            if (newBool)
            {
                // This is the correct control.
                RadioButton rb = (RadioButton)sender;
                int intIndex = System.Convert.ToInt32(rb.Content.ToString());
                MessageBox.Show(intIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }
    }
}
