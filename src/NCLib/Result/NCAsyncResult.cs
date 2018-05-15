using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCLib
{
    /// <summary>
    /// 异步操作结果
    /// </summary>
    public class NCAsyncResult : IAsyncResult,IResult
    {
        private AsyncCallback _asyncCallback;
        private baseResult _baseResult;
        private string _info;

        public NCAsyncResult(AsyncCallback asyncCallback,object state)
        {
            AsyncState = state;
            _asyncCallback = asyncCallback;
        }

        /// <summary>
        /// 异步完成
        /// </summary>
        /// <param name="result"></param>
        public void SetCompleted(baseResult baseResult, string Info = "")
        {
            _baseResult = baseResult;
            _info = Info;
            _asyncCallback?.Invoke(this);
        }

        /// <summary>
        /// 异步完成
        /// </summary>
        /// <param name="result"></param>
        public void SetCompleted(IResult result)
        {
            _baseResult = result.BaseResult;
            _info = result.Info;
            _asyncCallback?.Invoke(this);
        }

        public object AsyncState
        {
            get;
            set;
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _info != null;
            }
        }

        public baseResult BaseResult
        {
            get
            {
                return _baseResult;
            }
        }

        public string Info
        {
            get
            {
                return _info;
            }
        }
    }
}
