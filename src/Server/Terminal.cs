using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCLib;

namespace Server
{
    public class Terminal
    {
        #region 静态字段
        private static string ServerTitle = "[服务器<>][]";
        private static string ClientTitle = "[客户端<{0}>][{1}]:";
        #endregion

        #region 事件集
        /// <summary>
        /// 新消息到来事件
        /// </summary>
        public static event NewMessagePrintEventHandler OnNewMessagePrint;
        #endregion

        #region 消息打印
        /// <summary>
        /// 设置服务端消息题头
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        public static void SetServerTitle(string IP, int port)
        {
            ServerTitle = "[服务器<" + IP + ":" + port + ">]" + "[{0}]:";
        }

        /// <summary>
        /// 直接打印输出
        /// </summary>
        /// <param name="info"></param>
        public static void OriginPrint(string info)
        {
            OnNewMessagePrint("", info);
        }

        /// <summary>
        /// eg:[服务端127.0.0.1][type]:info
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public static void ServerPrint(InfoType type, string info)
        {
            switch (type)
            {
                case InfoType.信息:
                    OnNewMessagePrint(String.Format(ServerTitle, type), info, Color.Green);
                    break;
                case InfoType.响应:
                    OnNewMessagePrint(String.Format(ServerTitle, type), info, Color.RosyBrown);
                    break;
                case InfoType.异常:
                    OnNewMessagePrint(String.Format(ServerTitle, type), info, Color.Red);
                    break;
                default:
                    OnNewMessagePrint(String.Format(ServerTitle, type), info, Color.Green);
                    break;
            }
        }

        /// <summary>
        /// eg:[客户端127.0.0.1][type]:info
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public static void ClientPrint(string remoteEndPoint, InfoType type, string info)
        {
            OnNewMessagePrint(String.Format(ClientTitle, remoteEndPoint, type), info, Color.Blue);
        }
        #endregion

        #region 终端指令操作

        #endregion

    }

    /// <summary>
    /// 新消息到来（委托）
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">信息</param>
    /// <param name="color">打印颜色</param>
    public delegate void NewMessagePrintEventHandler(string title, string message, Color color = new Color());
}
