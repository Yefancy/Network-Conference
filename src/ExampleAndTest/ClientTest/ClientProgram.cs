using Client2Server;
using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;

namespace ClientTest
{
    class ClientProgram
    {
        static IClientSocket ClientC = new ClientSocket();
        static void Main(string[] args)
        {
            test2();
        }

        static void test1()
        {
            Console.Write("PORT:");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine(ClientC.Access("127.0.0.1", 9840, port).Info);
            ClientC.Receive(ar => { Console.WriteLine(ar); });
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "shutdown")
                    break;
                else if (cmd == "login")
                {
                    ClientC.Send(MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.请求, "1501", "123"));
                }
                else if (cmd == "login1")
                {
                    ClientC.Send(MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.请求, "0000", "123456"));
                }
                else
                    ClientC.Send(cmd);
            }
        }

        static void test2()
        {
            Console.Write("PORT:");
            int port = int.Parse(Console.ReadLine());
            Client.Client client = new Client.Client("127.0.0.1", 9840, port);
            client.Init();
            ClientUser c = client.UserInfo;
            NCAsyncResult aa = (NCAsyncResult)client.BeginLogin("1501","123", ar=> 
            { }
            ,null);
            while(!aa.IsCompleted)
            {
                Console.WriteLine("等待中。。。");
            }
            Console.WriteLine(aa.BaseResult.ToString() + aa.Info);

            client.BeginLogin("0000", "123456", ar =>
            {
                var a = (NCAsyncResult)ar;
                Console.WriteLine(a.BaseResult.ToString() + a.Info);
            }
            , null);
            Console.Read();
        }
    }
}
