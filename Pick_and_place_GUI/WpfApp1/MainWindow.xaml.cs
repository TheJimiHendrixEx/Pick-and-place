using System.Windows;
using Microsoft.Expression.Encoder.Devices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO.Ports;
using System;
using System.Threading;
using System.ComponentModel;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

   
    public partial class MainWindow : Window
    {
        public Collection<EncoderDevice> VideoDevices { get; set; }
        public Collection<EncoderDevice> AudioDevices { get; set; }
        static public SerialPort com;
        static public Boolean _continue = false;
        Thread Readthread = new Thread(Read);

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static double _xpos = 0;
        public static double xpos
        {
            get { return _xpos; }
            set
            {
                if(_xpos != value)
                {
                    _xpos = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("xpos"));
                }
            }
        }

        private static double _ypos = 0;
        public static double ypos
        {
            get { return _ypos; }
            set
            {
                if(_ypos != value)
                {
                    _ypos = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("ypos"));
                }
            }
        }

        private static double _zpos = 0;
        public static double zpos
        {
            get { return _zpos; }
            set
            {
                if(_zpos != value)
                {
                    _zpos = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("zpos"));
                }
            }
        }

        float apos = 0;
        float xOffsetVal = 0;
        float yOffsetVal = 0;
        int feedrate = 4000;
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
            else if (e.Key == Key.Right)
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

            Serial_Com(command);
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
            try
            {
                com.BaudRate = Convert.ToInt32(Rates.Text);
                com.PortName = Ports.Text;
                com.Open();
                MessageBox.Show("Connection Successful");
                _continue = true;
                Readthread.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection failed:: " + ex.Message, "Error bitch!!!!");
            }
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = com.ReadLine();
                    if(message.Contains("max limit reached"))
                    {
                        MessageBox.Show(message);
                        switch (message.Substring(0))
                        {
                            case "x":
                                xpos = int.Parse(message.Substring(20, message.Length));
                                break;

                            case "y":
                                ypos = int.Parse(message.Substring(20, message.Length));
                                break;
                        }


                    }
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
            Serial_Com("G1 X0.0 Y0.0 F2500");
        }

        private void Xp_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            xpos += intIndex;
            command = "G1 X" + xpos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Xm_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            xpos -= intIndex;
            command = "G1 X" + xpos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Yp_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            ypos += intIndex;
            command = "G1 Y" + ypos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Ym_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            ypos -= intIndex;
            command = "G1 Y" + ypos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Zp_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            zpos += intIndex;
            command = "G1 Z" + zpos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Zm_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            zpos -= intIndex;
            command = "G1 Z" + zpos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Ap_Click(object sender, RoutedEventArgs e)
        {
            command = "";
            apos += intIndex;
            command = "G1 A" + apos + " F" + feedrate;
            Serial_Com(command);
        }

        private void Am_Click(object sender, RoutedEventArgs e)
        {
            apos -= intIndex;
            command = "G1 A" + apos + " F" + feedrate;
            Serial_Com(command);
        }

        private void ZeroX_Click(object sender, RoutedEventArgs e)
        {
            xpos = 0;
            command = "G93 X0";
        }

        private void ZeroY_Click(object sender, RoutedEventArgs e)
        {
            ypos = 0;
            command = "G94 Y0";
        }

        private void ZeroZ_Click(object sender, RoutedEventArgs e)
        {
            zpos = 0;
            command = "G92 Z0";
            Serial_Com(command);
        }

        private void ZeroA_Click(object sender, RoutedEventArgs e)
        {
            apos = 0;
        }

        private void GoToOffset_Click(object sender, RoutedEventArgs e)
        {
            //Local Variables for Offset Coords

            xpos += xOffsetVal;
            ypos += yOffsetVal;
            command = "G1 X" + xpos + " Y" + ypos + " F" + feedrate;
            Serial_Com(command);
        }

        private void ReturnOffset_Click(object sender, RoutedEventArgs e)
        {
            // Reload Original Coords
            xpos -= xOffsetVal;
            ypos -= yOffsetVal;
            command = "G1 X" + xpos + " Y" + ypos + " F" + feedrate;
            Serial_Com(command);
        }

        // Automatically update offset values when entered into textbox
        private void XOffset_TextChanged(object sender, TextChangedEventArgs e)
        {
            xOffsetVal = float.Parse(xOffset.Text);
        }

        private void YOffset_TextChanged(object sender, TextChangedEventArgs e)
        {
            yOffsetVal = float.Parse(yOffset.Text);
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name 
            dlg.DefaultExt = ".txt"; // Default file extension 
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
            }
        }

        private void Serial_Com(String mes)
        {
            try
            {
                com.WriteLine(mes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

            }
        }

    }
}
