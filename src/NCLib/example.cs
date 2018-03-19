using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// 自定义结果类
    /// </summary>
    public class exResult : Result
    {
        /// <summary>
        /// 添加新结果类型（举例）
        /// </summary>
        public int Num;
        public exResult(baseResult baseResult, int num, string info = null) : base(baseResult, info)
        {
            this.Num = num;
        }
    }

    /// <summary>
    /// 假设这是对IServerCallDatabase接口的实现类
    /// </summary>
    public class example : IServerCallDatabase
    {
        /// <summary>
        /// 实现该接口返回自定义结果
        /// </summary>
        public Result AddFriend(string userId, string friendId)
        {
            exResult result = new exResult(baseResult.Faild, 23, "失败啦");
            return result;
        }

        #region 未实现接口
        public Result AddUser(UserInfo info, string password)
        {
            throw new NotImplementedException();
        }

        public void CloseDatabase()
        {
            throw new NotImplementedException();
        }

        public Result ConnectDatabase(string user, string password, string url)
        {
            throw new NotImplementedException();
        }

        public void DeleteFriend(string userId, string friendId)
        {
            throw new NotImplementedException();
        }

        public void DeletUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Result ExecuteStructuredQueryLanguage(string sql)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
