using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    public class NCEnum
    {
        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="ins">字符串</param>
        /// <returns>枚举</returns>
        public static T String2Enum<T>(string ins)
        {
            return (T)Enum.Parse(typeof(T), ins);
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

    /// <summary>
    /// 服务器状态
    /// </summary>
    public enum ServerState
    {
        未初始化,
        初始化中,
        初始化完成,
        关闭
    }

    /// <summary>
    /// Terminal信息类型
    /// </summary>
    public enum InfoType
    {
        信息,
        请求,
        响应,
        发送,
        异常
    }

    /// <summary>
    /// 消息内容
    /// </summary>
    public enum MessageContent
    {
        错误,
        登录,
        创建答疑室,
        加入答疑室,
        退出答疑室,
        某人退出答疑室,
        静音某人,
        静音自己
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        错误,
        请求,
        响应,
        通知
    }

    /// <summary>
    /// 用户当前状态
    /// </summary>
    public enum UserState
    {
        已登录,
        未登录
    }
}
