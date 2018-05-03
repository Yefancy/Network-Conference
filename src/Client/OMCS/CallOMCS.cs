using NCLib;
using OMCS.Passive;
using OMCS.Passive.Video;
using OMCS.Passive.WhiteBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// 接入OMCS
    /// </summary>
    public abstract class CallOMCS: IDisposable
    {
        /// <summary>
        /// 设备管理器
        /// </summary>
        protected IMultimediaManager multimediaManager;
        /// <summary>
        /// 聊天组容器 负责语音传输
        /// </summary>
        protected ChatContainer chatContainer;
        /// <summary>
        /// 摄像头连接器
        /// </summary>
        protected DynamicCameraConnector CameraConnector;
        /// <summary>
        /// 白板连接器（控件）
        /// </summary>
        public WhiteBoardConnector WhiteBoardControl;

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

        #region 事件监听
        /// <summary>
        /// 连接结束事件
        /// </summary>
        /// <param name="result"></param>
        private void OnConnectEnded(ConnectResult result)
        {
            if (result == ConnectResult.Succeed)
                ConnectEnded?.Invoke(new Result(baseResult.Successful));
            else
                ConnectEnded?.Invoke(new Result(baseResult.Faild, result.ToString()));
        }
        #endregion

        public CallOMCS()
        {
            CameraConnector = new DynamicCameraConnector();
            CameraConnector.ConnectEnded += OnConnectEnded;
            WhiteBoardControl = new WhiteBoardConnector();
            WhiteBoardControl.ConnectEnded += OnConnectEnded;
            multimediaManager = MultimediaManagerFactory.GetSingleton();
            multimediaManager.ChannelMode = ChannelMode.P2PDisabled;
            multimediaManager.SecurityLogEnabled = false;
            multimediaManager.SpeakerIndex = 0;
        }

        #region 子类可覆写
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        public abstract void Initialize(string id, string password, string IP, int port);
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            chatContainer.Close();
            CameraConnector.Dispose();
            //WhiteBoardControl.Dispose();
            multimediaManager.Dispose();
        }
        #endregion

        /// <summary>
        /// 绑定chatContainer事件
        /// </summary>
        protected void BindingEvent()
        {
            chatContainer.SomeoneJoin += SomeoneJoin;//传递事件
            chatContainer.SomeoneExit += SomeoneExit;
        }

        /// <summary>
        /// 加入答疑室
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <param name="teacherID">教师ID</param>
        public void JoinRoom(string roomID, string teacherID)
        {
            if (chatContainer == null)
                throw new Exception("未初始化CallOMCS类实例");    
            ExitRoom(); //离开上一个答疑室
            chatContainer.JoinChatGroup(roomID);
            CameraConnector.BeginConnect(teacherID);
            WhiteBoardControl.BeginConnect(roomID);
        }

        /// <summary>
        /// 离开答疑室
        /// </summary>
        public void ExitRoom()
        {
            if (CameraConnector.Connected)
                CameraConnector.Disconnect();
            if (WhiteBoardControl.Connected)
                WhiteBoardControl.Disconnect();
            if (chatContainer != null)
                chatContainer.Close();
            multimediaManager.OutputVideo = false;
            multimediaManager.OutputAudio = false;
        }

        /// <summary>
        /// 是否静音（不发送语音帧）
        /// </summary>
        /// <param name="isMute"></param>
        public void Mute(bool isMute)
        {
            multimediaManager.OutputAudio = isMute;
        }

    }
}
