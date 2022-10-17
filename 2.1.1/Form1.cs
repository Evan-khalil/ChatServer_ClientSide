using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2._1._1
{
    public partial class Form1 : Form
    {
        private readonly TcpClient TcpClient = new TcpClient();
        private readonly byte[] Bytes = new byte[1024];
        private string Data;
        private string DefaultAddress = "127.0.0.1";
        private int Port = 2000;
        private int Duration = 5;
        public Timer Timer = new Timer(); public Form1()
        {
            InitializeComponent();
        }
        public async Task Connect()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(DefaultAddress), Port);
            await TcpClient.ConnectAsync(remoteIpEndPoint.Address, remoteIpEndPoint.Port);
            await read();
        }
        public async Task read()
        {
            NetworkStream stream = TcpClient.GetStream();
            while (TcpClient.Connected)
            {
                try
                {
                    int i = await stream.ReadAsync(Bytes, 0, Bytes.Length);
                    Data = Encoding.ASCII.GetString(Bytes, 0, i);
                    ConversationBox.AppendText(Data + " \r\n");
                }
                catch (Exception)
                {
                }
            }
        }
        public async Task CheckConnectionStatus()
        {
            if (TcpClient.Connected)
            {
                ConversationBox.AppendText("kopplingen lyckades... \r\nDu är uppkopplad på :\r\nAddress : " + DefaultAddress + "\r\nPort : " + Port + "\r\n");
            }
            else
            {
                ConversationBox.AppendText("Kopplingen misslyckades \r\nAvslutar programmet om...");
                Timer.Tick += new EventHandler(Count_down);
                Timer.Interval = 1000;
                Timer.Start();
            }
        }
        private void Count_down(object sender, EventArgs e)
        {

            if (Duration == 0)
            {
                Timer.Stop();
                Environment.Exit(0);
            }
            else if (Duration > 0)
            {
                Duration--;
                ConversationBox.AppendText("\r\n" + Duration.ToString());
            }
        }
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (IpTextBox.Text != "" && PortTextBox.Text != "")
                {
                    DefaultAddress = IpTextBox.Text;
                    Port = int.Parse(PortTextBox.Text);
                }
                else if (IpTextBox.Text == "" && PortTextBox.Text != "")
                {
                    Port = int.Parse(PortTextBox.Text);
                }
                else if (IpTextBox.Text != "" && PortTextBox.Text == "")
                {
                    DefaultAddress = IpTextBox.Text;
                }
                else
                {
                }
                Connect();
                CheckConnectionStatus();
            }
            catch (Exception)
            {

            }
        }
        private void DisconnectBtn_Click(object sender, EventArgs e)
        {
            TcpClient.Close();
        }
        private void SendBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string message = MessageTextBox.Text;
                byte[] data = Encoding.ASCII.GetBytes(message);
                NetworkStream stream = TcpClient.GetStream();
                stream.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
            }
        }
    }
}
