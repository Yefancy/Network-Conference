using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NCLib.AOP
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AOPAttribute : ContextAttribute, IContributeObjectSink
    {
        public AOPAttribute() : base("AOP")
        {
        }

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            return new AOPHandler(nextSink);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AOPMethodAttribute : Attribute
    {
        private string _info;
        public AOPMethodHandler PreProceed;
        public AOPMethodHandler PostProceed;

        public AOPMethodAttribute(string info, AOPMethodHandler PreProceed, AOPMethodHandler PostProceed)
        {
            _info = info;
            this.PreProceed = PreProceed;
            this.PostProceed = PostProceed;
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
