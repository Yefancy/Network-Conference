using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ServerData
{
    public class ChatRoom : UserGroup<UserInfo>
    {
        public string Password;

        public event Action<ChatRoom> RoomEmpty;

        public ChatRoom(string password = "")
        {
            Password = password;
            memberList.ItemAdded += a => { Terminal.ServerPrint(InfoType.信息, "学号" + a.UserId + "加入" + GroupName + "答疑室"); };
            memberList.ItemRemoved += 
                a => 
            {                
                Terminal.ServerPrint(InfoType.信息, "学号" + a.UserId + "离开" + GroupName + "答疑室");
                if (this.memberList.Count == 0)
                    RoomEmpty?.Invoke(this);
            };
        }
    }
}
