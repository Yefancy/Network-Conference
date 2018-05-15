using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NCLib.AOP
{
    public class AOPHandler : IMessageSink
    {
        private IMessageSink nextSink;

        public AOPHandler(IMessageSink nextSink)
        {
            this.nextSink = nextSink;
        }

        public IMessageSink NextSink
        {
            get
            {
                return nextSink;
            }
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="replySink"></param>
        /// <returns></returns>
        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IMessage SyncProcessMessage(IMessage msg)
        {
            IMessage message = null;

            //方法调用接口
            IMethodCallMessage callMessage = msg as IMethodCallMessage;

            //如果被调用的方法没打MethodAttribute标签
            if (callMessage == null || (Attribute.GetCustomAttribute(callMessage.MethodBase, typeof(AOPMethodAttribute))) == null)
            {
                message = nextSink.SyncProcessMessage(msg);
            }
            else
            {
                AOPMethodAttribute attribute = (AOPMethodAttribute)Attribute.GetCustomAttribute(callMessage.MethodBase, typeof(AOPMethodAttribute));
                PreProceed(msg, attribute);
                message = nextSink.SyncProcessMessage(msg);
                PostProceed(message, attribute);
            }
            return message;
        }

        /// <summary>
        /// 方法执行前
        /// </summary>
        /// <param name="msg">消息</param>
        public void PreProceed(IMessage msg, AOPMethodAttribute attribute)
        {
            IMethodMessage message = (IMethodMessage)msg;
            attribute.PreProceed?.Invoke(message, attribute.Info);
        }

        /// <summary>
        /// 方法执行后
        /// </summary>
        /// <param name="msg">消息</param>
        public void PostProceed(IMessage msg, AOPMethodAttribute attribute)
        {
            IMethodReturnMessage message = (IMethodReturnMessage)msg;
            attribute.PostProceed?.Invoke(message, attribute.Info);
        }
    }
}
