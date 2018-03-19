using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// 结果类
    /// </summary>
    public class Result
    {
        private baseResult _baseResult;

        /// <summary>
        /// 基本结果（只读）
        /// </summary>
        public baseResult BaseResult
        {
            get
            {
                return _baseResult;
            }
        }

        /// <summary>
        /// 结果的初始化（只能初始化一次 初始化后结果不能修改）
        /// </summary>
        /// <param name="baseResult">结果枚举</param>
        public Result(baseResult baseResult)
        {
            this._baseResult = baseResult;
        }
    }

    /// <summary>
    /// 基本结果枚举：
    /// 0-成功 1-失败 2-未知
    /// <summary>
    public enum baseResult
    {
        Successful,
        Faild,
        Unknown
    }
}
