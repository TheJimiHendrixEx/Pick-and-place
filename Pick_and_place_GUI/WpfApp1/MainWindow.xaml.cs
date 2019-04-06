using System.Windows;
using Microsoft.Expression.Encoder.Devices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO.Ports;
using System;
using System.Threading;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Collection<EncoderDevice> VideoDevices { get; set; }
        public Collection<EncoderDevice> AudioDevices { get; set; }
        static public SerialPort com;
        static public Boolean _continue = false;
        Thread Readthread = new Thread(Read);

        float xpos = 0;
        float ypos = 0;
        float zpos = 0;
        float apos = 0;
        float angle = 0;
        int feedrate = 2500;
        int intIndex = 0;
        string command = "";

        public MainWindow()
        {
            InitializeComponent();
            com = new SerialPort();
            PopulateComboBox();
            this.DataContext = this;

            VideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            AudioDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);
        }

        public void PopulateComboBox()
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                Ports.Items.Add(s);
            }
            int[] rates = { 115200, 9600 };

            foreach(int i in rates )
            {
                Rates.Items.Add(i);
            }
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
                string command = "";
                if (e.Key == Key.Down)
                {
                    ypos -= intIndex;
                    command = "G1 Y" + ypos + " F" + feedrate;
                }
                else if (e.Key == Key.Up)
                {
                    ypos += intIndex;
                    command = "G1 Y" + ypos + " F" + feedrate;
                }
                else if (e.Key == Key.Left)
                {
                    xpos -= intIndex;
                    command = "G1 X" + xpos + " F" + feedrate;
                }
                else if(e.Key == Key.Right)
                {
                    xpos += intIndex;
                    command = "G1 X" + xpos + " F" + feedrate;
                }
                else if (e.Key == Key.U)
                {
                    zpos += intIndex;
                    command = "G1 Z" + ypos + " F" + feedrate;
                }
                else if (e.Key == Key.D)
                {
                    zpos -= intIndex;
                    command = "G1 Z" + ypos + " F" + feedrate;
                }
                
                com.WriteLine(command);
            }
           
        

        private void GoButton(object sender, RoutedEventArgs e)
        {
            //var checkedButton = 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton but = (RadioButton)sender;
            bool? x = but.IsChecked;
            bool newBool = x.HasValue ? x.Value : false;
            if (newBool)
            {
                // This is the correct control.
                RadioButton rb = (RadioButton)sender;
                intIndex = System.Convert.ToInt32(rb.Content.ToString());

            }
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
                intIndex = System.Convert.ToInt32(rb.Content.ToString());

            }

        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            com.BaudRate = Convert.ToInt32(Rates.Text);
            com.PortName = Ports.Text;
            com.Open();
            _continue = true;
            Readthread.Start();
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = com.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }

        private void Exit_but_Click(object sender, RoutedEventArgs e)
        {
            _continue = false;
            Readthread.Abort();
            com.Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void Ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            com.WriteLine("G1 X0.0 Y0.0 F2500");
        }

        private void Xp_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            xpos += intIndex;
            command = "G1 X" + xpos + " F" + feedrate;
            com.WriteLine(command);

        }

        private void Xm_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            xpos -= intIndex;
            command = "G1 X" + xpos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void Yp_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            ypos += intIndex;
            command = "G1 Y" + ypos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void Ym_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            ypos -= intIndex;
            command = "G1 Y" + ypos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void Zp_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            zpos += intIndex;
            command = "G1 Z" + zpos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void Zm_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            zpos -= intIndex;
            command = "G1 Z" + zpos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void Ap_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            apos += intIndex;
            command = "G1 A" + apos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void Am_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            apos -= intIndex;
            command = "G1 A" + apos + " F" + feedrate;
            com.WriteLine(command);
        }

        private void ZeroX_Click(object sender, RoutedEventArgs e)
        {
            xpos = 0;
        }

        private void ZeroY_Click(object sender, RoutedEventArgs e)
        {
            ypos = 0;
        }

        private void ZeroZ_Click(object sender, RoutedEventArgs e)
        {
            zpos = 0;
        }
    }
}
