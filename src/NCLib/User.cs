using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    public abstract class UserInfo
    {
        /// <summary>
        /// 用户ID唯一
        /// </summary>
        public string UserId;
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName;
        /// <summary>
        /// 特权用户
        /// </summary>
        public bool IsPrerogative;

        public UserInfo(string id = null)
        {
            UserId = id;
        }
    }

    /// <summary>
    /// 用户类
    /// </summary>
    public class ClientUser: UserInfo
    {
        /// <summary>
        /// 用户网络端点(IP:port)
        /// </summary>
        public string remoteEndPoint;
        /// <summary>
        /// 用户账号当前状态
        /// </summary>
        public UserState UserState;

        public ClientUser(string id = null):base(id)
        {
            UserState = UserState.未登录;
        }
    }

   
}
