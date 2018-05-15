using Client.OMCS;
using Client2Server;
using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class Client : IClientLogic
    {
        private IClientSocket clientSocket;
        private CallOMCS callOMCS;
        /// <summary>
        /// 等待接受响应结果
        /// </summary>
        private bool waitingRespond = false;
        /// <summary>
        /// 锁对象 互斥保证同一时刻只能有一个异步请求在等待响应
        /// </summary>
        private object LOCK;
        /// <summary>
        /// 响应结果
        /// </summary>
        private Dictionary<string, string> respondMessage;
        public User_Client UserInfo;

        #region 事件集
        /// <summary>
        /// 连接结束事件
        /// </summary>
        public event Action<Result> ConnectEnded;
        /// <summary>
        /// 语音组加入事件传递
        /// </summary>
        public event Action<ChatMember> SomeoneJoin;
        /// <summary>
        /// 语音组离开事件传递
        /// </summary>
        public event Action<ChatMember> SomeoneExit;
        #endregion

        public Client()
        {
            clientSocket = new ClientSocket();
            UserInfo = new User_Client();
            LOCK = new object();
        }

        public void Init(string serverIP, int serverPort, int localPort)
        {
            clientSocket.Access(serverIP, serverPort, localPort, Accept);
        }

        #region 客户端逻辑
        public IResult Login(string id, string password)
        {
            lock (LOCK)
            {
                this.UserInfo = new User_Client(id);
                if (waitingRespond) { throw new Exception("产生过未上锁的异步请求"); }
                this.waitingRespond = true;
                clientSocket.Send(MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.请求, id, password));
                while (waitingRespond) { }//等待响应
                if (respondMessage["结果"] == baseResult.Successful.ToString())
                {
                    this.UserInfo.UserState = UserState.已登录;
                    UserInfo.IsPrerogative = Boolean.Parse(respondMessage["权限"]);
                    return new Result(baseResult.Successful, respondMessage["权限"]);
                }
                return new Result(baseResult.Faild, respondMessage["权限"]);
            }
        }
        public IAsyncResult BeginLogin(AsyncCallback callback, string id, string password, object state = null)
        {
            var asyncResult = new NCAsyncResult(callback, state);
            var thr = new Thread(() =>
            {
                asyncResult.SetCompleted(Login(id, password));
            });
            thr.IsBackground = true;
            thr.Start();
            return asyncResult;
        }

        public void ConnectOMCS(string serverIP, int serverPort)
        {
            if (UserInfo.UserState == UserState.未登录) { throw new Exception("未登录用户申请初始化"); }
            if (callOMCS != null)//登录成功 实例CallOMCS
                callOMCS.Dispose();
            if (UserInfo.IsPrerogative)
                callOMCS = new TeacherCallOMCS();
            else
                callOMCS = new StudentCallOMCS();
            callOMCS.Initialize(UserInfo.UserId, "", serverIP, serverPort);
            callOMCS.ConnectEnded += a => { ConnectEnded?.Invoke(a); };
            callOMCS.SomeoneExit += a => { SomeoneExit?.Invoke(a); };
            callOMCS.SomeoneJoin += a => { SomeoneJoin?.Invoke(a); };
        }
        public IAsyncResult BeginConnectOMCS(AsyncCallback callback, string serverIP, int serverPort, object state = null)
        {
            var asyncResult = new NCAsyncResult(callback, state);
            var thr = new Thread(() =>
            {
                try
                {
                    ConnectOMCS(serverIP, serverPort);
                    asyncResult.SetCompleted(baseResult.Successful);
                }
                catch (Exception e)
                {
                    asyncResult.SetCompleted(baseResult.Faild, e.Message);
                }
            });
            thr.IsBackground = true;
            thr.Start();
            return asyncResult;
        }

        public IResult CreateNCRoom(string roomId, string password = "")
        {
            lock (LOCK)
            {
                if (waitingRespond) { throw new Exception("产生过未上锁的异步请求"); }
                this.waitingRespond = true;
                clientSocket.Send(MessageTranslate.EncapsulationInfo(MessageContent.创建答疑室, MessageType.请求, roomId, password));
                while (waitingRespond) { }//等待响应
                if (respondMessage["结果"] == baseResult.Successful.ToString())
                {
                    ((TeacherCallOMCS)callOMCS).createRoom(roomId, UserInfo.UserId);
                    return new Result(baseResult.Successful, respondMessage["描述"]);
                }
                return new Result(baseResult.Faild, respondMessage["描述"]);
            }
        }
        public IAsyncResult BeginCreateNCRoom(AsyncCallback callback, string roomId, string password = "", object state = null)
        {
            var asyncResult = new NCAsyncResult(callback, state);
            var thr = new Thread(() =>
            {
                asyncResult.SetCompleted(CreateNCRoom(roomId, password));
            });
            thr.IsBackground = true;
            thr.Start();
            return asyncResult;
        }

        public IResult JoinNCRoom(string roomId, string password = "")
        {
            lock (LOCK)
            {
                if (waitingRespond) { throw new Exception("产生过未上锁的异步请求"); }
                this.waitingRespond = true;
                clientSocket.Send(MessageTranslate.EncapsulationInfo(MessageContent.加入答疑室, MessageType.请求, roomId, password));
                while (waitingRespond) { }//等待响应
                if (respondMessage["结果"] == baseResult.Successful.ToString())
                {
                    callOMCS.JoinRoom(roomId, respondMessage["创建者"]);
                    return new Result(baseResult.Successful, respondMessage["描述"]);
                }
                return new Result(baseResult.Faild, respondMessage["描述"]);
            }
        }
        public IAsyncResult BeginJoinNCRoom(AsyncCallback callback, string roomId, string password = "", object state = null)
        {
            var asyncResult = new NCAsyncResult(callback, state);
            var thr = new Thread(() =>
            {
                asyncResult.SetCompleted(JoinNCRoom(roomId, password));
            });
            thr.IsBackground = true;
            thr.Start();
            return asyncResult;
        }

        public IResult ExitNCRoom(string roomId)
        {
            clientSocket.Send(MessageTranslate.EncapsulationInfo(MessageContent.退出答疑室, MessageType.通知, roomId));
            callOMCS.ExitRoom();
            return new Result(baseResult.Successful);
        }

        public IResult MuteUser(string roomid, string guestId)
        {
            if (UserInfo.IsPrerogative == false)
                return new Result(baseResult.Faild, "没有权限");
            clientSocket.Send(MessageTranslate.EncapsulationInfo(MessageContent.静音某人, MessageType.通知, roomid, guestId));
            return new Result(baseResult.Successful);
        }
        #endregion

        public void Close()
        {
            if (callOMCS != null)
                callOMCS.Dispose();
            if (clientSocket != null)
                clientSocket.DisposeSocket("");
        }

        #region 回调函数
        private void Accept(string remoteEndPoint)
        {
            clientSocket.Receive(Receive);
        }

        private void Receive(string info)
        {
            //分析信息
            Dictionary<string, string> receiveMessage = new Dictionary<string, string>();
            MessageContent messageContent;
            MessageType messageType;
            receiveMessage = MessageTranslate.AnalyseInfo(info, out messageContent, out messageType);
            switch (messageType)
            {
                case MessageType.错误:
                    break;
                case MessageType.响应:
                    respondMessage = receiveMessage;
                    if (waitingRespond)
                        waitingRespond = false;//得到响应 停止等待
                    else
                        throw new Exception("未请求的响应到来");
                    break;
                case MessageType.通知://未封装的
                    if (messageContent == MessageContent.静音自己)
                    {
                        callOMCS.Mute(bool.Parse(respondMessage["是否静音"]));
                    }
                    else if (messageContent == MessageContent.某人退出答疑室)
                    {
                        //
                    }
                    break;
            }
        }
        #endregion

    }
}
