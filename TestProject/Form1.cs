using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace TestProject
{
    public partial class S3IP : Form
    {

        private static byte[] _buffer = new byte[1024];
        private Socket _serverSocket;

        void Connecter()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!_serverSocket.Connected)
            {
                try
                {
                    _serverSocket.Connect(IpTxt.Text, 6000);
                    Connect.Enabled = false;
                    DisconnectBtn.Enabled = true;
                    button1.Enabled = true;
                    Console.WriteLine("Connected to Nintendo Switch " + IpTxt.Text + "\r\n");
                    if (pictureBox1.Image != null) 
                    {
                        button2.Enabled = true;
                    }
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed To Connect");
                    break;
                }
            }
        }

        void SendCmd(string cmd)
        {
            try
            {
                byte[] msg = Encoding.UTF8.GetBytes(cmd + "\r\n");
                byte[] bytes = new byte[256];
                int byteCount = _serverSocket.Send(msg, 0, msg.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {               
                    MessageBox.Show("No Device Connected");                                           
            }
        }

        

        public S3IP()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectImage.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            DialogResult img = SelectImage.ShowDialog();
            
            if (img == DialogResult.OK) 
            {

                string file = SelectImage.FileName;
                pictureBox1.Image = Image.FromFile(file);
                if (Connect.Enabled == false) 
                {
                    button2.Enabled = true;
                }
               
                
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {

            


                Bitmap bitmap = new Bitmap(pictureBox1.Image);
                int x;
                int y = 0;
                for (x = 0; x < bitmap.Size.Width; x++)
                {

                    Color pixelColor = bitmap.GetPixel(x, y);


                    if (pixelColor.ToString() == "Color [A=255, R=0, G=0, B=0]")
                    {
                        //blackpixels++;

                        Console.WriteLine($"{x} , {y}");

                        SendCmd($"touchHold {(x * 3) + 160} {(y * 3) + 190} 33");
                        await Task.Delay(160);
                    }
                    if (x == 319 && y < 117)
                    {
                        y++;
                        x = 0;
                    }
                }               
        }

        private void Connect_Click_1(object sender, EventArgs e)
        {
            Connecter();

        }


       

        private void DisconnectBtn_Click(object sender, EventArgs e)
        {
            _serverSocket.Close();
            DisconnectBtn.Enabled = false;
            Connect.Enabled = true;
            button2.Enabled =false;
        }
    }
}
