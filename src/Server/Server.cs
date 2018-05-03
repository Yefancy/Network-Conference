using Client2Server;
using NCLib;
using Server2DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 服务端
    /// </summary>
    public class Server : IServer
    {
        /// <summary>
        /// 数据库
        /// </summary>
        private IServerCallDatabase ServerCallDatabase;
        /// <summary>
        /// 服务器套接字
        /// </summary>
        private IServerSocket ServerSocket;
        /// <summary>
        /// 端点对用户字典（记录所有端点）
        /// </summary>
        private Dictionary<string, UserInfo> Clients_remoteEndPoint;
        /// <summary>
        /// 学号对端点字典（记录登录中的账号）
        /// </summary>
        private Dictionary<string, string> Clients_id;
        private string ip, user, password, url, database;
        private int port;
        private ServerState _isInit = ServerState.未初始化;
        
        public ServerState IsInit
        {
            get
            {
                return _isInit;
            }
        }

        public Server(string IP, int port, string user, string password, string url, string database)
        {
            ServerCallDatabase = new ServerCallDatabase();
            ServerSocket = new ServerSocket();
            ServerSocket.OnClientOffline += ClientOffline;
            Clients_remoteEndPoint = new Dictionary<string, UserInfo>();
            Clients_id = new Dictionary<string, string>();
            this.ip = IP;
            this.port = port;
            this.user = user;
            this.password = password;
            this.url = url;
            this.database = database;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (_isInit == ServerState.初始化完成)
            {
                Terminal.ServerPrint(InfoType.信息, "服务器已经运行");
                return;
            }
            else if (_isInit == ServerState.初始化中)
            {
                return;
            }
            try
            {
                _isInit = ServerState.初始化中;
                Terminal.SetServerTitle(this.ip, this.port);
                Terminal.ServerPrint(InfoType.信息, "服务器初始化中...");
                Terminal.ServerPrint(InfoType.信息, "连接数据库");
                IResult result = ServerCallDatabase.ConnectDatabase(this.user, this.password, this.url, this.database);
                if (result.BaseResult == baseResult.Faild)
                {
                    Terminal.ServerPrint(InfoType.信息, "初始化失败 原因:" + result.Info);
                    _isInit = ServerState.未初始化;
                    return;
                }
                Terminal.ServerPrint(InfoType.信息, "数据库连接成功");
                Terminal.ServerPrint(InfoType.信息, "初始化套接字");
                result = ServerSocket.Access(ip, port, 10, Accept);
                if (result.BaseResult == baseResult.Faild)
                {
                    Terminal.ServerPrint(InfoType.信息, "初始化失败 原因:" + result.Info);
                    _isInit = ServerState.未初始化;
                    return;
                }
                _isInit = ServerState.初始化完成;
            }
            catch (Exception e)
            {
                Terminal.ServerPrint(InfoType.信息, "初始化失败 原因:" + e.Message);
                _isInit = ServerState.未初始化;
                throw e;
            }
            Terminal.ServerPrint(InfoType.信息, "初始化完成");
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Close()
        {
            ServerCallDatabase.CloseDatabase();
            ServerSocket.DisposeSocket("");
            _isInit = ServerState.关闭;
        }

        #region 业务逻辑
        private void CreateRoomRequest(Dictionary<string, string> message, string remoteEndPoint, ClientUser userInfo)
        {
            throw new NotImplementedException();
        }

        private void GetUserListInRoomRequest(Dictionary<string, string> message, string remoteEndPoint, ClientUser userInfo, string roomId)
        {
            throw new NotImplementedException();
        }

        private void JoinRoomRequest(Dictionary<string, string> message, string remoteEndPoint, ClientUser userInfo, string roomId, string password = null)
        {
            throw new NotImplementedException();
        }

        private void LoginRequest(Dictionary<string, string> message, string remoteEndPoint, UserInfo userInfo, string password)
        {
            Terminal.ClientPrint(remoteEndPoint, InfoType.请求, "登录客户端=>学号:" + message["学号"]);
            //之前的ID；
            string lastUserId = userInfo.UserId==null? "":userInfo.UserId;
            userInfo.UserId = message["学号"];
            Result result;
            //判断是否已登录
            if (Clients_id.ContainsKey(userInfo.UserId))
                result = new Result(baseResult.Faild, "该用户已登录");
            else
            {
                Terminal.ServerPrint(InfoType.信息, "访问数据库中......");
                //验证结果
                if (ServerCallDatabase.CheckUserInfo(userInfo, password).BaseResult == baseResult.Successful)
                {
                    //是否权限用户
                    bool IsPrerogative = ServerCallDatabase.IsPrerogative(userInfo);
                    userInfo.IsPrerogative = IsPrerogative;
                    //已登录了 移除登陆
                    if(Clients_id.ContainsKey(lastUserId))
                        Clients_id.Remove(lastUserId);
                    Clients_id[userInfo.UserId] = remoteEndPoint;
                    result = new Result(baseResult.Successful, IsPrerogative.ToString());
                }
                else
                    result = new Result(baseResult.Faild, "错误的账号或密码");
            }

            if (result.BaseResult == baseResult.Successful)
            {
                Terminal.ServerPrint(InfoType.响应, "<" + remoteEndPoint + ">:" + "成功登录 权限:" + result.Info);
                ServerSocket.Send(remoteEndPoint, MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.响应, result.BaseResult.ToString(), result.Info));
            }
            else
            {
                Terminal.ServerPrint(InfoType.响应, "<" + remoteEndPoint + ">:" + "登录失败 原因:" + result.Info);
                ServerSocket.Send(remoteEndPoint, MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.响应, result.BaseResult.ToString(), result.Info));
            }
        }

        private void MuteToUserRequest(Dictionary<string, string> message, string remoteEndPoint, ClientUser userInfo, string friendId, string roomId, bool mute)
        {
            throw new NotImplementedException();
        }

        private void RegisterRequest(Dictionary<string, string> message, string remoteEndPoint, UserInfo userInfo, string password)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 回调函数
        private void Accept(string remoteEndPoint)
        {
            Terminal.ServerPrint(InfoType.信息, "接收到来自:<" + remoteEndPoint + ">连接请求。建立连接");
            Clients_remoteEndPoint[remoteEndPoint] = new ClientUser();
            ServerSocket.Receive(remoteEndPoint, Receive);
        }

        private void Receive(string remoteEndPoint, string info)
        {
            Dictionary<string, string> message;
            MessageContent messageContent;
            MessageType messageType;
            message = MessageTranslate.AnalyseInfo(info, out messageContent, out messageType);
            switch (messageContent)
            {
                case MessageContent.错误:
                    Terminal.ClientPrint(remoteEndPoint, InfoType.发送, info);
                    break;
                case MessageContent.登录:
                    if (messageType == MessageType.请求)
                    {
                        LoginRequest(message, remoteEndPoint, Clients_remoteEndPoint[remoteEndPoint], message["密码"]);
                    }
                    else if (messageType == MessageType.响应)
                    {

                    }
                    break;
            }
        }
        #endregion

        #region 事件监听
        /// <summary>
        /// 客户端掉线事件
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        private void ClientOffline(string remoteEndPoint)
        {
            Terminal.ServerPrint(InfoType.信息, "客户端<" + remoteEndPoint + ">断开了连接。服务器释放通信连接");
            string id = "";
            if (Clients_remoteEndPoint.ContainsKey(remoteEndPoint))
            {
                id = Clients_remoteEndPoint[remoteEndPoint].UserId;
                Clients_remoteEndPoint.Remove(remoteEndPoint);
            }
            if (Clients_id.ContainsKey(id))
                Clients_id.Remove(id);
        }
        #endregion
    }
}
