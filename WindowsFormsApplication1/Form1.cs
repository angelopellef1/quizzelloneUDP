using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        UDPSocket server = new UDPSocket();


        Boolean ServerOn = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static string GetLocalIPAddress()
        {
      
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList[1].ToString();

        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            ServerOn = true;
            server.Server("192.168.0.119", 27000);
            timer1.Enabled = true;
            button1.Text = "Start";
            label1.Text = GetLocalIPAddress();
        
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if(MSGmes.getset_new)
            {
                MSGmes.getset_new = false;
                if(MSGmes.get_set_valueres == 1)
                {
                    textBox_hit.Text = textBox1.Text;
                    textBox_hit.BackColor = textBox1.BackColor;
                }
                else if(MSGmes.get_set_valueres == 2)
                {
                     textBox_hit.Text = textBox2.Text;
                     textBox_hit.BackColor = textBox2.BackColor;
                }
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            MSGmes.getset_new = false;
            MSGmes.getset_locke = false;
            textBox_hit.Text = "pronti....";
            textBox_hit.BackColor = Color.DarkGray;
            
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            ServerOn = true;
            
            timer1.Enabled = false;
            button1.Text = "START";
            textBox_hit.Text = "";
        }

    }


     class MSGmes
    {
        private static Boolean neew;
        private static Boolean locke;
        private static int valueres;

        public static Boolean getset_new
        {
            get { return neew; }
            set { neew = value; }
        }
          public static Boolean getset_locke
        {
            get { return locke; }
            set { locke = value; }
        }

        public static int get_set_valueres
        {
            get { return valueres; }
            set { valueres = value; }
        }
    }

    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        MSGmes result = new MSGmes();

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
               
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                if(MSGmes.getset_locke == false)
                {
                    MSGmes.getset_locke = true;
                    MSGmes.getset_new = true;
                    MSGmes.get_set_valueres = Convert.ToInt32(so.buffer[0]);
                }
               
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
           
          
        }
    }

    
}
