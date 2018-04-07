﻿using NCLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client2Server
{
    public class ServerSocket : SocketAPI, IServerSocket
    {
        /// <summary>
        /// 连接Socket字典
        /// TKey:网络结点号
        /// TValue:套接字
        /// </summary>
        public Dictionary<string, Socket> clientConnections = new Dictionary<string, Socket>();

        /// <summary>
        /// 重写服务端Access函数
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        /// <param name="listen"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public override Result Access(string IP, int port, int listen, Action<string> callBack = null)
        {
            base.communicateSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //接入使用的IP和端口
            IPAddress tempIP;
            if (!IPAddress.TryParse(IP, out tempIP))
            {
                return new Result(baseResult.Faild, "监听失败，失败原因:错误IP地址");
            }
            IPEndPoint serverIP = new IPEndPoint(tempIP, port);
            //绑定服务端设置的IP
            base.communicateSocket.Bind(serverIP);
            //设置监听个数
            base.communicateSocket.Listen(listen);
            //开始异步监听连接请求
            StartAccept(callBack);
            return new Result(baseResult.Successful, "服务端接入Socket(IP:" + IP + "PORT:" + port + ")\n异步接收连接请求中...");
        }

        /// <summary>
        /// 开始异步监听连接请求
        /// </summary>
        /// <param name="callBack">毁掉函数</param>
        private void StartAccept(Action<string> callBack = null)
        {
            base.communicateSocket.BeginAccept(ar =>
            {
                Socket connection = base.communicateSocket.EndAccept(ar);
                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                clientConnections.Add(remoteEndPoint, connection);
                if (callBack != null)
                    callBack(remoteEndPoint);
                StartAccept(callBack);
            }, null);
        }

        /// <summary>
        /// 异步发送响应(Server)
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>发送结果</returns>
        public void SendRespond(string remoteEndPoint, string info, Action<string> callBack = null)
        {
            //是否存在连接
            if (!clientConnections.ContainsKey(remoteEndPoint))
            {
                throw new Exception("未存在该连接");
            }
            Socket cSocket = clientConnections[remoteEndPoint];
            if (cSocket.Connected == false)
            {
                throw new Exception("还没有建立连接, 不能发送消息");
            }
            Byte[] msg = Encoding.UTF8.GetBytes(info);
            cSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                ar =>
                {
                    //结束挂起的异步线程
                    cSocket.EndSend(ar);
                    if (callBack != null)
                        callBack(info);
                }, null);
            //return new Result(baseResult.Successful, "成功发送响应");
        }

        /// <summary>
        /// 异步接受请求(Server)
        /// </summary>
        /// <param name="callBack">回调函数</param>
        /// <returns>处理结果</returns>
        public void ReceiveRequest(string remoteEndPoint, ReceiveCallBack callBack = null)
        {
            Byte[] msg = new byte[1024];
            //异步的接受消息
            Socket cSocket = clientConnections[remoteEndPoint];
            cSocket.BeginReceive(msg, 0, msg.Length, SocketFlags.None,
                ar =>
                {
                    //连接断开时抛出Socket Exception 
                    try
                    {
                        //结束挂起的异步线程             
                        cSocket.EndReceive(ar);
                        //还原字符串
                        if (callBack != null)
                            callBack(cSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(msg).Trim('\0', ' '));
                        ReceiveRequest(remoteEndPoint, callBack);
                    }
                    catch (Exception e)
                    {
                        //cSocket.EndReceive(ar);
                        Console.WriteLine(e.Message);
                        //对于连接断开的异常 移除维持的Socket
                        clientConnections.Remove(remoteEndPoint.ToString());
                        cSocket.Close();
                        Console.WriteLine("释放与" + remoteEndPoint + "的连接资源");
                    }
                }, null);
        }

    }
}