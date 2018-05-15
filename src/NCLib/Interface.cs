using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// 结果类接口
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// 基本结果(只读)
        /// </summary>
        baseResult BaseResult
        {
            get;
        }
        /// <summary>
        /// 结果信息(只读)
        /// </summary>
        string Info
        {
            get;
        }
    }

    /// <summary>
    /// Client逻辑的调用接口
    /// </summary>
    public interface IClientLogic
    {
        /// <summary>
        /// 登录请求
        /// </summary>
        /// <param name="id">账号</param>
        /// <param name="password">密码</param>
        /// <returns>结果</returns>
        IResult Login(string id, string password);
        /// <summary>
        /// 异步登录请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="callBack"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IAsyncResult BeginLogin(AsyncCallback callback, string id, string password, object state = null);
        /// <summary>
        /// 创建一个会议室房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="password">房间密码</param>
        /// <returns></returns>
        IResult CreateNCRoom(string roomId, string password = "");
        /// <summary>
        /// 异步创建一个会议室房间
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="roomId"></param>
        /// <param name="password"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        IAsyncResult BeginCreateNCRoom(AsyncCallback callback, string roomId, string password = "", object state = null);
        /// <summary>
        /// 加入一个会议室房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="password">房间密码</param>
        /// <returns></returns>
        IResult JoinNCRoom(string roomId, string password = "");
        /// <summary>
        /// 异步加入一个会议室房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="password">房间密码</param>
        /// <returns></returns>
        IAsyncResult BeginJoinNCRoom(AsyncCallback callback, string roomId, string password = "", object state = null);
        /// <summary>
        /// 离开一个会议室房间
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        IResult ExitNCRoom(string roomId);
        /// <summary>
        /// 连接OMCS服务端
        /// </summary>
        /// <param name="serverIP">本地IP</param>
        /// <param name="serverPort">端口</param>
        void ConnectOMCS(string serverIP, int serverPort);
        /// <summary>
        /// 异步连接OMCS服务端
        /// </summary>
        /// <param name="serverIP">本地IP</param>
        /// <param name="serverPort">端口</param>
        IAsyncResult BeginConnectOMCS(AsyncCallback callback, string serverIP, int serverPort, object state = null);
        /// <summary>
        /// 指示某人静音
        /// </summary>
        /// <param name="guestId">来访者ID</param>
        IResult MuteUser(string roomid, string guestId);
    }

    /// <summary>
    /// 服务端接口
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// 服务器状态
        /// </summary>
        ServerState IsInit
        {
            get;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 关闭服务器
        /// </summary>
        void Close();
    }

    /// <summary>
    /// 服务端调用数据库接口
    /// </summary>
    public interface IServerCallDatabase
    {
        /// <summary>
        /// SQL执行结果（只读）
        /// </summary>
        DataSet TmpDataSet
        {
            get;
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="user">数据库账号</param>
        /// <param name="password">密码</param>
        /// <param name="url">数据库地址</param>
        /// <param name="database">数据库名称</param>
        /// <returns>链接结果</returns>
        IResult ConnectDatabase(string user, string password, string url, string database);
        /// <summary>
        /// 断开数据库连接
        /// </summary>
        void CloseDatabase();
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <param name="userId">用户ID</param>
        /// <param name="password">密码</param>
        /// <returns>处理结果</returns>
        IResult AddUser(UserInfo info, string password);
        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="password">密码</param>
        /// <returns>结果</returns>
        IResult CheckUserInfo(UserInfo info, string password);
        /// <summary>
        /// 是否特权用户
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>结果</returns>
        bool IsPrerogative(UserInfo info);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        void DeletUser(string userId);
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="friendId">好友ID</param>
        /// <returns>调用结果</returns>
        //IResult AddFriend(string userId, string friendId);
        /// <summary>
        /// 删除关联好友
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="friendId">好友ID</param>
        //void DeleteFriend(string userId, string friendId);
        /// <summary>
        /// 查找用户好友
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>处理结果</returns>
        //IResult GetUserFriends(string userId);
        /// <summary>
        /// 执行SQL结构化查询语句
        /// </summary>
        /// <param name="sql">结构化查询语句</param>
        /// <param name="tableTitle">结果标题</param>
        /// <returns>执行结果</returns>
        IResult ExecuteStructuredQueryLanguage(string sql, string tableTitle);
    }

    /// <summary>
    /// 套接字接口
    /// </summary>
    public interface ISocketAPI
    {
        /// <summary>
        /// 接入socket
        /// </summary>
        /// <param name="IP">IP</param>
        /// <param name="port">端口</param>
        /// /// <param name="listen">监听数目(server) 本地端口(Client)</param>
        /// <param name="AccessAction">回调函数</param>
        /// <returns>处理结果</returns>
        IResult Access(string IP, int port, int listen_or_port, Action<string> callBack = null);
        /// <summary>
        /// 释放连接Socket
        /// </summary>
        /// <param name="key">字典字</param>
        void DisposeSocket(string key);
        /// <summary>
        /// 识别信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns>识别结果</returns>
        IResult RecognizeInfo(byte info);
    }

    /// <summary>
    /// Client通信API接口
    /// </summary>
    public interface IClientSocket : ISocketAPI
    {
        /// <summary>
        /// 异步发送请求(Client)
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>发送结果</returns>
        void Send(string info, Action<string> callBack = null);
        /// <summary>
        /// 异步接受响应(Client)
        /// </summary>
        /// <param name="callBack">回调函数</param>
        /// <returns>处理结果</returns>
        void Receive(Action<string> callBack = null);
    }

    /// <summary>
    /// Server通信API接口
    /// </summary>
    public interface IServerSocket : ISocketAPI
    {
        /// <summary>
        /// 客户端断线事件
        /// </summary>
        event Action<string> OnClientOffline;
        /// <summary>
        /// 触发异常事件
        /// </summary>
        event Action<Exception> OnException;

        /// <summary>
        /// 异步发送响应(Server)
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>发送结果</returns>
        void Send(string remoteEndPoint, string info, Action<string> callBack = null);
        /// <summary>
        /// 异步接受请求(Server)
        /// </summary>
        /// <param name="callBack">回调函数</param>
        /// <returns>处理结果</returns>
        void Receive(string remoteEndPoint, ReceiveCallBack callBack = null);
    }

    /// <summary>
    /// Server回调函数
    /// </summary>
    /// <param name="remoteEndPoint">请求的节点</param>
    /// <param name="info">请求信息</param>
    public delegate void ReceiveCallBack(string remoteEndPoint, string info);

    /// <summary>
    /// AOP方法函数
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="minfo"></param>
    public delegate void AOPMethodHandler(IMessage msg, string minfo);
}
