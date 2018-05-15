using Client2Server;
using NCLib;
using NCLib.AOP;
using Server.ServerData;
using Server2DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 服务端
    /// </summary>
    //[AOP]
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
        /// 运行时服务器数据
        /// </summary>
        private IRunningData runningData;

        public ServerState IsInit
        {
            get
            {
                return runningData.IsInit;
            }
        }

        public Server(string IP, int port, string user, string password, string url, string database)
        {
            ServerCallDatabase = new ServerCallDatabase();
            ServerSocket = new ServerSocket();
            ServerSocket.OnClientOffline += ClientOffline;
            ServerSocket.OnException += OnException;
            runningData = new RunningData(IP, port, user, password, url, database);
            //Clients_remoteEndPoint = new Dictionary<string, UserInfo>();
            //Clients_id = new Dictionary<string, string>();
            //this.ip = IP;
            //this.port = port;
            //this.user = user;
            //this.password = password;
            //this.url = url;
            //this.database = database;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (runningData.IsInit == ServerState.初始化完成)
            {
                Terminal.ServerPrint(InfoType.信息, "服务器已经运行");
                return;
            }
            else if (runningData.IsInit == ServerState.初始化中)
            {
                Terminal.ServerPrint(InfoType.信息, "服务器初始化中...");
                return;
            }
            try
            {
                runningData.IsInit = ServerState.初始化中;
                Terminal.SetServerTitle(runningData.Ip, runningData.Port);
                Terminal.ServerPrint(InfoType.信息, "服务器初始化中...");
                Terminal.ServerPrint(InfoType.信息, "连接数据库");
                IResult result = ServerCallDatabase.ConnectDatabase(runningData.DbUser, runningData.DbPassword, runningData.DbUrl, runningData.Database);
                if (result.BaseResult == baseResult.Faild)
                {
                    Terminal.ServerPrint(InfoType.异常, "初始化失败 原因:" + result.Info);
                    runningData.IsInit = ServerState.未初始化;
                    return;
                }
                Terminal.ServerPrint(InfoType.信息, "数据库连接成功");
                Terminal.ServerPrint(InfoType.信息, "初始化套接字");
                result = ServerSocket.Access(runningData.Ip, runningData.Port, 10, Accept);
                if (result.BaseResult == baseResult.Faild)
                {
                    Terminal.ServerPrint(InfoType.异常, "初始化失败 原因:" + result.Info);
                    runningData.IsInit = ServerState.未初始化;
                    return;
                }
                runningData.IsInit = ServerState.初始化完成;
            }
            catch (Exception e)
            {
                Terminal.ServerPrint(InfoType.异常, "初始化失败 原因:" + e.Message);
                runningData.IsInit = ServerState.未初始化;
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
            runningData.IsInit = ServerState.关闭;
        }

        #region 业务逻辑
        //[AOPMethod("创建答疑室请求", PreProceed, PostProceed)]
        private IResult CreateRoomRequest(IReceiveInfo receiveInfo, UserInfo userInfo)
        {
            string roomID = receiveInfo.Message["房间名"];
            string password = receiveInfo.Message["密码"];
            Terminal.ClientPrint(receiveInfo.RemoteEndPoint, InfoType.请求, "创建答疑室=>房间号:" + roomID);
            IResult result;
            if (!runningData.Clients_remoteEndPoint[receiveInfo.RemoteEndPoint].IsPrerogative)//权限判定
            {
                result = new Result(baseResult.Faild, "用户没有创建权限");
            }
            else if (runningData.ChatRoom_id.ContainsKey(roomID))//房间存在判定
            {
                result = new Result(baseResult.Faild, "该答疑室已存在");
            }
            else//成功创建
            {
                ChatRoom chatroom = new ChatRoom(password);
                chatroom.RoomEmpty += RoomEmpty;
                chatroom.Builder = userInfo;
                chatroom.GroupName = roomID;
                chatroom.memberList.Add(userInfo);
                runningData.ChatRoom_id[roomID] = chatroom;
                ((User_Server)userInfo).RoomIDs.Add(roomID);
                result = new Result(baseResult.Successful, "房间号" + roomID);
            }
            if (result.BaseResult == baseResult.Faild)
            {
                Terminal.ServerPrint(InfoType.响应, "<" + receiveInfo.RemoteEndPoint + ">:" + "创建失败 描述:" + result.Info);
            }
            else
            {
                Terminal.ServerPrint(InfoType.响应, "<" + receiveInfo.RemoteEndPoint + ">:" + "创建成功 描述:" + result.Info);
            }
            ServerSocket.Send(receiveInfo.RemoteEndPoint, MessageTranslate.EncapsulationInfo(MessageContent.创建答疑室, MessageType.响应, result.BaseResult.ToString(), result.Info));
            return result;
        }

        private IResult GetUserListInRoomRequest(IReceiveInfo receiveInfo, UserInfo userInfo, string remoteEndPoint, string roomId)
        {
            throw new NotImplementedException();
        }

        private IResult JoinRoomRequest(IReceiveInfo receiveInfo, UserInfo userInfo)
        {
            string roomID = receiveInfo.Message["房间名"];
            string password = receiveInfo.Message["密码"];
            Terminal.ClientPrint(receiveInfo.RemoteEndPoint, InfoType.请求, "加入答疑室=>房间号:" + roomID);
            IResult result;
            if (!runningData.ChatRoom_id.ContainsKey(roomID))//房间存在判定
            {
                result = new Result(baseResult.Faild, "该答疑室未被创建");
            }
            else if (runningData.ChatRoom_id[roomID].memberList.Contains(userInfo))
            {
                result = new Result(baseResult.Faild, "用户已在答疑室");
            }
            else if (runningData.ChatRoom_id[roomID].Password != password)
            {
                result = new Result(baseResult.Faild, "密码错误");
            }
            else//成功加入
            {
                runningData.ChatRoom_id[roomID].memberList.Add(userInfo);
                ((User_Server)userInfo).RoomIDs.Add(roomID);
                result = new Result(baseResult.Successful, "房间号" + roomID);
            }
            if (result.BaseResult == baseResult.Faild)
            {
                Terminal.ServerPrint(InfoType.响应, "<" + receiveInfo.RemoteEndPoint + ">:" + "加入失败 描述:" + result.Info);
                ServerSocket.Send(receiveInfo.RemoteEndPoint, MessageTranslate.EncapsulationInfo(MessageContent.加入答疑室, MessageType.响应, result.BaseResult.ToString(), result.Info, "error"));
            }
            else
            {
                Terminal.ServerPrint(InfoType.响应, "<" + receiveInfo.RemoteEndPoint + ">:" + "加入成功 描述:" + result.Info);
                ServerSocket.Send(receiveInfo.RemoteEndPoint, MessageTranslate.EncapsulationInfo(MessageContent.加入答疑室, MessageType.响应, result.BaseResult.ToString(), result.Info, runningData.ChatRoom_id[roomID].Builder.UserId));
            }
            return result;
        }

        private IResult LoginRequest(IReceiveInfo receiveInfo, UserInfo userInfo)
        {
            Terminal.ClientPrint(receiveInfo.RemoteEndPoint, InfoType.请求, "登录客户端=>学号:" + receiveInfo.Message["学号"]);
            //之前的ID；
            string password = receiveInfo.Message["密码"];
            string lastUserId = userInfo.UserId == null ? "" : userInfo.UserId;
            userInfo.UserId = receiveInfo.Message["学号"];
            IResult result;
            //判断是否已登录
            if (runningData.Clients_id.ContainsKey(userInfo.UserId))
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
                    if (runningData.Clients_id.ContainsKey(lastUserId))
                        runningData.Clients_id.Remove(lastUserId);
                    runningData.Clients_id[userInfo.UserId] = receiveInfo.RemoteEndPoint;
                    result = new Result(baseResult.Successful, IsPrerogative.ToString());
                }
                else
                    result = new Result(baseResult.Faild, "错误的账号或密码");
            }

            if (result.BaseResult == baseResult.Successful)
            {
                Terminal.ServerPrint(InfoType.响应, "<" + receiveInfo.RemoteEndPoint + ">:" + "成功登录 权限:" + result.Info);
            }
            else
            {
                Terminal.ServerPrint(InfoType.响应, "<" + receiveInfo.RemoteEndPoint + ">:" + "登录失败 原因:" + result.Info);
            }
            ServerSocket.Send(receiveInfo.RemoteEndPoint, MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.响应, result.BaseResult.ToString(), result.Info));
            return result;
        }

        private IResult ExitRoomInform(IReceiveInfo receiveInfo, UserInfo userInfo)
        {
            string roomID = receiveInfo.Message["房间名"];
            Terminal.ClientPrint(receiveInfo.RemoteEndPoint, InfoType.信息, userInfo.UserId + "主动离开答疑室=>房间号:" + roomID);
            IResult result;
            if (!runningData.ChatRoom_id.ContainsKey(roomID))//房间存在判定
            {
                result = new Result(baseResult.Faild, "该答疑室未被创建");
            }
            else if (!runningData.ChatRoom_id[roomID].memberList.Contains(userInfo))
            {
                result = new Result(baseResult.Faild, "用户不在答疑室");
            }
            else
            {
                runningData.ChatRoom_id[roomID].memberList.Remove(userInfo);
                ((User_Server)userInfo).RoomIDs.Remove(roomID);
                if (runningData.ChatRoom_id.ContainsKey(roomID))
                    foreach (UserInfo user in runningData.ChatRoom_id[roomID].memberList)
                    {
                        ServerSocket.Send(runningData.Clients_id[user.UserId], MessageTranslate.EncapsulationInfo(MessageContent.某人退出答疑室, MessageType.通知, roomID, userInfo.UserId));
                    }
                result = new Result(baseResult.Successful);
            }
            return result;
        }

        private IResult MuteUserInform(IReceiveInfo receiveInfo, UserInfo userInfo)
        {
            string roomID = receiveInfo.Message["房间名"];
            string userID = receiveInfo.Message["学号"];
            bool isMute = bool.Parse(receiveInfo.Message["是否静音"]);
            IResult result;
            if (!runningData.ChatRoom_id.ContainsKey(roomID))//房间存在判定
            {
                result = new Result(baseResult.Faild, "该答疑室未被创建");
            }
            else if (!runningData.ChatRoom_id[roomID].memberList.Contains(runningData.Clients_remoteEndPoint[runningData.Clients_id[userID]]))
            {
                result = new Result(baseResult.Faild, "用户不在答疑室");
            }
            else
            {
                if (isMute)
                {
                    Terminal.ClientPrint(receiveInfo.RemoteEndPoint, InfoType.信息, "禁止" + userID + "发言 答疑室=>房间号:" + roomID);
                }
                else
                {
                    Terminal.ClientPrint(receiveInfo.RemoteEndPoint, InfoType.信息, "允许" + userID + "发言 答疑室=>房间号:" + roomID);
                }
                ServerSocket.Send(runningData.Clients_id[userID], MessageTranslate.EncapsulationInfo(MessageContent.静音自己, MessageType.通知, roomID, isMute.ToString()));
                result = new Result(baseResult.Successful);
            }
            return result;
        }

        private IResult RegisterRequest(IReceiveInfo receiveInfo, UserInfo userInfo, string password)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 回调函数
        private void Accept(string remoteEndPoint)
        {
            Terminal.ServerPrint(InfoType.信息, "接收到来自:<" + remoteEndPoint + ">连接请求。建立连接");
            runningData.Clients_remoteEndPoint[remoteEndPoint] = new User_Server();
            ServerSocket.Receive(remoteEndPoint, Receive);
        }

        private void Receive(string remoteEndPoint, string info)
        {
            Dictionary<string, string> message;
            MessageContent messageContent;
            MessageType messageType;
            message = MessageTranslate.AnalyseInfo(info, out messageContent, out messageType);
            IReceiveInfo receiveInfo = new ReceiveInfo(message, remoteEndPoint);
            switch (messageContent)
            {
                case MessageContent.错误:
                    Terminal.ClientPrint(remoteEndPoint, InfoType.发送, info);
                    break;
                case MessageContent.登录:
                    if (messageType == MessageType.请求)
                    {
                        LoginRequest(receiveInfo, runningData.Clients_remoteEndPoint[remoteEndPoint]);
                    }
                    break;
                case MessageContent.创建答疑室:
                    if (messageType == MessageType.请求)
                    {
                        CreateRoomRequest(receiveInfo, runningData.Clients_remoteEndPoint[remoteEndPoint]);
                    }
                    break;
                case MessageContent.加入答疑室:
                    if (messageType == MessageType.请求)
                    {
                        JoinRoomRequest(receiveInfo, runningData.Clients_remoteEndPoint[remoteEndPoint]);
                    }
                    break;
                case MessageContent.退出答疑室:
                    if (messageType == MessageType.通知)
                    {
                        ExitRoomInform(receiveInfo, runningData.Clients_remoteEndPoint[remoteEndPoint]);
                    }
                    break;
                case MessageContent.静音某人:
                    if (messageType == MessageType.通知)
                    {
                        MuteUserInform(receiveInfo, runningData.Clients_remoteEndPoint[remoteEndPoint]);
                    }
                    break;
            }
        }

        //void PreProceed(IMessage msg, string minfo)
        //{

        //}

        //void PostProceed(IMessage msg, string minfo)
        //{

        //}
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
            if (runningData.Clients_remoteEndPoint.ContainsKey(remoteEndPoint))
            {
                id = runningData.Clients_remoteEndPoint[remoteEndPoint].UserId;
                //从在的答疑室中清除
                foreach (string roomID in ((User_Server)runningData.Clients_remoteEndPoint[remoteEndPoint]).RoomIDs)
                {
                    runningData.ChatRoom_id[roomID].memberList.Remove(runningData.Clients_remoteEndPoint[remoteEndPoint]);
                    if (runningData.ChatRoom_id.ContainsKey(roomID))
                        foreach (UserInfo user in runningData.ChatRoom_id[roomID].memberList)
                        {
                            ServerSocket.Send(runningData.Clients_id[user.UserId], MessageTranslate.EncapsulationInfo(MessageContent.某人退出答疑室, MessageType.通知, roomID, id));
                        }
                }
                runningData.Clients_remoteEndPoint.Remove(remoteEndPoint);
            }
            if (id != null)
                if (runningData.Clients_id.ContainsKey(id))
                    runningData.Clients_id.Remove(id);
        }

        /// <summary>
        /// 触发异常事件
        /// </summary>
        /// <param name="e"></param>
        private void OnException(Exception e)
        {
            Terminal.ServerPrint(InfoType.异常, e.Message + e.StackTrace);
        }

        /// <summary>
        /// 答疑室空事件
        /// </summary>
        /// <param name="chatRoom"></param>
        private void RoomEmpty(ChatRoom chatRoom)
        {
            string roomid = chatRoom.GroupName;
            if (runningData.ChatRoom_id.ContainsKey(roomid))
            {
                runningData.ChatRoom_id.Remove(roomid);
                Terminal.ServerPrint(InfoType.信息, roomid + "已空，自动关闭");
            }
        }
        #endregion
    }
}
