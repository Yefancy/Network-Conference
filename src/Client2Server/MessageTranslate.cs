using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client2Server
{
    /// <summary>
    /// 拆装包传输信息
    /// </summary>
    public class MessageTranslate
    {
        #region 静态字段
        //private static string _getType = @"(?<type>.*?) (?<message>.*)";
        private static string _loginRequest = "学号 密码";
        private static string _loginRespond = "结果 权限";
        private static string _createRoomRequest = "房间名 密码";
        private static string _createRoomRespond = "结果 描述";
        private static string _joinRoomRequest = "房间名 密码";
        private static string _joinRoomRespond = "结果 描述 创建者";
        private static string _exitRoomInform = "房间名";
        private static string _someoneExitRoomInform = "房间名 学号";
        private static string _muteSomeoneInform = "房间名 学号 是否静音";
        private static string _muteInform = "房间名 是否静音";
        private static char[] _delimiterChars = { '[', ']', ',' };
        #endregion

        /// <summary>
        /// 拼接信息
        /// </summary>
        /// <param name="field"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        private static string SplitJointInfo(string field, string[] infos)
        {
            string result = "";
            string[] words = field.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                result += words[i] + "," + infos[i] + ",";
            }
            return result.Remove(result.Length - 1);
        }

        /// <summary>
        /// 封装信息
        /// </summary>
        /// <param name="messageContent"></param>
        /// <param name="messageType"></param>
        /// <param name="infos">多参数</param>
        /// <returns></returns>
        public static string EncapsulationInfo(MessageContent messageContent, MessageType messageType, params string[] infos)
        {
            string result = "[" + messageContent.ToString() + "," + messageType.ToString() + "]";
            try
            {
                switch (messageContent)
                {
                    case MessageContent.登录:
                        if (messageType == MessageType.请求)
                        {
                            return result + SplitJointInfo(_loginRequest, infos);
                        }
                        else if (messageType == MessageType.响应)
                        {
                            return result + SplitJointInfo(_loginRespond, infos);
                        }
                        break;
                    case MessageContent.创建答疑室:
                        if (messageType == MessageType.请求)
                        {
                            return result + SplitJointInfo(_createRoomRequest, infos);
                        }
                        else if (messageType == MessageType.响应)
                        {
                            return result + SplitJointInfo(_createRoomRespond, infos);
                        }
                        break;
                    case MessageContent.加入答疑室:
                        if (messageType == MessageType.请求)
                        {
                            return result + SplitJointInfo(_joinRoomRequest, infos);
                        }
                        else if (messageType == MessageType.响应)
                        {
                            return result + SplitJointInfo(_joinRoomRespond, infos);
                        }
                        break;
                    case MessageContent.退出答疑室:
                        if (messageType == MessageType.通知)
                        {
                            return result + SplitJointInfo(_exitRoomInform, infos);
                        }
                        break;
                    case MessageContent.某人退出答疑室:
                        if (messageType == MessageType.通知)
                        {
                            return result + SplitJointInfo(_someoneExitRoomInform, infos);
                        }
                        break;
                    case MessageContent.静音某人:
                        if (messageType == MessageType.通知)
                        {
                            return result + SplitJointInfo(_muteSomeoneInform, infos);
                        }
                        break;
                    case MessageContent.静音自己:
                        if (messageType == MessageType.通知)
                        {
                            return result + SplitJointInfo(_muteInform, infos);
                        }
                        break;
                }
            }
            catch
            {
                throw new Exception("错误的入参" + messageContent.ToString() + "|" + messageType.ToString() + ":" + infos);
            }
            return result;
        }

        /// <summary>
        /// 分析信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="messageContent"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AnalyseInfo(string info, out MessageContent messageContent, out MessageType messageType)
        {
            string[] words = info.Split(_delimiterChars);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (words.Length < 2 || words.Length % 2 != 1)
            {
                messageType = MessageType.错误;
                messageContent = MessageContent.错误;
                return dic;
            }
            else
            {
                messageContent = NCEnum.String2Enum<MessageContent>(words[1]);
                messageType = NCEnum.String2Enum<MessageType>(words[2]);
                for (int i = 3; i < words.Length - 1; i = i + 2)
                {
                    dic[words[i]] = words[i + 1];
                }
            }
            return dic;
        }
    }
}
