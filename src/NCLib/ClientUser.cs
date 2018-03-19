using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// 用户类
    /// </summary>
    public class ClientUser
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
        /// 用户登录客户端IP
        /// </summary>
        public string IP;
        /// <summary>
        /// 用户客户端端口
        /// </summary>
        public int Port; 
        /// <summary>
        /// 用户账号当前状态
        /// </summary>
        public State UserState;
    }

    /// <summary>
    /// 用户当前状态
    /// </summary>
    public enum State
    {
        Online,
        Offline
    }
}
