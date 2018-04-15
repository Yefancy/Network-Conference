using Client2Server;
using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class ServerProgram
    {
        static List<string> clients = new List<string>();
        static IServerSocket ServerC = new ServerSocket();

        static void Main(string[] args)
        {

            Console.WriteLine(ServerC.Access("127.0.0.1", 9840, 10, Accept).Info);
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "shutdown")
                    break;
                ServerC.Send(clients[0], "服务器发送信息:" + cmd);
            }
        }

        public static void Accept(string info)
        {
            Console.WriteLine("接收到来自:" + info + "连接请求");
            clients.Add(info);
            ServerC.Receive(info, Receive);
        }

        public static void Receive(string IP, string info)
        {
            Console.WriteLine("来自"+IP+"请求:"+info);
            ServerC.Send(IP, "服务器响应了你:" + info + "的请求");
        }
    }
}
