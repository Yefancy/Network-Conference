using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerTerminal
{
    public partial class Form1 : Form
    {
        Server.Server myServer;
        public Form1()
        {
            Terminal.OnNewMessageCome += print;
            myServer = new Server.Server("127.0.0.1", 9840, "sa", "1213141516", "127.0.0.1", "NCDB");
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Terminal.ServerPrint(InfoType.信息, InputBox.Text);
            InputBox.Clear();
        }

        /// <summary>
        /// 异步挂载句柄 打印信息
        /// </summary>
        /// <param name="ins"></param>
        private void print(string title, string ins, Color color = new Color())
        {
            OutputBox.Invoke(new NewMessageComeEventHandler
                ( ( _title,  _ins,  _color) =>
                    {
                        if (_color == Color.Empty)
                            OutputBox.SelectionColor = Color.Black;
                        else
                            OutputBox.SelectionColor = _color;
                        OutputBox.SelectionFont = new Font(Font, FontStyle.Bold);
                        OutputBox.AppendText(title);
                        OutputBox.SelectionFont = new Font(Font, FontStyle.Regular);
                        OutputBox.AppendText(ins + "\n");
                        OutputBox.ScrollToCaret();
                    }), title, ins, color);
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void InputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                e.Handled = true;
                Terminal.ServerPrint(InfoType.信息, InputBox.Text);
                InputBox.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(() => { myServer.Init(); });
            t1.IsBackground = true;
            t1.Start();
        }

    }
}
