using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static event NewMessageComeEventHandler OnNewMessageCome;
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
            OnNewMessageCome("", info);
        }

        /// <summary>
        /// eg:[服务端127.0.0.1][type]:info
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public static void ServerPrint(InfoType type, string info)
        {
            OnNewMessageCome(String.Format(ServerTitle, type), info, Color.Green);
        }

        /// <summary>
        /// eg:[客户端127.0.0.1][type]:info
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public static void ClientPrint(string remoteEndPoint, InfoType type, string info)
        {
            OnNewMessageCome(String.Format(ClientTitle, remoteEndPoint, type), info, Color.Blue);
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
    public delegate void NewMessageComeEventHandler(string title, string message, Color color = new Color());


    /// <summary>
    /// 消息类型
    /// </summary>
    public enum InfoType
    {
        信息,
        请求,
        响应,
        发送,
    }
}
