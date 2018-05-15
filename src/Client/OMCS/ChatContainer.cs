using ESBasic;
using NCLib;
using OMCS.Contracts;
using OMCS.Passive;
using OMCS.Passive.MultiChat;
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
    /// 聊天组容器
    /// </summary>
    public class ChatContainer : UserGroup<ChatMember>
    {
        private IMultimediaManager multimediaManager;
        private IChatGroup chatGroup;
        /// <summary>
        /// 摄像头连接器（控件）
        /// </summary>
        private CameraConnector CameraControl;
        /// <summary>
        /// 白板连接器（控件）
        /// </summary>
        private WhiteBoardConnector WhiteBoardControl;     
        private bool _isWhiteBoardWatchingOnly=true;
        /// <summary>
        /// 白板仅可见不能修改
        /// </summary>
        public bool IsWhiteBoardWatchingOnly
        {
            get
            {
                return _isWhiteBoardWatchingOnly;
            }

            set
            {
                if (WhiteBoardControl != null)
                {
                    WhiteBoardControl.WatchingOnly = value;
                    _isWhiteBoardWatchingOnly = value;
                }
            }
        }

        #region 事件集
        public event Action<Result> ConnectEnded;
        #endregion

        #region 事件监听
        /// <summary>
        /// 某人加入答疑室事件
        /// </summary>
        /// <param name="unit"></param>
        private void OnChatGroup_SomeoneJoin(IChatUnit unit)
        {
            ChatMember member = new ChatMember(unit);
            memberList.Add(member);
        }

        /// <summary>
        /// 某人离开答疑室事件
        /// </summary>
        /// <param name="memberID"></param>
        private void OnChatGroup_SomeoneExit(string memberID)
        {
            foreach (ChatMember Member in memberList)
            {
                if (Member.MemberID == memberID)
                {
                    memberList.Remove(Member);
                    break;
                }
            }            
        }

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

        public ChatContainer(IMultimediaManager multimediaManager, CameraConnector CameraConnector = null, WhiteBoardConnector WhiteBoardControl = null)
        {
            this.multimediaManager = multimediaManager;
            this.CameraControl = CameraConnector;
            this.WhiteBoardControl = WhiteBoardControl;
            if (CameraControl != null)
                CameraControl.ConnectEnded += OnConnectEnded;
            if (WhiteBoardControl != null)
                WhiteBoardControl.ConnectEnded += OnConnectEnded;
        }

        /// <summary>
        /// 加入答疑室
        /// </summary>
        /// <param name="chatGroupID">组ID</param>
        public void JoinChatGroup(string chatGroupID, string teacherID)
        {
            //退出上一次答疑室
            if (chatGroup != null)
            {
                this.multimediaManager.ChatGroupEntrance.Exit(ChatType.Audio, this.chatGroup.GroupID);
                Builder = null;
                memberList.ForEach(chatMember => { chatMember.Dispose(); });
                memberList.Clear();
            }
            //加入答疑室
            if (CameraControl != null)
                this.CameraControl.BeginConnect(teacherID);
            if (WhiteBoardControl != null)
                this.WhiteBoardControl.BeginConnect(chatGroupID);
            this.chatGroup = multimediaManager.ChatGroupEntrance.Join(ChatType.Audio, chatGroupID);
            this.chatGroup.SomeoneJoin += OnChatGroup_SomeoneJoin;
            this.chatGroup.SomeoneExit += OnChatGroup_SomeoneExit;
            this.GroupName = chatGroupID;
            //获取已在答疑室的成员 加入列表
            foreach (IChatUnit unit in this.chatGroup.GetOtherMembers())
            {
                ChatMember member = new ChatMember(unit);
                memberList.Add(member);
            }
            this.Builder = this.memberList.Find(a => { return a.MemberID == teacherID; });
        }

        /// <summary>
        /// 关闭答疑室
        /// </summary>
        public void Close()
        {
            if (this.multimediaManager != null)
            {
                //退出聊天室
                if (chatGroup != null)
                    this.multimediaManager.ChatGroupEntrance.Exit(ChatType.Audio, this.chatGroup.GroupID);
                chatGroup = null;
                if (CameraControl != null)
                    if (CameraControl.Connected)
                        CameraControl.Disconnect();
                if (WhiteBoardControl != null)
                    if (WhiteBoardControl.Connected)
                        WhiteBoardControl.Disconnect();
            }
        }

    }
}
