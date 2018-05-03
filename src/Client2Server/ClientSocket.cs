using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCLib;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Client2Server
{
    public class ClientSocket : SocketAPI, IClientSocket
    {
        /// <summary>
        /// 重写客户端Access函数
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        /// <param name="listen"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public override IResult Access(string IP, int port, int blindport, Action<string> callBack = null)
        {
            base.communicateSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            base.communicateSocket.Bind(new IPEndPoint(IPAddress.Any, blindport));

            //服务器的IP和端口
            IPAddress tempIP;
            if (!IPAddress.TryParse(IP, out tempIP))
            {
                return new Result(baseResult.Faild, "监听失败，失败原因:错误IP地址");
            }
            IPEndPoint serverIP = new IPEndPoint(tempIP, port);

            //连接请求
            try
            {
                base.communicateSocket.BeginConnect(serverIP, ar =>
                {
                    if(callBack!=null)
                        callBack(serverIP.ToString());
                }, null);
            }
            catch
            {
                throw new Exception(string.Format("尝试连接{0}不成功!", IP));
            }
            return new Result(baseResult.Successful, "客户端请求连接Socket(IP:" + IP + "PORT:" + port + ")中..\n");
        }

        /// <summary>
        /// 重写异步发送请求(Client)
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>发送结果</returns>
        public void Send(string info, Action<string> callBack = null)
        {
            if (base.communicateSocket.Connected == false)
            {
                throw new Exception("还没有建立连接, 不能发送消息");
            }
            Byte[] msg = Encoding.UTF8.GetBytes(info);
            base.communicateSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                ar =>
                {
                    //结束挂起的异步线程
                    base.communicateSocket.EndSend(ar);
                    if (callBack != null)
                        callBack(info);
                }, null);
            //return new Result(baseResult.Successful, "成功发送请求");
        }

        /// <summary>
        /// 重写异步接受响应(Client)
        /// </summary>
        /// <param name="callBack">回调函数</param>
        /// <returns>处理结果</returns>
        public void Receive(Action<string> callBack = null)
        {
            Byte[] msg = new byte[1024];
            //异步的接受消息
            base.communicateSocket.BeginReceive(msg, 0, msg.Length, SocketFlags.None,
                ar =>
                {
                    //连接断开时抛出Socket Exception 
                    try
                    {
                        //结束挂起的异步线程                     
                        base.communicateSocket.EndReceive(ar);
                        //还原字符串
                        if(callBack!=null)
                        callBack(Encoding.UTF8.GetString(msg).Trim('\0', ' '));
                        Receive(callBack);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        base.communicateSocket.Close();
                    }
                }, null);
        }
    }
}
