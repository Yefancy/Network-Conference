using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// 用于客户端的用户信息对象
    /// </summary>
    public class User_Client : UserInfo
    {
        /// <summary>
        /// 用户账号当前状态
        /// </summary>
        public UserState UserState;

        public User_Client(string id = null):base(id)
        {
            UserState = UserState.未登录;
        }
    }
}
