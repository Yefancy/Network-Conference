using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ServerData
{
    public class ReceiveInfo : IReceiveInfo
    {
        private Dictionary<string, string> _message;
        private string _remoteEndPoint;

        #region 封装数据
        public Dictionary<string, string> Message
        {
            get
            {
                return _message;
            }
        }
        public string RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }
        }
        #endregion

        public ReceiveInfo(Dictionary<string, string> message, string remoteEndPoint)
        {
            _message = message;
            _remoteEndPoint = remoteEndPoint;
        }

    }
}
