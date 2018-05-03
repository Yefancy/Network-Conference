using Client2Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Terminal.OnNewMessagePrint += (title,info,color) => { Console.WriteLine(title+info); };
            Server server = new Server("127.0.0.1", 9840, "sa", "1213141516", "127.0.0.1", "NCDB");
            server.Init();
            //server.Close();
            Console.Read();
        }
    }
}
