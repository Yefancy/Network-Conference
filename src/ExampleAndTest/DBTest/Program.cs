using NCLib;
using Server2DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IServerCallDatabase scd = new ServerCallDatabase();
            Result r = scd.ConnectDatabase("sa","1213141516","127.0.0.1","NCDB");
            #region Shop
            //scd.ExecuteStructuredQueryLanguage(@"select 名称 from 商店 where 商店.地址 LIKE '上海%'","aa");
            //foreach (DataRow theRow in scd.TmpDataSet.Tables["aa"].Rows)
            //{
            //    Console.WriteLine(theRow["名称"]);
            //}
            //scd.ExecuteStructuredQueryLanguage(@"select 名称 from 商店 where 商店.地址 LIKE '北京%'", "aa");
            //foreach (DataRow theRow in scd.TmpDataSet.Tables["aa"].Rows)
            //{
            //    Console.WriteLine(theRow["名称"]);
            //}
            //Console.Read();
            #endregion
            #region check
            UserInfo userInfo = new UserInfo();
            //userInfo.UserId = "1501";
            //bool k = scd.CheckUserInfo(userInfo);
            //userInfo.UserId = "0000";
            //bool k1 = scd.CheckUserInfo(userInfo);
            //userInfo.UserId = "15021";
            //bool k2 = scd.CheckUserInfo(userInfo);
            userInfo.UserId = "1501";
            bool k3 = scd.IsPrerogative(userInfo);
            userInfo.UserId = "0000";
            bool k4 = scd.IsPrerogative(userInfo);
            #endregion

        }
    }
}
