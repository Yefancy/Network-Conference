using ESBasic;
using OMCS.Passive.MultiChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ChatMember: IDisposable
    {
        private IChatUnit chatUnit;
        public string MemberID
        {
            get
            {
                if (this.chatUnit == null)
                {
                    return null;
                }

                return this.chatUnit.MemberID;
            }
        }

        #region 事件集
        /// <summary>
        /// 成员音频输出状态改变事件 有输出:true 无输出:false
        /// </summary>
        public event Action<bool> OwnerOutputChanged;
        /// <summary>
        /// 成员音频数据流接收事件
        /// </summary>
        public event Action<byte[]> AudioDataReceived;
        #endregion

        public ChatMember(IChatUnit chatUnit)
        {
            this.chatUnit = chatUnit;
            //this.chatUnit.MicrophoneConnector.ConnectEnded += ;
            this.chatUnit.MicrophoneConnector.OwnerOutputChanged += ()=> { OwnerOutputChanged?.Invoke(this.chatUnit.MicrophoneConnector.OwnerOutput); };
            this.chatUnit.MicrophoneConnector.AudioDataReceived += a => { AudioDataReceived?.Invoke(a); };
            //开始连接到目标成员的麦克风设备
            this.chatUnit.MicrophoneConnector.BeginConnect(chatUnit.MemberID);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            chatUnit.MicrophoneConnector.Dispose();
        }
    }
}
