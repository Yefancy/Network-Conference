using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// 用户信息(抽象)
    /// </summary>
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
    /// 用户组(抽象)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UserGroup<T>
    {
        public event Action<T> UserJoin;
        public event Action<T> UserExit;

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName;
        /// <summary>
        /// 创建者
        /// </summary>
        public T Builder;
        /// <summary>
        /// 组成员
        /// </summary>
        public NCList<T> memberList;
        public int Count
        {
            get { return memberList == null ? 0 : memberList.Count; }
        }

        public UserGroup()
        {
            memberList = new NCList<T>();
            memberList.ItemAdded += a => { UserJoin?.Invoke(a); };
            memberList.ItemRemoved += a => { UserExit?.Invoke(a); };
        }
    }  
}
