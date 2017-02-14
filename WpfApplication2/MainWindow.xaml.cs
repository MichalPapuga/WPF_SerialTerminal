using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Timers;


namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool isPortOpen;

        static SerialPort _serialPort;
        public string SerialPort_Name;
        public Parity SerialPort_Parity = 0;
        public StopBits SerialPort_StopBits;
        public int SerialPort_DataBits = 8;
        private Handshake SerialPort_Handshake;
        public int BaudRate;

        public string ImageURL;
        private Rectangle[] _rectangleArray;

        public Thread Reading;
        public static System.Timers.Timer aTimer;

        public static string message;

        public static int Statistic_Rx_Counter = 0;
        public static int Statistic_Tx_Counter = 0;


        Action action;

        public MainWindow()
        {
            

            InitializeComponent();

            action = Run;

            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox.Items.Add(s);

            }

            textBox_SerialPort_Stats_Rx.Text = Statistic_Rx_Counter.ToString();
            textBox_SerialPort_Stats_Tx.Text = Statistic_Tx_Counter.ToString();

            if (comboBox.SelectedIndex == -1)
            {
                comboBox.SelectedIndex = 0;
            }

            Brush LightGray = Brushes.LightGray; // default color

            Brush Gray = Brushes.Gray; // Pin coneccted to Ground

            Brush LimeGreen = Brushes.LimeGreen; // Pin coneccted to Ground


            _rectangleArray = new[] { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10,
                                       a11, a12, a13, a14, a15, a16, a17, a18, a19, a20,
                                       a21, a22, a23, a24, a25, a26, a27, a28, a29, a30,
                                       a31, a32,
                                       b1, b2, b3, b4, b5, b6, b7, b8, b9, b10,
                                       b11, b12, b13, b14, b15, b16, b17, b18, b19, b20,
                                       b21, b22, b23, b24, b25, b26, b27, b28, b29, b30,
                                       b31, b32,
                                       c1, c2, c3, c4, c5, c6, c7, c8, c9, c10,
                                       c11, c12, c13, c14, c15, c16, c17, c18, c19, c20,
                                       c21, c22, c23, c24, c25, c26, c27, c28, c29, c30,
                                       c31, c32,};
            
            foreach(Rectangle l in _rectangleArray)
            {
                l.Fill = LimeGreen;

            }
            

        

        }

        private void Run()
        {
         //   Console.WriteLine(Thread.CurrentThread.IsBackground);
         //   Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);

            RichTextBox_Recive.AppendText(message);
           // Statistic_Rx_Counter += _serialPort.BytesToRead;

            textBox_SerialPort_Stats_Rx.Text = Statistic_Rx_Counter.ToString();



        }


        private void Read()
        {

            while (isPortOpen)
            {
                try
                {
                   
                     message = _serialPort.ReadLine();

                    Console.WriteLine("COM port says: "+message+"\r\n");

                    if  (System.Threading.Thread.CurrentThread != RichTextBox_Recive.Dispatcher.Thread)
                        {
                            RichTextBox_Recive.Dispatcher.Invoke(action);
                            
                        }
                    else
                        action();
                
                }
                catch (Exception ex)
                {
                    if (ex is System.IO.IOException)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    if (ex is TimeoutException)
                    {

                    }
                }

            }

        }


        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(200);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _serialPort.WriteLine("Write some text to Port! \r\n");
        }

        private void ComInitializer(string portname)
        {
         
            _serialPort = new SerialPort();
            try { 
            _serialPort.PortName = SerialPort_Name;
            }
            catch(ArgumentNullException e)
            {
                MessageBox.Show("Brak COM portow! " + e.Message, "For debug purpose only!");
            }
                _serialPort.BaudRate = BaudRate;
            _serialPort.Parity = SerialPort_Parity;
            _serialPort.DataBits = SerialPort_DataBits;
            _serialPort.StopBits = SerialPort_StopBits;
            _serialPort.Handshake = SerialPort_Handshake;

            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            if (_serialPort.IsOpen)
            {
                MessageBox.Show(_serialPort.PortName, " is opened!");
                
            }
            _serialPort.Close();
          

        }

        private void PortOpen(SerialPort serialPort)
        {

            try
            {
                serialPort.Open();
                
                MessageBox.Show("COM port open!","Debug purpose only");

                button_SerialPort_Open.Content = "Close Port";
            //    button_SerialPort_Rescan.IsEnabled = false;
                DisableAllButtons();
                Reading = new Thread(Read);
                Reading.Start();
                SetTimer();

            }
            catch(IOException e)
            {
                MessageBox.Show("COM port open error! \r" + e.Message);
                serialPort.Close();
                button_SerialPort_Rescan.IsEnabled = true;
                button_SerialPort_Open.Content = "Open Port";
                isPortOpen = false;
            }


        }

        private void DisableAllButtons()
        {
            button_SerialPort_Rescan.IsEnabled = false;
            radioButton_Baudrate_115200.IsEnabled = false;
            radioButton_Baudrate_9600.IsEnabled = false;
            radioButton_CustomBaudRate.IsEnabled = false;

            radioButton_DataBits_5.IsEnabled = false;
            radioButton_DataBits_6.IsEnabled = false;
            radioButton_DataBits_7.IsEnabled = false;
            radioButton_DataBits_8.IsEnabled = false;

            radioButton_Handshaking_none.IsEnabled = false;
            radioButton_Handshaking_RTSCTS.IsEnabled = false;
            radioButton_Handshaking_RTSCTS_XONXOFF.IsEnabled = false;
            radioButton_Handshaking_xONXOFF.IsEnabled = false;

            radioButton_Parity_even.IsEnabled = false;
            radioButton_Parity_mark.IsEnabled = false;
            radioButton_Parity_none.IsEnabled = false;
            radioButton_Parity_odd.IsEnabled = false;

            radioButton_StopBits_1.IsEnabled = false;
            radioButton_StopBits_2.IsEnabled = false;
            radioButton_StopBits_3.IsEnabled = false;
            
        }

        private void EnableAllButtons()
        {
            button_SerialPort_Rescan.IsEnabled = true;
            radioButton_Baudrate_115200.IsEnabled = true;
            radioButton_Baudrate_9600.IsEnabled = true;
            radioButton_CustomBaudRate.IsEnabled = true;

            radioButton_DataBits_5.IsEnabled = true;
            radioButton_DataBits_6.IsEnabled = true;
            radioButton_DataBits_7.IsEnabled = true;
            radioButton_DataBits_8.IsEnabled = true;

            radioButton_Handshaking_none.IsEnabled = true;
            radioButton_Handshaking_RTSCTS.IsEnabled = true;
            radioButton_Handshaking_RTSCTS_XONXOFF.IsEnabled = true;
            radioButton_Handshaking_xONXOFF.IsEnabled = true;

            radioButton_Parity_even.IsEnabled = true;
            radioButton_Parity_mark.IsEnabled = true;
            radioButton_Parity_none.IsEnabled = true;
            radioButton_Parity_odd.IsEnabled = true;

            radioButton_StopBits_1.IsEnabled = true;
            radioButton_StopBits_2.IsEnabled = true;
            radioButton_StopBits_3.IsEnabled = true;


        }

        private void PortClose(SerialPort serialPort)
        {
            try
            {
                isPortOpen = false;

                Reading.Join();
                aTimer.Stop();
                aTimer.Dispose();
             

                serialPort.Close();

                button_SerialPort_Open.Content = "Open Port";

                //button_SerialPort_Rescan.IsEnabled = true;

                EnableAllButtons();


            }
            catch (NullReferenceException d)
            {
                Console.WriteLine("COM port close error! \r"+d.Message);
            }

        }

        private void button_SerialPort_Open_Click(object sender, RoutedEventArgs e)
        {
            if (!isPortOpen)
            {

                isPortOpen = true;

                ComInitializer(SerialPort_Name);
                //      buttonComPort.IsEnabled = false;
                PortOpen(_serialPort);
                
                

            }
            else
            {
                
                PortClose(_serialPort);
                
                
            }
        }

        private void button_SerialPort_Rescan_Click(object sender, RoutedEventArgs e)
        {

            comboBox.Items.Clear();

            foreach (String s in SerialPort.GetPortNames())
            {
                comboBox.Items.Add(s);

            }

            if (comboBox.SelectedIndex == -1)
            {
                comboBox.SelectedIndex = 0;
            }
        }


        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var item = ((sender as ComboBox).SelectedItem as String);

            SerialPort_Name = item;
         
           
        }


        private void radioButton_Baudrate_115200_Checked(object sender, RoutedEventArgs e)
        {
            BaudRate = 115200;
            try
            {
                textBox_Baudrate_Custom.IsEnabled = false;
            }
            catch (NullReferenceException d)
            {
                Console.WriteLine(d.Message);
            }
        }

        private void radioButton_Baudrate_9600_Checked(object sender, RoutedEventArgs e)
        {
            BaudRate = 9600;
            textBox_Baudrate_Custom.IsEnabled = false;
        }

        
        private void radioButton_DataBits_5_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_DataBits = 5;
        }
        private void radioButton_DataBits_6_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_DataBits = 6;
        }

        private void radioButton_DataBits_7_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_DataBits = 7;
        }
        private void radioButton_DataBits_8_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_DataBits = 8;
        }

        // Port PARITY button CHECKED

            // NONE
        private void radioButton_Parity_none_Checked(object sender, RoutedEventArgs e)
        {
            string parity = "none";

            SerialPort_Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);

        }
            // ODD
        private void radioButton_Parity_odd_Checked(object sender, RoutedEventArgs e)
        {
            string parity = "odd";

            SerialPort_Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
        }
            
            //EVEN
        private void radioButton_Parity_even_Checked(object sender, RoutedEventArgs e)
        {
            string parity = "even";

            SerialPort_Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
        }
            //MARK
        private void radioButton_Parity_mark_Checked(object sender, RoutedEventArgs e)
        {
            string parity = "mark";

            SerialPort_Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
        }


        // Port HANDSHAKE button CHECKED
        
            //NONE 
        private void radioButton_Handshaking_none_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true);
        }

        private void radioButton_Handshaking_RTSCTS_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_Handshake = (Handshake)Enum.Parse(typeof(Handshake), "RequestToSend", true);
        }

        private void radioButton_Handshaking_xONXOFF_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_Handshake = (Handshake)Enum.Parse(typeof(Handshake), "XOnXOff", true);
        }

        private void radioButton_Handshaking_RTSCTS_XONXOFF_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_Handshake = (Handshake)Enum.Parse(typeof(Handshake), "RequestToSendXOnXOff", true);
        }



        private void radioButton_CustomBaudRate_Checked(object sender, RoutedEventArgs e)
        {
            textBox_Baudrate_Custom.IsEnabled = true;
        }

        private void radioButton_StopBits_1_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_StopBits = (StopBits)Enum.Parse(typeof(StopBits), "one", true);
        }

        private void radioButton_StopBits_2_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_StopBits = (StopBits)Enum.Parse(typeof(StopBits), "OnePointFive", true);
        }

        private void radioButton_StopBits_3_Checked(object sender, RoutedEventArgs e)
        {
            SerialPort_StopBits = (StopBits)Enum.Parse(typeof(StopBits), "Two", true);
        }

        private void button_RichText_Clear_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox_Recive.Document.Blocks.Clear();
        }
    }

}
