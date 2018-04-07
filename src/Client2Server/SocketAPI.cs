﻿using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client2Server
{
    public class SocketAPI : ISocketAPI
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
        public virtual Result Access(string IP, int port, int listen_or_port, Action<string> callBack = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 释放socket资源
        /// </summary>
        /// <param name="key"></param>
        public void DisposeSocket(string key)
        {
            communicateSocket.Disconnect(false);
            communicateSocket.Close();
        }

        public Result RecognizeInfo(byte info)
        {
            throw new NotImplementedException();
        }

    }
}
