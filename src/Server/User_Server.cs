using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 用于服务端的用户信息对象
    /// </summary>
    public class User_Server : UserInfo
    {
        /// <summary>
        /// 用户网络端点(IP:port)
        /// </summary>
        public string RemoteEndPoint;
        /// <summary>
        /// 用户加入的房间
        /// </summary>
        public List<string> RoomIDs;

        public User_Server(string id = null):base(id)
        {
            RoomIDs = new List<string>();
        }
    }    
}
