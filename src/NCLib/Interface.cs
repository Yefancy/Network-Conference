using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// Client对OMCS.DLL的调用接口
    /// </summary>
    public interface IClientCallOMCS
    {
        /// <summary>
        /// 创建一个会议室房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="password">房间密码</param>
        /// <returns></returns>
        Result CreateNCRoom(string roomId, string password = null);
        /// <summary>
        /// 加入一个会议室房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="password">房间密码</param>
        /// <returns></returns>
        Result JoinNCRoom(string roomId, string password = null);
        /// <summary>
        /// 退出一个会议室房间
        /// </summary>
        void CloseNCRoom();
        /// <summary>
        /// 连接OMCS服务端
        /// </summary>
        /// <param name="serverIP">本地IP</param>
        /// <param name="serverPort">端口</param>
        void ConnectOMCS(string serverIP, int serverPort);
        /// <summary>
        /// 对某人静音
        /// </summary>
        /// <param name="guestId">来访者ID</param>
        void MuteToUser(string guestId);
    }

    /// <summary>
    /// 服务端业务逻辑接口
    /// </summary>
    public interface IServerLogic
    {
        /// <summary>
        /// 处理登陆请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="password">登录密码</param>
        /// <returns>处理结果</returns>
        Result LoginRequest(UserInfo userInfo, string password);
        /// <summary>
        /// 处理注册请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="password">注册密码</param>
        /// <returns>处理结果</returns>
        Result RegisterRequest(UserInfo userInfo, string password);
        /// <summary>
        /// 处理添加好友请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="friendId">好友Id</param>
        /// <returns>处理结果</returns>
        Result AddFriendRequest(ClientUser userInfo, string friendId);
        /// <summary>
        /// 处理删除好友请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="friendId">好友ID</param>
        /// <returns>处理结果</returns>
        Result DeleteFriendRequest(ClientUser userInfo, string friendId);
        /// <summary>
        /// 处理创建房间请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>处理结果</returns>
        Result CreateRoomRequest(ClientUser userInfo);
        /// <summary>
        /// 处理加入房间请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="roomId">房间ID</param>
        /// <param name="password">密码</param>
        /// <returns>处理结果</returns>
        Result JoinRoomRequest(ClientUser userInfo, string roomId, string password = null);
        /// <summary>
        /// 处理邀请好友请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="friendId">好友ID</param>
        /// <param name="roomId">房间ID</param>
        /// <returns>处理结果</returns>
        Result InviteFriendRequest(ClientUser userInfo, string friendId, string roomId);
        /// <summary>
        /// 处理静音某人请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="friendId">好友ID</param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        Result MuteToUserRequest(ClientUser userInfo, string friendId, string roomId);
        /// <summary>
        /// 处理获取房间用户表请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="roomId">房间ID</param>
        /// <returns>处理结果</returns>
        Result GetUserListInRoomRequest(ClientUser userInfo, string roomId);
        /// <summary>
        /// 处理获取好友列表请求
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>处理结果</returns>
        Result GetFriendsListInRoomRequest(ClientUser userInfo);
    }

    /// <summary>
    /// 服务端调用数据库接口
    /// </summary>
    public interface IServerCallDatabase
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="user">数据库账号</param>
        /// <param name="password">密码</param>
        /// <param name="url">数据库地址</param>
        /// <returns>链接结果</returns>
        Result ConnectDatabase(string user, string password, string url);
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
        Result AddUser(UserInfo info, string password);
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
        Result AddFriend(string userId, string friendId);
        /// <summary>
        /// 删除关联好友
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="friendId">好友ID</param>
        void DeleteFriend(string userId, string friendId);
		/// <summary>
        /// 查找用户好友
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>处理结果</returns>
        Result GetUserFriend(string password);
        /// <summary>
        /// 执行SQL结构化查询语句
        /// </summary>
        /// <param name="sql">结构化查询语句</param>
        /// <returns>执行结果</returns>
        Result ExecuteStructuredQueryLanguage(string sql);
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
        /// <param name="AccessAction">回调函数</param>
        /// <returns>处理结果</returns>
        Result Access(string IP, int port, Action<string> accessAction);
        /// <summary>
        /// 异步发送请求
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="accessAction">回调函数</param>
        /// <returns>发送结果</returns>
        Result SendRequest(object info, Action<string> accessAction);
        /// <summary>
        /// 异步接受响应
        /// </summary>
        /// <param name="accessAction">回调函数</param>
        /// <returns>处理结果</returns>
        Result ReceiveRespond(Action<string> accessAction);
        /// <summary>
        /// 释放与指定Socket
        /// </summary>
        /// <param name="key">字典字</param>
        void DisposeSocket(string key);
        /// <summary>
        /// 识别信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns>识别结果</returns>
        Result RecognizeInfo(byte info);
    }

    /// <summary>
    /// Login/SignUp2Server通信API接口
    /// </summary>
    public interface ILoginSignUP2Server : ISocketAPI
    { }

    /// <summary>
    /// MainForm2Server通信API接口
    /// </summary>
    public interface IMainForm2Server : ISocketAPI
    { }
}
