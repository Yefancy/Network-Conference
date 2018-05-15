using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ServerData
{
    /// <summary>
    /// 运行时数据
    /// </summary>
    public interface IRunningData
    {
        /// <summary>
        /// 端点对用户字典（记录所有端点）
        /// </summary>
        Dictionary<string, UserInfo> Clients_remoteEndPoint { get; set; }
        /// <summary>
        /// 学号对端点字典（记录登录中的账号）
        /// </summary>
        Dictionary<string, string> Clients_id { get; set; }
        /// <summary>
        /// 答疑室ID对组类字典(记录正在服务的答疑室)
        /// </summary>
        Dictionary<string, ChatRoom> ChatRoom_id { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        string Ip { get; set; }
        /// <summary>
        /// SQLServer用户
        /// </summary>
        string DbUser { get; set; }
        /// <summary>
        /// SQLServer密码
        /// </summary>
        string DbPassword { get; set; }
        /// <summary>
        /// SQLServer地址
        /// </summary>
        string DbUrl { get; set; }
        /// <summary>
        /// 数据库名
        /// </summary>
        string Database { get; set; }
        /// <summary>
        /// 监听端口
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// 服务器状态
        /// </summary>
        ServerState IsInit { get; set; }
    }

    /// <summary>
    /// 接收到的信息
    /// </summary>
    public interface IReceiveInfo
    {
        /// <summary>
        /// 解析后信息字典
        /// </summary>
        Dictionary<string, string> Message { get; }
        /// <summary>
        /// 远程请求的端点
        /// </summary>
        string RemoteEndPoint { get; }
    }
}
