using Client2Server;
using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class ClientProgram
    {
        static IClientSocket ClientC = new ClientSocket();
        static void Main(string[] args)
        {
            Console.Write("PORT:");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine(ClientC.Access("127.0.0.1", 9840, port).Info);
            ClientC.Receive(ar=> { Console.WriteLine(ar); });
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "shutdown")
                    break;
                else if (cmd == "login")
                {
                    ClientC.Send(MessageTranslate.EncapsulationInfo(MessageContent.登录,MessageType.请求,"1501","123"));
                }
                else if (cmd == "login1")
                {
                    ClientC.Send(MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.请求, "0000", "123456"));
                }
                else
                ClientC.Send(cmd);
            }
        }

        
    }
}
