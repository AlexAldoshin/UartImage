using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UartImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PortsRefresh();
            int x = int.Parse(textBoxX.Text);
            int y = int.Parse(textBoxY.Text);
            bmp = new Bitmap(x, y);

        }

        private void PortsRefresh()
        {
            comboBox1.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }

            if (ports.Length > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            var port = comboBox1.Text;

            if (port.Length > 3)
            {
                var BaudRate = int.Parse(textBoxBaudRate.Text);

                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }

                serialPort1.PortName = port;
                serialPort1.BaudRate = BaudRate;
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }
                else
                {
                    serialPort1.Close();
                    serialPort1.Open();
                }
                TestPort();

                ReciveBytes = 0;
                ReciveAllBytes = 0;
                StartTime = DateTime.Now;

            }
            else
            {
                MessageBox.Show("Выберите порт", "Ahtung!");
            }

        }

        private void TestPort()
        {
            if (serialPort1.IsOpen)
            {
                toolStripStatusLabelPort.Text = "Порт " + serialPort1.PortName + " открыт";
            }
            else
            {
                toolStripStatusLabelPort.Text = "Порт " + serialPort1.PortName + " закрыт";
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {


            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            TestPort();
        }

        DateTime StartTime;
        private bool newLine = false;
        private int lineNum = 0;
        private int lastLineNum = 0;

        List<byte> LineData = new List<byte>();
        private int ReciveBytes = 0;
        private int ReciveAllBytes = 0;
        private Bitmap bmp;



        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                int count = serialPort1.BytesToRead;
                byte[] data = new byte[count];
                serialPort1.Read(data, 0, data.Length);

                foreach (var indata in data)
                {
                    ReciveBytes++;

                    if (indata == 0 && ReciveBytes > 4)
                    {
                        ReciveAllBytes += ReciveBytes;
                        UpdateIMG();
                        ReciveBytes = 0;
                        LineData = new List<byte>();
                    }
                    if (ReciveBytes == 1)
                    {
                        lineNum = indata;
                    }
                    if (ReciveBytes == 2)
                    {
                        lineNum = lineNum * 256 + indata;
                    }
                    if (ReciveBytes > 2)
                    {                       
                        var colb = indata % 8;
                        byte valb = (byte)(indata - colb);
                        for (int i = 0; i < colb; i++)
                        {
                            LineData.Add(valb);
                        }
                       
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void UpdateIMG()
        {
            try
            {
                if (lastLineNum > lineNum)
                {

                    pictureBox1.Image = (Image)bmp;

                    var dT = DateTime.Now - StartTime;
                    var dTsec = dT.TotalMilliseconds;
                    var dSpeed = Math.Round(1000 * ReciveAllBytes / dTsec);
                    toolStripStatusLabelError.Text = "Получено " + ReciveAllBytes.ToString() + " байт. Скорость " + dSpeed.ToString() + " байт/сек";
                }
                lastLineNum = lineNum;


                if (lineNum < bmp.Height)
                {
                    int px = 0;
                    foreach (var color_val in LineData)
                    {
                        if (px < bmp.Width)
                        {
                            byte b2 = (byte)(color_val % 16);
                            byte b1 = (byte)(color_val - b2);
                            var col = Color.FromArgb(b1, b1, b1);
                            bmp.SetPixel(px, lineNum, col);
                            px++;
                            if (px < bmp.Width)
                            {
                                col = Color.FromArgb(b2, b2, b2);
                                bmp.SetPixel(px, lineNum, col);
                                px++;
                            }
                        }
                    }
                }
                // pictureBox1.Image = (Image)bmp;
            }
            catch (Exception)
            {
                toolStripStatusLabelError.Text = "Ошибка в функции UpdateIMG";
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            int x = int.Parse(textBoxX.Text);
            int y = int.Parse(textBoxY.Text);
            bmp = new Bitmap(x, y);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            PortsRefresh();
        }
    }
}
