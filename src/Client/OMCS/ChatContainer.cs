using ESBasic;
using OMCS.Contracts;
using OMCS.Passive;
using OMCS.Passive.MultiChat;
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
    public class ChatContainer
    {
        private IMultimediaManager multimediaManager;
        private IChatGroup chatGroup;
        /// <summary>
        /// 答疑室成员表
        /// </summary>
        private List<ChatMember> MemberList = new List<ChatMember>();

        #region 事件集
        public event Action<ChatMember> SomeoneJoin;
        public event Action<string> SomeoneExit;
        #endregion

        #region 事件监听
        /// <summary>
        /// 某人加入答疑室事件
        /// </summary>
        /// <param name="unit"></param>
        private void OnChatGroup_SomeoneJoin(IChatUnit unit)
        {
            ChatMember member = new ChatMember(unit);
            MemberList.Add(member);
            SomeoneJoin?.Invoke(member);
        }

        /// <summary>
        /// 某人离开答疑室事件
        /// </summary>
        /// <param name="memberID"></param>
        private void OnChatGroup_SomeoneExit(string memberID)
        {
            foreach (ChatMember Member in MemberList)
            {
                if (Member.MemberID == memberID)
                {
                    MemberList.Remove(Member);
                    break;
                }
            }
            SomeoneExit?.Invoke(memberID);
        }
        #endregion

        public ChatContainer(IMultimediaManager multimediaManager)
        {
            this.multimediaManager = multimediaManager;
        }

        /// <summary>
        /// 加入答疑室
        /// </summary>
        /// <param name="chatGroupID">组ID</param>
        public void JoinChatGroup(string chatGroupID)
        {
            //退出上一次答疑室
            if (chatGroup != null)
            {
                this.multimediaManager.ChatGroupEntrance.Exit(ChatType.Audio, this.chatGroup.GroupID);
                MemberList.ForEach(chatMember => { chatMember.Dispose(); });
                MemberList.Clear();
            }
            //加入答疑室
            this.chatGroup = multimediaManager.ChatGroupEntrance.Join(ChatType.Audio, chatGroupID);
            this.chatGroup.SomeoneJoin += OnChatGroup_SomeoneJoin;
            this.chatGroup.SomeoneExit += OnChatGroup_SomeoneExit;

            //获取已在答疑室的成员 加入列表
            foreach (IChatUnit unit in this.chatGroup.GetOtherMembers())
            {
                ChatMember member = new ChatMember(unit);
                MemberList.Add(member);
                SomeoneJoin?.Invoke(member);
            }
        }

        public void Close()
        {
            if (this.multimediaManager != null)
            {
                //退出聊天室
                if (chatGroup != null)
                    this.multimediaManager.ChatGroupEntrance.Exit(ChatType.Audio, this.chatGroup.GroupID);
                chatGroup = null;
            }
        }

    }
}
