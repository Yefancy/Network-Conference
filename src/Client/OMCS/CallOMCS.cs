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
    public abstract class CallOMCS : IDisposable
    {
        /// <summary>
        /// 设备管理器
        /// </summary>
        protected IMultimediaManager multimediaManager;
        /// <summary>
        /// 聊天组容器
        /// </summary>
        protected ChatContainer chatContainer;

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

        public CallOMCS()
        {
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
            try
            {
                chatContainer.Close();                
                multimediaManager.Dispose();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        #endregion

        /// <summary>
        /// 绑定chatContainer事件
        /// </summary>
        protected void BindingEvent()
        {
            chatContainer.UserJoin += a => { SomeoneJoin?.Invoke(a); };
            chatContainer.UserExit += a => { SomeoneExit?.Invoke(a); };
            chatContainer.ConnectEnded += a => { ConnectEnded?.Invoke(a); };
        }

        /// <summary>
        /// 加入答疑室
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <param name="teacherID">教师ID</param>
        public void JoinRoom(string roomID, string teacherID)
        {
            if (chatContainer == null)
            {
                throw new Exception("未初始化CallOMCS类实例");
            }
            ExitRoom(); //离开上一个答疑室           
            chatContainer.JoinChatGroup(roomID, teacherID);
            chatContainer.IsWhiteBoardWatchingOnly = true;
        }

        /// <summary>
        /// 离开答疑室
        /// </summary>
        public void ExitRoom()
        {            
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
            multimediaManager.OutputAudio = !isMute;
        }

    }
}
