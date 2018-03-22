using NCLib;
using System;
using static NCLibExample.Achieveinterface;

namespace NCLibExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //对于负责别的模块的开发者来说并不知道IServerCallDatabase里面的实现 可以先引用这个接口
            IServerCallDatabase ex;
            //当别人对这个接口实现后（例如在NCLib实现为example类）就能直接实例使用了
            ex = new example();

            Result result = ex.AddFriend("", "");

            Console.WriteLine(result.BaseResult + result.Info);//任何情况下都能识别基本结果类的结果
            Console.WriteLine((result as exResult).Num);//在知道自定义结果类型下 查看强制转换后的自定义结果信息

            //输出结果:
            //Faild失败啦
            //23
        }
    }
}
