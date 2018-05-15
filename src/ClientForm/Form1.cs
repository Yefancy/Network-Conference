using NCLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        Client.Client client;
        public Form1(Client.Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rd = new Random();
            int port = rd.Next(1000, 9999);
            label1.Text = "port:" + port;            
            client.Init("127.0.0.1", 9840, port);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.BeginLogin(ar =>
            {
                var a = (NCAsyncResult)ar;
                if (a.BaseResult == baseResult.Successful)
                    client.ConnectOMCS("127.0.0.1", 9900);
                showResult(a);
                Invoke(new Action(()=> { label3.Text = "id:0000"; }));                               
            }
            , "0000", "123456", null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.BeginLogin(ar =>
            {
                var a = (NCAsyncResult)ar;
                if (a.BaseResult == baseResult.Successful)
                    client.ConnectOMCS("127.0.0.1", 9900);
                showResult(a);
                Invoke(new Action(() => { label3.Text = "id:0000"; }));
            }
            , "1501", "123", null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client.BeginLogin(ar =>
            {
                var a = (NCAsyncResult)ar;
                if (a.BaseResult == baseResult.Successful)
                    client.ConnectOMCS("127.0.0.1", 9900);
                showResult(a);
                Invoke(new Action(() => { label3.Text = "id:0000"; }));
            }
            , "1502", "123", null);
        }

        void showResult(IResult result)
        {
            this.BeginInvoke(new Action<IResult>(a=> { label2.Text = a.BaseResult.ToString() + a.Info; }),result);          
        }

        private void button5_Click(object sender, EventArgs e)
        {
            client.BeginCreateNCRoom(ar => 
            {
                var a = (NCAsyncResult)ar; showResult(a);
                if (a.BaseResult == baseResult.Successful)
                    Invoke(new Action(() => { label4.Text = "roomid:" + textBox1.Text; }));                
            }, textBox1.Text, textBox2.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            client.BeginJoinNCRoom(ar => 
            {
                var a = (NCAsyncResult)ar; showResult(a);
                if (a.BaseResult == baseResult.Successful)
                    Invoke(new Action(() => { label4.Text = "roomid:" + textBox1.Text; }));
            }, textBox1.Text, textBox2.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            client.ExitNCRoom(textBox1.Text);
            label4.Text = "roomid:";
        }
    }
}
