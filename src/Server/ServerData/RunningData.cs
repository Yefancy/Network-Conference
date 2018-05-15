using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCLib;

namespace Server.ServerData
{
    public class RunningData : IRunningData
    {
        private Dictionary<string, UserInfo> _Clients_remoteEndPoint;
        private Dictionary<string, string> _Clients_id;
        private Dictionary<string, ChatRoom> _ChatRoom_id;
        private string _ip, _dbUser, _dbPassword, _dbUrl, _database;
        private int _port;
        private ServerState _isInit;

        #region 数据封装
        public Dictionary<string, UserInfo> Clients_remoteEndPoint
        {
            get
            {
                return _Clients_remoteEndPoint;
            }

            set
            {
                _Clients_remoteEndPoint = value;
            }
        }
        public Dictionary<string, string> Clients_id
        {
            get
            {
                return _Clients_id;
            }

            set
            {
                _Clients_id = value;
            }
        }
        public Dictionary<string, ChatRoom> ChatRoom_id
        {
            get
            {
                return _ChatRoom_id;
            }

            set
            {
                _ChatRoom_id = value;
            }
        }
        public string Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
            }
        }
        public string DbUser
        {
            get
            {
                return _dbUser;
            }

            set
            {
                _dbUser = value;
            }
        }
        public string DbPassword
        {
            get
            {
                return _dbPassword;
            }

            set
            {
                _dbPassword = value;
            }
        }
        public string DbUrl
        {
            get
            {
                return _dbUrl;
            }

            set
            {
                _dbUrl = value;
            }
        }
        public string Database
        {
            get
            {
                return _database;
            }

            set
            {
                _database = value;
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
            }
        }
        public ServerState IsInit
        {
            get
            {
                return _isInit;
            }

            set
            {
                _isInit = value;
            }
        }
        #endregion

        public RunningData(string IP, int port, string user, string password, string url, string database)
        {
            _Clients_remoteEndPoint = new Dictionary<string, UserInfo>();
            _Clients_id = new Dictionary<string, string>();
            _ChatRoom_id = new Dictionary<string, ChatRoom>();
            this._ip = IP;
            this._port = port;
            this._dbUser = user;
            this._dbPassword = password;
            this._dbUrl = url;
            this._database = database;
            _isInit = ServerState.未初始化;
        }
    }
}
