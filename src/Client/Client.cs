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
    public class Client : IClientCallOMCS
    {
        private IClientSocket clientSocket;
        private CallOMCS callOMCS;
        private string serverIP;
        private int serverPort, localPort;
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
        public ClientUser UserInfo;

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
        public event Action<string> SomeoneExit;
        #endregion

        public Client(string serverIP, int serverPort, int localPort)
        {
            clientSocket = new ClientSocket();
            UserInfo = new ClientUser();
            LOCK = new object();
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.localPort = localPort;
        }

        public void Init()
        {
            clientSocket.Access(serverIP, serverPort, localPort, Accept);
        }

        #region 客户端逻辑
        public IResult Login(string id, string password)
        {
            lock (LOCK)
            {
                this.UserInfo = new ClientUser(id);
                if (waitingRespond) { throw new Exception("产生过未上锁的异步请求"); }
                this.waitingRespond = true;
                clientSocket.Send(MessageTranslate.EncapsulationInfo(MessageContent.登录, MessageType.请求, id, password));
                while (waitingRespond) { }//等待响应
                if(respondMessage["验证结果"] == baseResult.Successful.ToString())
                {
                    this.UserInfo.UserState = UserState.已登录;
                    UserInfo.IsPrerogative = Boolean.Parse(respondMessage["权限"]);
                    return new Result(baseResult.Successful, respondMessage["权限"]);
                }
                return new Result(baseResult.Faild, respondMessage["权限"]);
            }           
        }
        public IAsyncResult BeginLogin(string id, string password, AsyncCallback callback, object state = null)
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

        public void CloseNCRoom()
        {
            throw new NotImplementedException();
        }

        public void ConnectOMCS(string serverIP, int serverPort)
        {
            if(UserInfo.UserState == UserState.未登录) { throw new Exception("未登录用户申请初始化"); }
            if (callOMCS != null)//登录成功 实例CallOMCS
                callOMCS.Dispose();
            if (UserInfo.IsPrerogative)
                callOMCS = new TeacherCallOMCS();
            callOMCS = new StudentCallOMCS();
            callOMCS.Initialize(UserInfo.UserId, "", serverIP, serverPort);
            callOMCS.ConnectEnded += ConnectEnded;
            callOMCS.SomeoneExit += SomeoneExit;
            callOMCS.SomeoneJoin += SomeoneJoin;
        }
        public IAsyncResult BeginConnectOMCS(string serverIP, int serverPort, AsyncCallback callback, object state)
        {
            var asyncResult = new NCAsyncResult(callback, state);
            var thr = new Thread(() =>
            {
                try
                {
                    ConnectOMCS(serverIP, serverPort);
                    asyncResult.SetCompleted(baseResult.Successful);
                }
                catch(Exception e)
                {
                    asyncResult.SetCompleted(baseResult.Faild,e.Message);
                }
            });
            thr.IsBackground = true;
            thr.Start();
            return asyncResult;
        }

        public IResult CreateNCRoom(string roomId, string password = null)
        {
            throw new NotImplementedException();
        }

        public IResult JoinNCRoom(string roomId, string password = null)
        {
            throw new NotImplementedException();
        }

        public IResult MuteToUser(string guestId)
        {
            throw new NotImplementedException();
        }
        #endregion

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
            }
        }
        #endregion

    }
}
