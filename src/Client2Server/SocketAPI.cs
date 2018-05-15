using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client2Server
{
    public abstract class SocketAPI : ISocketAPI
    {
        /// <summary>
        /// 定义的通信Socket
        /// </summary>
        public Socket communicateSocket = null;

        /// <summary>
        /// Socket套接字入口 交由子类实现
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="port">端口</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>结果</returns>
        public abstract IResult Access(string IP, int port, int listen_or_port, Action<string> callBack = null);

        /// <summary>
        /// 释放socket资源
        /// </summary>
        /// <param name="key"></param>
        public void DisposeSocket(string key)
        {
            try
            {
                if (communicateSocket != null)
                    communicateSocket.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IResult RecognizeInfo(byte info)
        {
            throw new NotImplementedException();
        }

    }
}
