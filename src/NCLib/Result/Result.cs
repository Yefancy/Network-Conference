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
    public class Result: IResult
    {
        /// <summary>
        /// 基本结果
        /// </summary>
        private baseResult _baseResult;
        /// <summary>
        /// 结果信息
        /// </summary>
        private string _info;
        
        /// <summary>
        /// 基本结果(只读)
        /// </summary>
        public baseResult BaseResult
        {
            get
            {
                return _baseResult;
            }
        }
        /// <summary>
        /// 结果信息(只读)
        /// </summary>
        public string Info
        {
            get
            {
                return _info;
            }
        }

        /// <summary>
        /// 结果的初始化
        /// </summary>
        /// <param name="baseResult">结果枚举</param>
        /// <param name="info">结果信息</param>
        public Result(baseResult baseResult, string info = "")
        {
            this._baseResult = baseResult;
            this._info = info;
        }
    }
}
